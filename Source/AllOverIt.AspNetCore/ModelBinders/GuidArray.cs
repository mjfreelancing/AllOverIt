using Microsoft.AspNetCore.Mvc;
using System;

namespace AllOverIt.AspNetCore.ModelBinders
{
    // Only applicable for models used to bind from a query string
    [ModelBinder(typeof(ValuesArrayModelBinder<GuidArray, Guid>))]
    public sealed record GuidArray : ValuesArray<Guid>
    {
    }
}
