using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApiFunction
{
	internal class WeatherDTO
	{
		public string Location { get; set; }
		public float TemperatureF { get; set; }
		public string Condition { get; set; }
		public List<ForecastDayDTO> ForecastDays { get; set; } = new();
	}

	public class ForecastDayDTO
	{
		public string Date { get; set; } = string.Empty;
		public float MaxTempF { get; set; }
		public float MinTempF { get; set; }
		public string Condition { get; set; } = string.Empty;
		public string IconUrl { get; set; } = string.Empty;
	}
}
