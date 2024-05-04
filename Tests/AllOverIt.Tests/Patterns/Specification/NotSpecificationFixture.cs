using AllOverIt.Patterns.Specification.Extensions;
using FluentAssertions;

namespace AllOverIt.Tests.Patterns.Specification
{
    public class NotSpecificationFixture : SpecificationFixtureBase
    {
        [Theory]
        [InlineData(-2, false)]
        [InlineData(2, false)]
        [InlineData(-3, true)]
        [InlineData(3, true)]
        public void Should_Return_Expected_Result(int value, bool expected)
        {
            var combined = IsEven.Not();

            var actual = combined.IsSatisfiedBy(value);

            actual.Should().Be(expected);
        }
    }
}