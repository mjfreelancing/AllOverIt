using AllOverIt.Patterns.Specification.Types;
using FluentAssertions;
using Xunit;

namespace AllOverIt.Tests.Patterns.Specification.Types
{
    public class IsNotNullOrWhitespaceFixture : SpecificationFixtureBase
    {
        private readonly IsNotNullOrWhitespace _specification;

        public IsNotNullOrWhitespaceFixture()
        {
            _specification = new IsNotNullOrWhitespace();
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("  ", false)]
        [InlineData("xyz", true)]
        public void Should_Return_Expected_Result(string value, bool expected)
        {
            var result = _specification.IsSatisfiedBy(value);

            result.Should().Be(expected);
        }
    }
}