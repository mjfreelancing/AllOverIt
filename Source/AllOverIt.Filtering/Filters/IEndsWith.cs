namespace AllOverIt.Filtering.Filters
{
    public interface IEndsWith : IStringFilterOperation
    {
        string Value { get; }
    }
}