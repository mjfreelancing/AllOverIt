using AllOverIt.AspNetCore.ModelBinders;
using Microsoft.AspNetCore.Mvc;
using System;

namespace AllOverIt.AspNetCore.ValueArray
{
    /// <summary>Represents an array of GUIDs that can be bound to a model from a query string.</summary>
    [ModelBinder(typeof(ValueArrayModelBinder<GuidArray, Guid>))]
    public sealed record GuidArray : ValueArray<Guid>
    {
    }
}
