namespace AllOverIt.Filtering.Filters
{
    public interface IContains : IStringFilterOperation
    {
        string Value { get; }
    }
}