using AllOverIt.Fixture.Extensions;
using AllOverIt.Tests.Patterns.Specification.Dummies;
using FluentAssertions;
using System;
using Xunit;

namespace AllOverIt.Tests.Patterns.Specification
{
    public class BinarySpecificationFixture : SpecificationFixtureBase
    {
        private readonly bool _negate;
        private readonly BinarySpecificationDummy _specification;

        public BinarySpecificationFixture()
        {
            _negate = Create<bool>();
            _specification = new BinarySpecificationDummy(IsEven, IsPositive, _negate);
        }

        [Fact]
        public void Should_Throw_When_Null_Left_Specification()
        {
            Invoking(() =>
                {
                    _ = new BinarySpecificationDummy(null, IsPositive, _negate);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("leftSpecification");
        }

        [Fact]
        public void Should_Throw_When_Null_Right_Specification()
        {
            Invoking(() =>
                {
                    _ = new BinarySpecificationDummy(IsEven, null, _negate);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("rightSpecification");
        }

        [Fact]
        public void Should_Set_Specification_Members()
        {
            _specification.Left.Should().BeSameAs(IsEven);
            _specification.Right.Should().BeSameAs(IsPositive);
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