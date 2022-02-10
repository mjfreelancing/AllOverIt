using AllOverIt.Fixture;
using AllOverIt.Formatters.Objects;
using FluentAssertions;
using Xunit;

namespace AllOverIt.Tests.Formatters.Objects
{
    public class ObjectPropertyEnumerableOptionsFixture : FixtureBase
    {
        private readonly ObjectPropertyEnumerableOptions _options = new();

        [Fact]
        public void Should_Default_To_No_Collation()
        {
            _options.CollateValues.Should().BeFalse();
        }

        [Fact]
        public void Should_Default_To_Comma_Separator()
        {
            _options.Separator.Should().Be(", ");
        }
    }
}