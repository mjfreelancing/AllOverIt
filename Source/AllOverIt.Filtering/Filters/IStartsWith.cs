namespace AllOverIt.Filtering.Filters
{
    public interface IStartsWith : IStringOperation
    {
        string Value { get; }
    }
}