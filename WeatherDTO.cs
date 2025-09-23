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
	}
}
