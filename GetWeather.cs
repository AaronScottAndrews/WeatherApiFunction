
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace WeatherApiFunction
{
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
		public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "weather")] HttpRequestData req, FunctionContext executionContext)
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

			var url = $"http://api.weatherapi.com/v1/forecast.json?key={apiKey}&q={location}&days=3";

			try
			{
				var response = await _httpClient.GetAsync(url);
				var content = await response.Content.ReadAsStringAsync();

				using var doc = JsonDocument.Parse(content);

				var root = doc.RootElement;

				WeatherDTO weatherForcast = BuildDto(root);

				var filteredJson = JsonSerializer.Serialize(weatherForcast);

				var okResponse = req.CreateResponse(System.Net.HttpStatusCode.OK);

				okResponse.StatusCode = okResponse.StatusCode;

				okResponse.Headers.Add("Content-Type", "application/json");

				await okResponse.WriteStringAsync(filteredJson);

				return okResponse;
			}
			catch (HttpRequestException ex)
			{
				logger.LogError($"Weather API call failed: {ex.Message}");
				var errorResponse = req.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
				await errorResponse.WriteStringAsync("Error calling weather API.");
				return errorResponse;
			}

			static WeatherDTO BuildDto(JsonElement root)
			{
				return new WeatherDTO
				{
					Location = root.GetProperty("location").GetProperty("name").GetString() ?? string.Empty,
					TemperatureF = root.GetProperty("current").GetProperty("temp_f").GetSingle(),
					Condition = root.GetProperty("current").GetProperty("condition").GetProperty("text").GetString() ?? string.Empty
				};
			}
		}
	}
}
