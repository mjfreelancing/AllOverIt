namespace AllOverIt.Filtering.Filters
{
    public interface IGreaterThanOrEqual<TProperty> : IFilterOperation
    {
        TProperty Value { get; }
    }
}