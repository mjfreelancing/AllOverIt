using EnrichedEnumModelBinding.Enums;

namespace EnrichedEnumModelBinding.Requests
{
    public sealed class WeatherRequestMulti
    {
        public ForecastPeriodArray Periods { get; init; }
    }
}