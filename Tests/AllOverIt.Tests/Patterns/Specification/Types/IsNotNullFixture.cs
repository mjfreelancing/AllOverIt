using AllOverIt.Patterns.Specification.Types;
using AllOverIt.Tests.Patterns.Specification.Dummies;
using FluentAssertions;
using Xunit;

namespace AllOverIt.Tests.Patterns.Specification.Types
{
    public class IsNotNullFixture : SpecificationFixtureBase
    {
        private readonly IsNotNull<TypeDummy> _specification;

        public IsNotNullFixture()
        {
            _specification = new IsNotNull<TypeDummy>();
        }

        [Fact]
        public void Should_Return_False_When_Null()
        {
            var result = _specification.IsSatisfiedBy(null);

            result.Should().BeFalse();
        }

        [Fact]
        public void Should_Return_True_When_Not_Null()
        {
            var result = _specification.IsSatisfiedBy(Create<TypeDummy>());

            result.Should().BeTrue();
        }
    }
}