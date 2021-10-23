using AllOverIt.Fixture;
using AllOverIt.Patterns.Specification;
using AllOverIt.Tests.Patterns.Specification.Dummies;

namespace AllOverIt.Tests.Patterns.Specification
{
    public class LinqSpecificationFixtureBase : FixtureBase
    {
        protected readonly ILinqSpecification<int> LinqIsEven;
        protected readonly ILinqSpecification<int> LinqIsPositive;

        public LinqSpecificationFixtureBase()
        {
            LinqIsEven = new LinqIsEven();
            LinqIsPositive = new LinqIsPositive();
        }
    }
}