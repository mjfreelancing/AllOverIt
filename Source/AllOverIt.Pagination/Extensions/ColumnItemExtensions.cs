using AllOverIt.Extensions;
using AllOverIt.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace AllOverIt.Pagination.Extensions
{
    internal static class ColumnItemExtensions
    {
        public static IReadOnlyCollection<ColumnValueType> GetColumnValueTypes(this IReadOnlyCollection<IColumnDefinition> columns, object reference)
        {
            var referenceTypeInfo = reference.GetType().GetTypeInfo();

            return columns
                .SelectAsReadOnlyCollection(column =>
                {
                    var propertyInfo = ReflectionCache.GetPropertyInfo(referenceTypeInfo, column.Property.Name);
                    var value = propertyInfo.GetValue(reference);
                    var valueType = value.GetType();

                    return new ColumnValueType
                    {
                        Type = Type.GetTypeCode(valueType),
                        Value = value
                    };
                });
        }
    }
}
