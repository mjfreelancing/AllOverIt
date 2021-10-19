using AllOverIt.Patterns.Specification.Types;
using FluentAssertions;
using Xunit;

namespace AllOverIt.Tests.Patterns.Specification.Types
{
    public class IsNullOrWhitespaceFixture : SpecificationFixtureBase
    {
        private readonly IsNullOrWhitespace _specification;

        public IsNullOrWhitespaceFixture()
        {
            _specification = new IsNullOrWhitespace();
        }

        [Theory]
        [InlineData(null, true)]
        [InlineData("", true)]
        [InlineData("  ", true)]
        [InlineData("xyz", false)]
        public void Should_Return_Expected_Result(string value, bool expected)
        {
            var result = _specification.IsSatisfiedBy(value);

            result.Should().Be(expected);
        }
    }
}