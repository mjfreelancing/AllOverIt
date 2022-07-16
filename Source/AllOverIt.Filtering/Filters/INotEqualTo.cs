namespace AllOverIt.Filtering.Filters
{
    public interface INotEqualTo<TProperty> : IFilterOperation
    {
        TProperty Value { get; }
    }
}