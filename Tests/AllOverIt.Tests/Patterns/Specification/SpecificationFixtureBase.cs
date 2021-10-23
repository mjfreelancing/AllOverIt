using AllOverIt.Fixture;
using AllOverIt.Patterns.Specification;
using AllOverIt.Tests.Patterns.Specification.Dummies;

namespace AllOverIt.Tests.Patterns.Specification
{
    public class SpecificationFixtureBase : FixtureBase
    {
        protected readonly ISpecification<int> IsEven;
        protected readonly ISpecification<int> IsPositive;

        protected SpecificationFixtureBase()
        {
            IsEven = new IsEven();
            IsPositive = new IsPositive();
        }
    }
}