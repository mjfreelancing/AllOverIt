namespace AllOverIt.Filtering.Filters
{
    public interface INotContains : IStringFilterOperation
    {
        string Value { get; }
    }
}