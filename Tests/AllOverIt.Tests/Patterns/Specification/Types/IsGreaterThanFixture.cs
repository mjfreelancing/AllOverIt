using AllOverIt.Patterns.Specification.Types;
using FluentAssertions;
using Xunit;

namespace AllOverIt.Tests.Patterns.Specification.Types
{
    public class IsGreaterThanFixture : SpecificationFixtureBase
    {
        private readonly IsGreaterThan<int> _specification;
        private readonly int _value;

        public IsGreaterThanFixture()
        {
            _value = Create<int>();
            _specification = new IsGreaterThan<int>(_value);
        }

        [Fact]
        public void Should_Compare_Greater_Than()
        {
            var result = _specification.IsSatisfiedBy(_value + Create<int>());

            result.Should().BeTrue();
        }

        [Fact]
        public void Should_Not_Compare_Greater_Than()
        {
            var result = _specification.IsSatisfiedBy(_value - Create<int>());

            result.Should().BeFalse();
        }

        [Fact]
        public void Should_Not_Compare_Equal()
        {
            var result = _specification.IsSatisfiedBy(_value);

            result.Should().BeFalse();
        }
    }
}