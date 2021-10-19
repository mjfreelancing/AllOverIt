using AllOverIt.Patterns.Specification.Types;
using FluentAssertions;
using Xunit;

namespace AllOverIt.Tests.Patterns.Specification.Types
{
    public class IsEqualFixture : SpecificationFixtureBase
    {
        private readonly IsEqual<int> _specification;
        private readonly int _value;

        public IsEqualFixture()
        {
            _value = Create<int>();
            _specification = new IsEqual<int>(_value);
        }

        [Fact]
        public void Should_Compare_Equal()
        {
            var result = _specification.IsSatisfiedBy(_value);

            result.Should().BeTrue();
        }

        [Fact]
        public void Should_Not_Compare_Equal()
        {
            var result = _specification.IsSatisfiedBy(_value + Create<int>());

            result.Should().BeFalse();
        }
    }
}