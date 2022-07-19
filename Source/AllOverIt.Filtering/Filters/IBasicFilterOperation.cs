namespace AllOverIt.Filtering.Filters
{
    public interface IBasicFilterOperation
    {
    }

    public interface IBasicFilterOperation<TProperty> : IBasicFilterOperation
    {
        TProperty Value { get; }
    }
}