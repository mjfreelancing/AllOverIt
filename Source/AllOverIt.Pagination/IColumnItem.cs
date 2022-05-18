using System.Reflection;

namespace AllOverIt.Pagination
{
    public interface IColumnItem
    {
        PropertyInfo Property { get; }
    }
}
