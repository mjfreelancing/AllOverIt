using AllOverIt.AspNetCore.ModelBinders;
using AllOverIt.AspNetCore.ValueArray;
using Microsoft.AspNetCore.Mvc;

namespace EnrichedEnumModelBindingDemo.Enums
{
    // Only applicable for models used to bind from a query string
    [ModelBinder(typeof(ValueArrayModelBinder<ForecastPeriodArray, ForecastPeriod>))]
    public sealed class ForecastPeriodArray : ValueArray<ForecastPeriod>
    {
    }
}