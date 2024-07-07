using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Reflection;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace AllOverIt.Pagination.Extensions
{
    internal static class ColumnItemExtensions
    {
        [return: NotNull]
        public static object?[] GetColumnValues(this IReadOnlyCollection<IColumnDefinition> columns, object reference)
        {
            _ = columns.WhenNotNullOrEmpty(nameof(columns));
            _ = reference.WhenNotNull(nameof(reference));

            // The reference type could be different to the entity type
            var referenceTypeInfo = reference.GetType().GetTypeInfo();

            return columns
                .SelectToArray(column =>
                {
                    return ReflectionCache
                        .GetPropertyInfo(referenceTypeInfo, column.Property.Name)!
                        .GetValue(reference);
                });
        }
    }
}
