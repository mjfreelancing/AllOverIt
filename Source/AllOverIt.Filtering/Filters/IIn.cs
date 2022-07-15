using System.Collections.Generic;

namespace AllOverIt.Filtering.Filters
{
    public interface IIn<TProperty> : IArrayFilterOperation
    {
        IList<TProperty> Values { get; }
    }
}