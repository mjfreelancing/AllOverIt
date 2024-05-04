using AllOverIt.Patterns.Specification.Extensions;
using FluentAssertions;

namespace AllOverIt.Tests.Patterns.Specification
{
    public class AndNotLinqSpecificationFixture : LinqSpecificationFixtureBase
    {
        [Theory]
        [InlineData(-2, true)]
        [InlineData(2, false)]
        [InlineData(-3, false)]
        [InlineData(3, false)]
        public void Should_Return_Expected_Result(int value, bool expected)
        {
            var combined = LinqIsEven.AndNot(LinqIsPositive);

            var actual = combined.IsSatisfiedBy(value);

            actual.Should().Be(expected);
        }
    }
}