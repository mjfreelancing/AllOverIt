using System;
using System.Collections.Generic;

namespace AllOverIt.Expressions.Strings.Extensions
{
    internal static class StringComparisonModeExtensions
    {
        private static readonly Dictionary<StringComparisonMode, StringComparison> ComparisonModes = new()
        {
            { StringComparisonMode.CurrentCulture, StringComparison.CurrentCulture},
            { StringComparisonMode.CurrentCultureIgnoreCase, StringComparison.CurrentCultureIgnoreCase},
            { StringComparisonMode.InvariantCulture, StringComparison.InvariantCulture},
            { StringComparisonMode.InvariantCultureIgnoreCase, StringComparison.InvariantCultureIgnoreCase},
            { StringComparisonMode.Ordinal, StringComparison.Ordinal},
            { StringComparisonMode.OrdinalIgnoreCase, StringComparison.OrdinalIgnoreCase}
        };

        public static StringComparison GetStringComparison(this StringComparisonMode stringComparisonMode)
        {
            if (ComparisonModes.TryGetValue(stringComparisonMode, out var stringComparison))
            {
                return stringComparison;
            }

            throw new InvalidOperationException($"The string comparison mode '{stringComparisonMode}' cannot be converted to a {nameof(StringComparison)} value.");
        }

        public static bool IsStringComparison(this StringComparisonMode stringComparisonMode)
        {
            return stringComparisonMode != StringComparisonMode.None &&
                   !stringComparisonMode.IsStringModifier();

        }

        public static bool IsStringModifier(this StringComparisonMode stringComparisonMode)
        {
            return stringComparisonMode == StringComparisonMode.ToLower ||
                   stringComparisonMode == StringComparisonMode.ToUpper;
        }
    }
}