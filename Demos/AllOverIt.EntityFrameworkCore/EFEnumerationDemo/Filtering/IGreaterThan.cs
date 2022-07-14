namespace EFEnumerationDemo.Filtering
{
    public interface IGreaterThan<TProperty> : IOperation
    {
        TProperty Value { get; }
    }
}