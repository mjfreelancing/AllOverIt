using AllOverIt.AspNetCore.ModelBinders;
using AllOverIt.AspNetCore.ValueArray;
using Microsoft.AspNetCore.Mvc;

namespace EnrichedEnumModelBinding.Enums
{
    // Only applicable for models used to bind from a query string
    [ModelBinder(typeof(ValueArrayModelBinder<ForecastPeriodArray, ForecastPeriod>))]
    public sealed record ForecastPeriodArray : ValueArray<ForecastPeriod>
    {
    }
}