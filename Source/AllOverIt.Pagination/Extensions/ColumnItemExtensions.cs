using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Reflection;
using System.Collections.Generic;
using System.Reflection;

namespace AllOverIt.Pagination.Extensions
{
    internal static class ColumnItemExtensions
    {
        public static IReadOnlyCollection<object> GetColumnValues(this IReadOnlyCollection<IColumnDefinition> columns, object reference)
        {
            _ = columns.WhenNotNullOrEmpty(nameof(columns));
            _ = reference.WhenNotNull(nameof(reference));

            // The reference type could be different to the entity type
            var referenceTypeInfo = reference.GetType().GetTypeInfo();

            return columns
                .SelectAsReadOnlyCollection(column =>
                {
                    return ReflectionCache
                        .GetPropertyInfo(referenceTypeInfo, column.Property.Name)
                        .GetValue(reference);
                });
        }
    }
}
