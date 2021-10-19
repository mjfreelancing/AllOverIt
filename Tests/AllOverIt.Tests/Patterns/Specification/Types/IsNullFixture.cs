using AllOverIt.Patterns.Specification.Types;
using AllOverIt.Tests.Patterns.Specification.Dummies;
using FluentAssertions;
using Xunit;

namespace AllOverIt.Tests.Patterns.Specification.Types
{
    public class IsNullFixture : SpecificationFixtureBase
    {
        private readonly IsNullSpecification<TypeDummy> _specification;

        public IsNullFixture()
        {
            _specification = new IsNullSpecification<TypeDummy>();
        }

        [Fact]
        public void Should_Return_True_When_Null()
        {
            var result = _specification.IsSatisfiedBy(null);

            result.Should().BeTrue();
        }

        [Fact]
        public void Should_Return_False_When_Not_Null()
        {
            var result = _specification.IsSatisfiedBy(Create<TypeDummy>());

            result.Should().BeFalse();
        }
    }
}