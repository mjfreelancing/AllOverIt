namespace AllOverIt.Filtering.Filters
{
    /// <summary>Represents a filter operation that determines if a value is not equal to another value.</summary>
    /// <typeparam name="TProperty">The property type.</typeparam>
    public interface INotEqualTo<TProperty> : IBasicFilterOperation<TProperty>
    {
    }
}