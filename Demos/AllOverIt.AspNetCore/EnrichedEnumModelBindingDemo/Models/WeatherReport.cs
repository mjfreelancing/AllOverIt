namespace EnrichedEnumModelBindingDemo.Models
{
    public sealed class WeatherReport
    {
        public required string Title { get; set; }
        public required WeatherForecast[] Forecast { get; set; }
    }
}