using EnrichedEnumModelBinding.Enums;

namespace EnrichedEnumModelBinding.Requests
{
    public sealed class WeatherRequest
    {
        public ForecastPeriod Period { get; init; }
    }
}