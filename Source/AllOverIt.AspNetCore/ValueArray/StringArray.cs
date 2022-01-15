using AllOverIt.AspNetCore.ModelBinders;
using Microsoft.AspNetCore.Mvc;

namespace AllOverIt.AspNetCore.ValueArray
{
    // Only applicable for models used to bind from a query string
    [ModelBinder(typeof(ValueArrayModelBinder<StringArray, string>))]
    public sealed record StringArray : ValueArray<string>
    {
    }
}
