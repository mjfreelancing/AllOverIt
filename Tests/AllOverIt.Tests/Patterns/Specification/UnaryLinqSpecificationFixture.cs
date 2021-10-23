using AllOverIt.Fixture.Extensions;
using AllOverIt.Tests.Patterns.Specification.Dummies;
using FluentAssertions;
using System;
using Xunit;

namespace AllOverIt.Tests.Patterns.Specification
{
    public class UnaryLinqSpecificationFixture : LinqSpecificationFixtureBase
    {
        private readonly bool _expectedResult;
        private readonly UnaryLinqSpecificationDummy _specification;

        public UnaryLinqSpecificationFixture()
        {
            _expectedResult = Create<bool>();
            _specification = new UnaryLinqSpecificationDummy(LinqIsEven);
        }

        [Fact]
        public void Should_Throw_When_Null_Specification()
        {
            Invoking(() =>
                {
                    _ = new UnaryLinqSpecificationDummy(null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("specification");
        }

        [Fact]
        public void Should_Set_Specification_Member()
        {
            _specification.Spec.Should().BeSameAs(LinqIsEven);
        }

        [Fact]
        public void Should_Return_Expected_Result()
        {
            var actual = _specification.IsSatisfiedBy(Create<int>());

            actual.Should().Be(_expectedResult);
        }
    }
}