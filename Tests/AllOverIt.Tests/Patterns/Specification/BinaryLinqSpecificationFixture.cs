using AllOverIt.Fixture.Extensions;
using AllOverIt.Tests.Patterns.Specification.Dummies;
using FluentAssertions;
using System;
using Xunit;

namespace AllOverIt.Tests.Patterns.Specification
{
    public class BinaryLinqSpecificationFixture : LinqSpecificationFixtureBase
    {
        private readonly bool _expectedResult;
        private readonly BinaryLinqSpecificationDummy _specification;

        public BinaryLinqSpecificationFixture()
        {
            _expectedResult = Create<bool>();
            _specification = new BinaryLinqSpecificationDummy(LinqIsEven, LinqIsPositive, _expectedResult);
        }

        [Fact]
        public void Should_Throw_When_Null_Left_Specification()
        {
            Invoking(() =>
                {
                    _ = new BinaryLinqSpecificationDummy(null, LinqIsPositive, _expectedResult);
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
                    _ = new BinaryLinqSpecificationDummy(LinqIsEven, null, _expectedResult);
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

            actual.Should().Be(_expectedResult);
        }
    }
}