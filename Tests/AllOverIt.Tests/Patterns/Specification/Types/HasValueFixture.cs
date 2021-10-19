using AllOverIt.Patterns.Specification.Types;
using FluentAssertions;
using Xunit;

namespace AllOverIt.Tests.Patterns.Specification.Types
{
    public class HasValueFixture : SpecificationFixtureBase
    {
        private readonly HasValue<double> _specification;

        public HasValueFixture()
        {
            _specification = new HasValue<double>();
        }

        [Theory]
        [InlineData(0.0d, true)]
        [InlineData(null, false)]
        public void Should_Return_Expected_result(double? value, bool expected)
        {
            var actual = _specification.IsSatisfiedBy(value);

            actual.Should().Be(expected);
        }
    }
}