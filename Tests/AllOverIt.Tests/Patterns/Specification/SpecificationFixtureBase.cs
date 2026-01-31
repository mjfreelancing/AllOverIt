using AllOverIt.Fixture;
using AllOverIt.Patterns.Specification;
using AllOverIt.Tests.Patterns.Specification.Dummies;

namespace AllOverIt.Tests.Patterns.Specification
{
    public class SpecificationFixtureBase : FixtureBase
    {
        protected readonly Specification<int> IsEven;
        protected readonly Specification<int> IsPositive;

        protected SpecificationFixtureBase()
        {
            IsEven = new IsEven();
            IsPositive = new IsPositive();
        }
    }
}