namespace AllOverIt.Filtering.Filters
{
    public interface IBasicFilterOperation
    {
    }

    public interface IBasicFilterOperation<TProperty> : IBasicFilterOperation, IFilterOperationType<TProperty>
    {
        //TProperty Value { get; }
    }
}