namespace AllOverIt.Filtering.Filters
{
    /// <summary>Represents a filter operation that determines if a value is less than another value.</summary>
    /// <typeparam name="TProperty">The property type.</typeparam>
    public interface ILessThan<TProperty> : IBasicFilterOperation<TProperty>
    {
    }
}