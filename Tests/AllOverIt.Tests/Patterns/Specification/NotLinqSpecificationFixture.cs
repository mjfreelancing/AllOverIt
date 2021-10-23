using AllOverIt.Extensions;
using FluentAssertions;
using Xunit;

namespace AllOverIt.Tests.Patterns.Specification
{
    public class NotLinqSpecificationFixture : LinqSpecificationFixtureBase
    {
        [Theory]
        [InlineData(-2, false)]
        [InlineData(2, false)]
        [InlineData(-3, true)]
        [InlineData(3, true)]
        public void Should_Return_Expected_Result(int value, bool expected)
        {
            var combined = LinqIsEven.Not();

            var actual = combined.IsSatisfiedBy(value);

            actual.Should().Be(expected);
        }
    }
}