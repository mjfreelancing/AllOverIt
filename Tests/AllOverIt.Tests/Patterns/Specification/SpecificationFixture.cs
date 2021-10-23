using AllOverIt.Tests.Patterns.Specification.Dummies;
using FluentAssertions;
using Xunit;

namespace AllOverIt.Tests.Patterns.Specification
{
    public class SpecificationFixture : SpecificationFixtureBase
    {
        private readonly bool _expectedResult;
        private readonly SpecificationDummy _specification;

        public SpecificationFixture()
        {
            _expectedResult = Create<bool>();
            _specification = new SpecificationDummy(_expectedResult);
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

            actual.Should().Be(_expectedResult);
        }
    }
}