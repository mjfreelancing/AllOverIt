namespace AllOverIt.Filtering.Filters
{
    public interface ILessThan<TProperty> : IOperation
    {
        TProperty Value { get; }
    }
}