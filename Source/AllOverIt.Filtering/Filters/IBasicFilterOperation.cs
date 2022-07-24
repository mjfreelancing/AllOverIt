namespace AllOverIt.Filtering.Filters
{
    /// <summary>Represents a basic filter operation for any non-string value type. Basic operations include those
    /// that can compare one value to another.</summary>
    public interface IBasicFilterOperation
    {
    }

    /// <inheritdoc />
    /// <typeparam name="TProperty">The non-string property type supported by this filter operation.</typeparam>
    public interface IBasicFilterOperation<TProperty> : IBasicFilterOperation, IFilterOperationType<TProperty>
    {
    }
}