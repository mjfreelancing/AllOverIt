using AllOverIt.Fixture;
using AllOverIt.Patterns.Specification;
using AllOverIt.Tests.Patterns.Specification.Dummies;
using FluentAssertions;
using Xunit;

namespace AllOverIt.Tests.Patterns.Specification
{
    public class SpecificationFactoryFixture : FixtureBase
    {
        private readonly SpecificationFactory _specificationFactory;

        public SpecificationFactoryFixture()
        {
            _specificationFactory = new SpecificationFactory();
        }

        public class CreateSpecification : SpecificationFactoryFixture
        {
            [Fact]
            public void Should_Create_Specification()
            {
                var actual = _specificationFactory.CreateSpecification<IsPositive, int>();

                actual.Should().BeOfType<IsPositive>();
            }
        }
    }
}