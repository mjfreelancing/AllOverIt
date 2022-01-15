using AllOverIt.AspNetCore.ModelBinders;
using Microsoft.AspNetCore.Mvc;

namespace AllOverIt.AspNetCore.ValueArray
{
    /// <summary>Represents an array of strings that can be bound to a model from a query string.</summary>
    [ModelBinder(typeof(ValueArrayModelBinder<IntegerArray, int>))]
    public sealed record IntegerArray : ValueArray<int>
    {
    }
}