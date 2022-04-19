using EnrichedEnumModelBinding.Enums;
using EnrichedEnumModelBinding.Models;

namespace EnrichedEnumModelBinding.Requests
{
    public sealed class WeatherRequest
    {
        public ForecastPeriod Period { get; init; }
    }
}