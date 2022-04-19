using EnrichedEnumModelBinding.Enums;
using EnrichedEnumModelBinding.Models;

namespace EnrichedEnumModelBinding.Requests
{
    public sealed class WeatherRequestMulti
    {
        public ForecastPeriodArray Periods { get; init; }
    }
}