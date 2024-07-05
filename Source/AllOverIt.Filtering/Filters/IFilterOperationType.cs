namespace AllOverIt.Filtering.Filters
{
    /// <summary>Represents the property type on a filter comparison operation.</summary>
    /// <typeparam name="TProperty">The property type.</typeparam>
    public interface IFilterOperationType<TProperty>
    {
        /// <summary>The value of the filter. Although the value is nullable, not all operations support <see langword="null"/>.</summary>
        TProperty? Value { get; }
    }
}