using System.Reflection;

namespace AllOverIt.Pagination
{
    /// <summary>Provides information about a property (column) that plays a role in ordering columns in a paginated query.</summary>
    public interface IColumnDefinition
    {
        /// <summary>The property (column) that is ordered in a paginated query.</summary>
        PropertyInfo Property { get; }
    }
}
