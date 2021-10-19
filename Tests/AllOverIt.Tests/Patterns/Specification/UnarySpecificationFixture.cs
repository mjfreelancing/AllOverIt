using AllOverIt.Fixture.Extensions;
using AllOverIt.Tests.Patterns.Specification.Dummies;
using FluentAssertions;
using System;
using Xunit;

namespace AllOverIt.Tests.Patterns.Specification
{
    public class UnarySpecificationFixture : SpecificationFixtureBase
    {
        private readonly bool _negate;
        private readonly UnarySpecificationDummy _specification;

        public UnarySpecificationFixture()
        {
            _negate = Create<bool>();
            _specification = new UnarySpecificationDummy(IsEven, _negate);
        }

        [Fact]
        public void Should_Throw_When_Null_Specification()
        {
            Invoking(() =>
                {
                    _ = new UnarySpecificationDummy(null, _negate);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("specification");
        }

        [Fact]
        public void Should_Set_Specification_Member()
        {
            _specification.Spec.Should().BeSameAs(IsEven);
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