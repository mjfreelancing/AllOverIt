namespace AllOverIt.Filtering.Filters
{
    public interface IGreaterThan<TProperty> : IFilterOperation
    {
        TProperty Value { get; }
    }
}