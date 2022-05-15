using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using AllOverIt.Assertion;
using AllOverIt.Extensions;

namespace KeysetPaginationConsole.KeysetPagination
{
    public abstract class PaginationQueryBuilderBase
    {
        // base class mainly exists to keep statics out of the generic implementations
        private static readonly IReadOnlyDictionary<Type, MethodInfo> _typeComparisonMethods;
        protected static readonly ConstantExpression ConstantZeroExpression = Expression.Constant(0);

        static PaginationQueryBuilderBase()
        {
            var comparableTypes = new[]
            {
                typeof(bool),
                typeof(string),
                typeof(Guid)
            };

            var registry = new Dictionary<Type, MethodInfo>();

            foreach (var type in comparableTypes)
            {
                var compareTo = type
                    .GetTypeInfo()
                    .GetMethod(nameof(IComparable.CompareTo), new[] { type });

                compareTo.CheckNotNull(nameof(compareTo), $"The type {type.GetFriendlyName()} does not provide a {nameof(IComparable.CompareTo)}() method.");

                registry.Add(type, compareTo);
            }

            _typeComparisonMethods = registry;
        }

        protected static bool TryGetComparisonMethodInfo(Type type, out MethodInfo methodInfo)
        {
            return _typeComparisonMethods.TryGetValue(type, out methodInfo);
        }
    }
}