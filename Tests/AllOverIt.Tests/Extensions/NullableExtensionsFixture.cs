using AllOverIt.Extensions;
using AllOverIt.Fixture;
using FluentAssertions;

namespace AllOverIt.Tests.Extensions
{
    public class NullableExtensionsFixture : FixtureBase
    {
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Should_Deconstruct(bool expected)
        {
            int? actual = null;

            if (expected)
            {
                actual = Create<int>();
            }

            var (hasValue, value) = actual;

            hasValue.Should().Be(expected);
            hasValue.Should().Be(actual.HasValue);
            value.Should().Be(expected ? actual.Value : default);
        }
    }
}
