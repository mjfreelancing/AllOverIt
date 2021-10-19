using AllOverIt.Fixture.Extensions;
using AllOverIt.Patterns.Specification.Types;
using AllOverIt.Tests.Patterns.Specification.Dummies;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace AllOverIt.Tests.Patterns.Specification.Types
{
    public class IsNotEmptyFixture : SpecificationFixtureBase
    {
        private readonly IsNotEmpty<TypeDummy> _specification;

        public IsNotEmptyFixture()
        {
            _specification = new IsNotEmpty<TypeDummy>();
        }

        [Fact]
        public void Should_Throw_When_Null()
        {
            Invoking(() =>
                {
                    _ = _specification.IsSatisfiedBy(null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("candidate");
        }

        [Fact]
        public void Should_Return_False_When_Empty()
        {
            var result = _specification.IsSatisfiedBy(Enumerable.Empty<TypeDummy>());

            result.Should().BeFalse();
        }

        [Fact]
        public void Should_Return_True_When_Not_Empty()
        {
            var result = _specification.IsSatisfiedBy(CreateMany<TypeDummy>());

            result.Should().BeTrue();
        }
    }
}