namespace EFEnumerationDemo.Filtering
{
    public interface IStartsWith : IStringOperation
    {
        string Value { get; }
    }
}