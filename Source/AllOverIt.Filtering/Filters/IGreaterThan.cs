namespace AllOverIt.Filtering.Filters
{
    /// <summary>Represents a filter operation that determines if a value is greater than another value.</summary>
    /// <typeparam name="TProperty">The property type.</typeparam>
    public interface IGreaterThan<TProperty> : IBasicFilterOperation<TProperty>
    {
    }
}