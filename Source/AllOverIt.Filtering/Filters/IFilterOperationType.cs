namespace AllOverIt.Filtering.Filters
{
    public interface IFilterOperationType<TProperty>
    {
        TProperty Value { get; }
    }
}