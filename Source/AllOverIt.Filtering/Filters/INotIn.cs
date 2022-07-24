namespace AllOverIt.Filtering.Filters
{
    /// <summary>Represents a filter operation that determines if a value is not within a set of other values.</summary>
    /// <typeparam name="TProperty">The property type.</typeparam>
    public interface INotIn<TProperty> : IArrayFilterOperation<TProperty>
    {
    }
}