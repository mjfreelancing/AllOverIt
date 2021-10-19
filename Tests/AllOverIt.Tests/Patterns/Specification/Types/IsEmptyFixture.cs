using AllOverIt.Fixture.Extensions;
using AllOverIt.Patterns.Specification.Types;
using AllOverIt.Tests.Patterns.Specification.Dummies;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace AllOverIt.Tests.Patterns.Specification.Types
{
    public class IsEmptyFixture : SpecificationFixtureBase
    {
        private readonly IsEmpty<TypeDummy> _specification;

        public IsEmptyFixture()
        {
            _specification = new IsEmpty<TypeDummy>();
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
        public void Should_Return_True_When_Empty()
        {
            var result = _specification.IsSatisfiedBy(Enumerable.Empty<TypeDummy>());

            result.Should().BeTrue();
        }

        [Fact]
        public void Should_Return_False_When_Not_Empty()
        {
            var result = _specification.IsSatisfiedBy(CreateMany<TypeDummy>());

            result.Should().BeFalse();
        }
    }
}