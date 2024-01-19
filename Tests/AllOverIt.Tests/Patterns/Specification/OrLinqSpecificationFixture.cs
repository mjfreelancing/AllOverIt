using AllOverIt.Patterns.Specification;
using AllOverIt.Patterns.Specification.Extensions;
using FluentAssertions;
using Xunit;

namespace AllOverIt.Tests.Patterns.Specification
{
    public class OrLinqSpecificationFixture : LinqSpecificationFixtureBase
    {
        [Theory]
        [InlineData(-2, true)]
        [InlineData(2, true)]
        [InlineData(-3, false)]
        [InlineData(3, true)]
        public void Should_Return_Expected_Result(int value, bool expected)
        {
            var combined = LinqIsEven.Or(LinqIsPositive);

            var actual = combined.IsSatisfiedBy(value);

            actual.Should().Be(expected);
        }
    }
}