## Azure Function for WeatherAPI.com

This Azure function calls and returns data from WeatherAPI.com
This was created because calling WeatherAPI directly from a chrome extention wasn't possible.

### `Local Testing`

For local testing add you WeatherApi key to the “local.settings.json” file which is auto-generated when you create your HTTP trigger function app. 

### `After Deployment`

After deploying to Azure you will need to add the WeatherApiKey as an application setting.
To do this in the Azure Portal:

1. Login in and navigate to your function app

2. In the left pane of your function app, expand Settings, select Environment variables, and then select the App settings tab.

*Note, older documentation (and AI trained off older documentation) will tell you to select Configuration instead of Environment variables but this has changed.

3. Now click on +Add, add "WeatherApiKey" as the name and your WeatherAPI.com key as the value. Save and apply.

To test from the portal, click on Overview and then click on the GetWeather function. At the Code + Test tab click Test/Run.
Under Query parameters, add a name "location" and any city name for the value.