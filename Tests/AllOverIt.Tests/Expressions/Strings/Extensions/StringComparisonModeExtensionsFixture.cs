using AllOverIt.Expressions.Strings;
using AllOverIt.Expressions.Strings.Extensions;
using AllOverIt.Fixture;
using FluentAssertions;

namespace AllOverIt.Tests.Expressions.Strings.Extensions
{
    public class StringComparisonModeExtensionsFixture : FixtureBase
    {
        public class GetStringComparison : StringComparisonModeExtensionsFixture
        {
            [Theory]
            [MemberData(nameof(StringComparisonOptions))]
            public void Should_Get_StringComparison(StringComparisonMode stringComparisonMode, StringComparison stringComparison)
            {
                StringComparisonModeExtensions
                    .GetStringComparison(stringComparisonMode)
                    .Should()
                    .Be(stringComparison);
            }

            [Theory]
            [MemberData(nameof(NonStringComparisons))]
            public void Should_Throw_When_Not_StringComparison(StringComparisonMode stringComparisonMode)
            {
                Invoking(() =>
                {
                    _ = StringComparisonModeExtensions.GetStringComparison(stringComparisonMode);

                })
                    .Should()
                    .Throw<InvalidOperationException>()
                    .WithMessage($"The string comparison mode '{stringComparisonMode}' cannot be converted to a {nameof(StringComparison)} value.");
            }
        }

        public class IsStringComparison : StringComparisonModeExtensionsFixture
        {
            [Theory]
            [MemberData(nameof(StringComparisonModes))]
            public void Should_Get_IsComparison(StringComparisonMode stringComparisonMode, bool isComparison, bool isModifier)
            {
                StringComparisonModeExtensions
                    .IsStringComparison(stringComparisonMode)
                    .Should()
                    .Be(isComparison);
            }
        }

        public class IsStringModifier : StringComparisonModeExtensionsFixture
        {
            [Theory]
            [MemberData(nameof(StringComparisonModes))]
            public void Should_Get_IsModifier(StringComparisonMode stringComparisonMode, bool isComparison, bool isModifier)
            {
                StringComparisonModeExtensions
                  .IsStringModifier(stringComparisonMode)
                  .Should()
                  .Be(isModifier);
            }
        }

        protected static IEnumerable<object[]> NonStringComparisons()
        {
            return new List<object[]>
            {
                new object[] { StringComparisonMode.None },
                new object[] { StringComparisonMode.ToLower },
                new object[] { StringComparisonMode.ToUpper }
            };
        }

        protected static IEnumerable<object[]> StringComparisonModes()
        {
            return new List<object[]>
            {
                // mode, is comparison, is modifier
                new object[] { StringComparisonMode.None, false, false },
                new object[] { StringComparisonMode.ToLower, false, true },
                new object[] { StringComparisonMode.ToUpper, false, true },
                new object[] { StringComparisonMode.CurrentCulture, true, false },
                new object[] { StringComparisonMode.CurrentCultureIgnoreCase, true, false },
                new object[] { StringComparisonMode.InvariantCulture, true, false },
                new object[] { StringComparisonMode.InvariantCultureIgnoreCase, true, false },
                new object[] { StringComparisonMode.Ordinal, true, false },
                new object[] { StringComparisonMode.OrdinalIgnoreCase, true, false }
            };
        }

        protected static IEnumerable<object[]> StringComparisonOptions()
        {
            return new List<object[]>
            {
                new object[] { StringComparisonMode.CurrentCulture, StringComparison.CurrentCulture },
                new object[] { StringComparisonMode.CurrentCultureIgnoreCase, StringComparison.CurrentCultureIgnoreCase },
                new object[] { StringComparisonMode.InvariantCulture, StringComparison.InvariantCulture },
                new object[] { StringComparisonMode.InvariantCultureIgnoreCase, StringComparison.InvariantCultureIgnoreCase },
                new object[] { StringComparisonMode.Ordinal, StringComparison.Ordinal },
                new object[] { StringComparisonMode.OrdinalIgnoreCase, StringComparison.OrdinalIgnoreCase }
            };
        }
    }
}