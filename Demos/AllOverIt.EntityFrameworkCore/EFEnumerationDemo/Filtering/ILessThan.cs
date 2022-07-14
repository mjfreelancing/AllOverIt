namespace EFEnumerationDemo.Filtering
{
    public interface ILessThan<TProperty> : IOperation
    {
        TProperty Value { get; }
    }
}