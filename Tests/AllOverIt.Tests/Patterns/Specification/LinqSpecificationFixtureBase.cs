using AllOverIt.Fixture;
using AllOverIt.Patterns.Specification;
using AllOverIt.Tests.Patterns.Specification.Dummies;

namespace AllOverIt.Tests.Patterns.Specification
{
    public class LinqSpecificationFixtureBase : FixtureBase
    {
        protected readonly LinqSpecification<int> LinqIsEven;
        protected readonly LinqSpecification<int> LinqIsPositive;

        public LinqSpecificationFixtureBase()
        {
            LinqIsEven = new LinqIsEven();
            LinqIsPositive = new LinqIsPositive();
        }
    }
}