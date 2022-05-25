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

            var referenceTypeInfo = reference.GetType().GetTypeInfo();

            return columns
                .SelectAsReadOnlyCollection(column =>
                {
                    var propertyInfo = ReflectionCache.GetPropertyInfo(referenceTypeInfo, column.Property.Name);
                    var type = propertyInfo.PropertyType;

                    return propertyInfo.GetValue(reference);
                });
        }
    }
}
