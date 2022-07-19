using System.Collections.Generic;

namespace AllOverIt.Filtering.Filters
{
    public interface IArrayFilterOperation : IBasicFilterOperation
    {
    }

    public interface IArrayFilterOperation<TProperty> : IArrayFilterOperation
    {
        IList<TProperty> Values { get; }
    }
}