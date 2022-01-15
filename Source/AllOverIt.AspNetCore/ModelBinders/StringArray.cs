using Microsoft.AspNetCore.Mvc;

namespace AllOverIt.AspNetCore.ModelBinders
{
    // Only applicable for models used to bind from a query string
    [ModelBinder(typeof(ValuesArrayModelBinder<StringArray, string>))]
    public sealed record StringArray : ValuesArray<string>
    {
    }
}
