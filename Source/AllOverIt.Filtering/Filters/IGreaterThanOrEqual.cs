namespace AllOverIt.Filtering.Filters
{
    /// <summary>Represents a filter operation that determines if a value is greater than or equal to another value.</summary>
    /// <typeparam name="TProperty">The property type.</typeparam>
    public interface IGreaterThanOrEqual<TProperty> : IBasicFilterOperation<TProperty>
    {
    }
}