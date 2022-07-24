namespace AllOverIt.Filtering.Filters
{
    /// <summary>Represents a filter operation that determines if a value is less than or equal to another value.</summary>
    /// <typeparam name="TProperty">The property type.</typeparam>
    public interface ILessThanOrEqual<TProperty> : IBasicFilterOperation<TProperty>
    {
    }
}