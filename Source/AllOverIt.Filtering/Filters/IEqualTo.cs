namespace AllOverIt.Filtering.Filters
{
    public interface IEqualTo<TProperty> : IFilterOperation
    {
        TProperty Value { get; }
    }
}