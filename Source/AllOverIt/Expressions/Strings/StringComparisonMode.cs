using AllOverIt.Assertion;
using AllOverIt.Patterns.Enumeration;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace AllOverIt.Expressions.Strings
{
    /// <summary>Provides options that control how string comparisons will be performed.</summary>
    public sealed class StringComparisonMode : EnrichedEnum<StringComparisonMode>
    {
        /// <summary>Apply no modification to string values.</summary>
        public static readonly StringComparisonMode None = new(0);

        /// <summary>Will lowercase input values (in advance) and lowercase each data row so string comparisons become case-insensitive.</summary>
        public static readonly StringComparisonMode ToLower = new(1);

        /// <summary>Will uppercase input values (in advance) and uppercase each data row so string comparisons become case-insensitive.</summary>
        public static readonly StringComparisonMode ToUpper = new(2);

        /// <summary>Compare strings using culture-sensitive sort rules and the current culture.</summary>
        public static readonly StringComparisonMode CurrentCulture = new(3);

        /// <summary>Compare strings using culture-sensitive sort rules, the current culture, and ignoring the case of the strings being compared.</summary>
        public static readonly StringComparisonMode CurrentCultureIgnoreCase = new(4);

        /// <summary>Compare strings using culture-sensitive sort rules and the invariant culture.</summary>
        public static readonly StringComparisonMode InvariantCulture = new(5);

        /// <summary>Compare strings using culture-sensitive sort rules, the invariant culture, and ignoring the case of the strings being compared.</summary>
        public static readonly StringComparisonMode InvariantCultureIgnoreCase = new(6);

        /// <summary>Compare strings using ordinal (binary) sort rules.</summary>
        public static readonly StringComparisonMode Ordinal = new(7);

        /// <summary>Compare strings using ordinal (binary) sort rules and ignoring the case of the strings being compared.</summary>
        public static readonly StringComparisonMode OrdinalIgnoreCase = new(8);

        [ExcludeFromCodeCoverage]
        private StringComparisonMode()
        {
            // Required for some serialization scenarios
        }

        private StringComparisonMode(int value, [CallerMemberName] string name = "")
            : base(value, name.WhenNotNull())
        {
        }
    }
}