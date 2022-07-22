using AllOverIt.Assertion;
using AllOverIt.Extensions;
using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace AllOverIt.Pagination
{
    public abstract class QueryPaginatorBase
    {
        // base class mainly exists to keep statics out of the generic implementations
        private static readonly ConcurrentDictionary<Type, MethodInfo> _comparisonMethods;

        static QueryPaginatorBase()
        {
            var comparableTypes = new[]
            {
                typeof(bool),
                typeof(string),
                typeof(Guid)
            };

            var registry = new ConcurrentDictionary<Type, MethodInfo>();

            foreach (var type in comparableTypes)
            {
                var compareTo = type
                    .GetTypeInfo()
                    .GetMethod(nameof(IComparable.CompareTo), new[] { type });

                compareTo.CheckNotNull(nameof(compareTo), $"The type {type.GetFriendlyName()} does not provide a {nameof(IComparable.CompareTo)}() method.");

                registry.TryAdd(type, compareTo);
            }

            _comparisonMethods = registry;
        }

        protected static bool TryGetComparisonMethodInfo(Type type, out MethodInfo methodInfo)
        {
            // Enum's are IComparable but we can't pre-register the types we don't know about - so register them as they arrive
            if (type.IsEnum)
            {
                methodInfo = _comparisonMethods.GetOrAdd(type, key =>
                {
                    return type
                        .GetTypeInfo()
                        .GetMethod(nameof(Enum.CompareTo), new[] { type });
                });

                return true;
            }

            return _comparisonMethods.TryGetValue(type, out methodInfo);
        }
    }
}