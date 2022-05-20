using System.Reflection;

namespace AllOverIt.Pagination
{
    public interface IColumnDefinition
    {
        PropertyInfo Property { get; }
    }
}
