using EnrichedEnumModelBindingDemo.Enums;

namespace EnrichedEnumModelBindingDemo.Requests
{
    public sealed class WeatherRequestMulti
    {
        public ForecastPeriodArray? Periods { get; init; }
    }
}