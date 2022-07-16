using System;
using System.Reflection;

namespace AllOverIt.Filtering.Operations
{
    internal static class StringFilterMethodInfo
    {
        public static readonly MethodInfo Contains = typeof(string).GetMethod("Contains", new[] { typeof(string) });
        public static readonly MethodInfo ContainsStringComparison = typeof(string).GetMethod("Contains", new[] { typeof(string), typeof(StringComparison) });

        public static readonly MethodInfo StartsWith = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
        public static readonly MethodInfo StartsWithStringComparison = typeof(string).GetMethod("StartsWith", new[] { typeof(string), typeof(StringComparison) });

        public static readonly MethodInfo EndsWith = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });
        public static readonly MethodInfo EndsWithStringComparison = typeof(string).GetMethod("EndsWith", new[] { typeof(string), typeof(StringComparison) });
    }
}