namespace AllOverIt.Filtering.Filters
{
    /// <summary>Represents a filter operation that determines if a value is within a set of other values.</summary>
    /// <typeparam name="TProperty">The property type.</typeparam>
    public interface IIn<TProperty> : IArrayFilterOperation<TProperty>
    {
    }
}