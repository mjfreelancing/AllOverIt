namespace AllOverIt.Filtering.Filters
{
    public interface IGreaterThan<TProperty> : IOperation
    {
        TProperty Value { get; }
    }
}