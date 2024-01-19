using EnrichedEnumModelBindingDemo.Enums;

namespace EnrichedEnumModelBindingDemo.Requests
{
    public sealed class WeatherRequest
    {
        public ForecastPeriod Period { get; init; }
    }
}