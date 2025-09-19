
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

public class GetWeatherFunction
{
	private readonly HttpClient _httpClient;
	private readonly IConfiguration _config;

	public GetWeatherFunction(IHttpClientFactory httpClientFactory, IConfiguration config)
	{
		_httpClient = httpClientFactory.CreateClient();
		_config = config;
	}

	[Function("GetWeather")]
	public async Task<HttpResponseData> Run(
		[HttpTrigger(AuthorizationLevel.Function, "get", Route = "weather")] HttpRequestData req,
		FunctionContext executionContext)
	{
		var logger = executionContext.GetLogger("GetWeather");
		var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);

		var location = query["location"];
		var apiKey = _config["WeatherApiKey"];

		if (string.IsNullOrWhiteSpace(location))
		{
			var badResponse = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
			await badResponse.WriteStringAsync("Missing required 'location' query parameter.");
			return badResponse;
		}

		if (string.IsNullOrEmpty(apiKey))
		{
			var badResponse = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
			await badResponse.WriteStringAsync("Missing WeatherApiKey in configuration.");
			return badResponse;
		}

		var url = $"http://api.weatherapi.com/v1/current.json?key={apiKey}&q={location}&aqi=no";

		try
		{
			var response = await _httpClient.GetAsync(url);
			var content = await response.Content.ReadAsStringAsync();

			var okResponse = req.CreateResponse(System.Net.HttpStatusCode.OK);
			okResponse.Headers.Add("Content-Type", "application/json");
			await okResponse.WriteStringAsync(content);
			return okResponse;
		}
		catch (HttpRequestException ex)
		{
			logger.LogError($"Weather API call failed: {ex.Message}");
			var errorResponse = req.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
			await errorResponse.WriteStringAsync("Error calling weather API.");
			return errorResponse;
		}
	}
}
