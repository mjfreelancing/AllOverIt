using System.Collections.Generic;

namespace AllOverIt.Filtering.Filters
{
    public interface INotIn<TProperty> : IArrayFilterOperation
    {
        IList<TProperty> Values { get; }
    }
}