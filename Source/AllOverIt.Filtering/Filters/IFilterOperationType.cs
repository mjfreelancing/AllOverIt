namespace AllOverIt.Filtering.Filters
{
    /// <summary>Represents the propety type on a filter comparison operation.</summary>
    /// <typeparam name="TProperty">The property type.</typeparam>
    public interface IFilterOperationType<TProperty>
    {
        TProperty Value { get; }
    }
}