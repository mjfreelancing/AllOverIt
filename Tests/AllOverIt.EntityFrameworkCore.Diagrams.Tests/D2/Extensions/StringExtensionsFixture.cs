using AllOverIt.EntityFrameworkCore.Diagrams.D2.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;

namespace AllOverIt.EntityFrameworkCore.Diagrams.Tests.D2.Extensions
{
    public class StringExtensionsFixture : FixtureBase
    {
        public class D2EscapeString : StringExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_String_Null()
            {
                Invoking(() =>
                {
                    _ = StringExtensions.D2EscapeString(null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("value");
            }


            [Fact]
            public void Should_Not_Throw_When_String_Empty()
            {
                Invoking(() =>
                {
                    _ = StringExtensions.D2EscapeString(string.Empty);
                })
                .Should()
                .NotThrow();
            }

            [Theory]
            [InlineData("#")]
            [InlineData("[")]
            [InlineData("]")]
            public void Should_Escape_String(string input)
            {
                var str1 = Create<string>();
                var str2 = Create<string>();

                var value = $"{str1}{input}{str2}";
                var expected = $"{str1}\\{input}{str2}";

                var actual = StringExtensions.D2EscapeString(value);

                actual.Should().Be(expected);
            }
        }
    }
}
