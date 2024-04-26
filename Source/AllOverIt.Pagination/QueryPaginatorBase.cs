using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Reflection;
using System.Collections.Concurrent;
using System.Reflection;

namespace AllOverIt.Pagination
{
    /// <summary>Base query paginator. Contains a static registry of type comparisons.</summary>
    public abstract class QueryPaginatorBase
    {
        // base class mainly exists to keep non-generic statics out of the generic implementations
        private static readonly ConcurrentDictionary<Type, MethodInfo> ComparisonMethods;

        static QueryPaginatorBase()
        {
            var comparableTypes = new[]
            {
                CommonTypes.BoolType,
                CommonTypes.StringType,
                CommonTypes.GuidType
            };

            var registry = new ConcurrentDictionary<Type, MethodInfo>();

            foreach (var type in comparableTypes)
            {
                var compareTo = type
                    .GetTypeInfo()
                    .GetMethod(nameof(IComparable.CompareTo), [type]);

                compareTo.CheckNotNull(nameof(compareTo), $"The type {type.GetFriendlyName()} does not provide a {nameof(IComparable.CompareTo)}() method.");

                registry.TryAdd(type, compareTo);
            }

            ComparisonMethods = registry;
        }

        internal static bool TryGetComparisonMethodInfo(Type type, out MethodInfo methodInfo)
        {
            // Enum's are IComparable but we can't pre-register the types we don't know about - so register them as they arrive
            if (type.IsEnum)
            {
                methodInfo = ComparisonMethods.GetOrAdd(type, key =>
                {
                    return type
                        .GetTypeInfo()
                        .GetMethod(nameof(Enum.CompareTo), [type]);
                });

                return true;
            }

            return ComparisonMethods.TryGetValue(type, out methodInfo);
        }
    }
}