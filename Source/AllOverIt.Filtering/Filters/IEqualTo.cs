namespace AllOverIt.Filtering.Filters
{
    /// <summary>Represents a filter operation that determines if a value is equal to another value.</summary>
    /// <typeparam name="TProperty">The property type.</typeparam>
    public interface IEqualTo<TProperty> : IBasicFilterOperation<TProperty>
    {
    }
}