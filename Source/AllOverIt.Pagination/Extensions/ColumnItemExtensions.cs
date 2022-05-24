using AllOverIt.Extensions;
using AllOverIt.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;
using AllOverIt.Assertion;

namespace AllOverIt.Pagination.Extensions
{
    internal static class ColumnItemExtensions
    {
        public static IReadOnlyCollection<ColumnValueType> GetColumnValueTypes(this IReadOnlyCollection<IColumnDefinition> columns, object reference)
        {
            _ = columns.WhenNotNullOrEmpty(nameof(columns));
            _ = reference.WhenNotNull(nameof(reference));

            var referenceTypeInfo = reference.GetType().GetTypeInfo();

            return columns
                .SelectAsReadOnlyCollection(column =>
                {
                    var propertyInfo = ReflectionCache.GetPropertyInfo(referenceTypeInfo, column.Property.Name);
                    var type = propertyInfo.PropertyType;
                    var value = propertyInfo.GetValue(reference);

                    return new ColumnValueType
                    {
                        Type = Type.GetTypeCode(type),
                        Value = value
                    };
                });
        }
    }
}
