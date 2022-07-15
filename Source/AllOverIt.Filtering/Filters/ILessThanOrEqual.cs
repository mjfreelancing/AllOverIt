namespace AllOverIt.Filtering.Filters
{
    public interface ILessThanOrEqual<TProperty> : IFilterOperation
    {
        TProperty Value { get; }
    }
}