using AllOverIt.Tests.Patterns.Specification.Dummies;
using FluentAssertions;
using Xunit;

namespace AllOverIt.Tests.Patterns.Specification
{
    public class SpecificationFixture : SpecificationFixtureBase
    {
        private readonly bool _negate;
        private readonly SpecificationDummy _specification;

        public SpecificationFixture()
        {
            _negate = Create<bool>();
            _specification = new SpecificationDummy(_negate);
        }

        [Fact]
        public void Should_Pass_Candidate()
        {
            var expected = Create<int>();

            _specification.IsSatisfiedBy(expected);

            _specification.Candidate.Should().Be(expected);
        }

        [Fact]
        public void Should_Return_Expected_Result()
        {
            var actual = _specification.IsSatisfiedBy(Create<int>());

            // the dummy returns 'true', so the base class returns a value based on _negate
            actual.Should().Be(!_negate);
        }
    }
}