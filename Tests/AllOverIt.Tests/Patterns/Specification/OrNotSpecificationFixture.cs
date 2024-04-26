using AllOverIt.Patterns.Specification.Extensions;
using FluentAssertions;

namespace AllOverIt.Tests.Patterns.Specification
{
    public class OrNotSpecificationFixture : SpecificationFixtureBase
    {
        [Theory]
        [InlineData(-2, true)]
        [InlineData(2, true)]
        [InlineData(-3, true)]
        [InlineData(3, false)]
        public void Should_Return_Expected_Result(int value, bool expected)
        {
            var combined = IsEven.OrNot(IsPositive);

            var actual = combined.IsSatisfiedBy(value);

            actual.Should().Be(expected);
        }
    }
}