using System;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Tests.Patterns.Specification.Dummies;
using FluentAssertions;
using Xunit;

namespace AllOverIt.Tests.Patterns.Specification
{
    public class BinaryLinqSpecificationFixture : LinqSpecificationFixtureBase
    {
        private readonly bool _negate;
        private readonly BinaryLinqSpecificationDummy _specification;

        public BinaryLinqSpecificationFixture()
        {
            _negate = Create<bool>();
            _specification = new BinaryLinqSpecificationDummy(LinqIsEven, LinqIsPositive, _negate);
        }

        [Fact]
        public void Should_Throw_When_Null_Left_Specification()
        {
            Invoking(() =>
                {
                    _ = new BinaryLinqSpecificationDummy(null, LinqIsPositive, _negate);
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
                    _ = new BinaryLinqSpecificationDummy(LinqIsEven, null, _negate);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("rightSpecification");
        }

        [Fact]
        public void Should_Set_Specification_Members()
        {
            _specification.Left.Should().BeSameAs(LinqIsEven);
            _specification.Right.Should().BeSameAs(LinqIsPositive);
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