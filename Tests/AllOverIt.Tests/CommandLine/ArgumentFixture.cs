using AllOverIt.CommandLine;
using AllOverIt.Fixture;
using AllOverIt.Tests.Extensions;
using FluentAssertions;
using Xunit;

namespace AllOverIt.Tests.CommandLine
{
    public class ArgumentFixture : FixtureBase
    {
        public class Escape : StringExtensionsFixture
        {
            [Theory]
            [InlineData("", "\"\"")]
            [InlineData("a", "a")]
            [InlineData("a ", "\"a \"")]
            [InlineData(" a", "\" a\"")]
            [InlineData(" a ", "\" a \"")]
            [InlineData("a a", "\"a a\"")]
            [InlineData("\"", "\"\\\"\"")]                      // Quote with no preceding backslash
            [InlineData("\"a", "\"\\\"a\"")]
            [InlineData("a\"", "\"a\\\"\"")]
            [InlineData("\"a\"", "\"\\\"a\\\"\"")]
            [InlineData("\\a", "\\a")]
            [InlineData("\\ a", "\"\\ a\"")]
            [InlineData("\\ a \\", "\"\\ a \\\\\"")]
            [InlineData("\\\\a", "\\\\a")]
            [InlineData("\\\\a\\\\", "\\\\a\\\\")]
            [InlineData("\\\\ a \\\\", "\"\\\\ a \\\\\\\\\"")]
            [InlineData("\\\"", "\"\\\\\\\"\"")]                // backslash before quote
            public void Should_Escape_Value(string value, string expected)
            {
                var actual = Argument.Escape(value);

                actual.Should().Be(expected);
            }
        }
    }
}
