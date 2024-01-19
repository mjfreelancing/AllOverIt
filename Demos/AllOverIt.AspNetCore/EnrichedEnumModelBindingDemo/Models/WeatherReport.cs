using System.Collections.Generic;

namespace EnrichedEnumModelBindingDemo.Models
{
    public sealed class WeatherReport
    {
        public string Title { get; set; }
        public IEnumerable<WeatherForecast> Forecast { get; set; }
    }
}