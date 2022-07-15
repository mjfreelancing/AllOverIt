namespace AllOverIt.Filtering.Filters
{
    public interface IContains : IStringOperation
    {
        string Value { get; }
    }
}