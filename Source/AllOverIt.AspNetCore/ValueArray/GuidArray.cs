using AllOverIt.AspNetCore.ModelBinders;
using Microsoft.AspNetCore.Mvc;
using System;

namespace AllOverIt.AspNetCore.ValueArray
{
    // Only applicable for models used to bind from a query string
    [ModelBinder(typeof(ValueArrayModelBinder<GuidArray, Guid>))]
    public sealed record GuidArray : ValueArray<Guid>
    {
    }
}
