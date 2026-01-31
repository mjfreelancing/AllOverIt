using AllOverIt.Patterns.Specification;
using AllOverIt.Patterns.Specification.Extensions;
using FluentAssertions;

namespace AllOverIt.Tests.Patterns.Specification
{
    public class OrSpecificationFixture : SpecificationFixtureBase
    {
        [Theory]
        [InlineData(-2, true)]
        [InlineData(2, true)]
        [InlineData(-3, false)]
        [InlineData(3, true)]
        public void Should_Return_Expected_Result(int value, bool expected)
        {
            var combined = IsEven.Or(IsPositive);

            var actual = combined.IsSatisfiedBy(value);

            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(-2, true)]
        [InlineData(2, true)]
        [InlineData(-3, false)]
        [InlineData(3, true)]
        [InlineData(4, true)]
        [InlineData(-4, true)]
        [InlineData(5, true)]
        [InlineData(-5, false)]
        public void Should_Return_Expected_Result_Using_Operator(int value, bool expected)
        {
            var combined = IsEven || IsPositive;

            var actual = combined.IsSatisfiedBy(value);

            actual.Should().Be(expected);
        }
    }
}