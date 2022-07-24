using System.Collections.Generic;

namespace AllOverIt.Filtering.Filters
{
    /// <summary>Represents a basic array-type filter operation for any value type. Basic array operations include
    /// those that can compare a value to a list of candidate values.</summary>
    public interface IArrayFilterOperation : IBasicFilterOperation
    {
    }

    /// <inheritdoc />
    /// <typeparam name="TProperty">The property type supported by this filter operation.</typeparam>
    public interface IArrayFilterOperation<TProperty> : IArrayFilterOperation, IFilterOperationType<IList<TProperty>>
    {
    }
}