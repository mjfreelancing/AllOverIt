using System;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Tests.Patterns.Specification.Dummies;
using FluentAssertions;
using Xunit;

namespace AllOverIt.Tests.Patterns.Specification
{
    public class UnaryLinqSpecificationFixture : LinqSpecificationFixtureBase
    {
        private readonly bool _negate;
        private readonly UnaryLinqSpecificationDummy _specification;

        public UnaryLinqSpecificationFixture()
        {
            _negate = Create<bool>();
            _specification = new UnaryLinqSpecificationDummy(LinqIsEven, _negate);
        }

        [Fact]
        public void Should_Throw_When_Null_Specification()
        {
            Invoking(() =>
                {
                    _ = new UnaryLinqSpecificationDummy(null, _negate);
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

            // the dummy returns 'true', so the base class returns a value based on _negate
            actual.Should().Be(!_negate);
        }
    }
}