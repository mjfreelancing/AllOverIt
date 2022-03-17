using AllOverIt.Patterns.Specification;

namespace AllOverIt.Tests.Patterns.Specification.Dummies
{
    internal sealed class LinqIsPositive : LinqSpecification<int>
    {
        public LinqIsPositive()
            : base(() => candidate => candidate > 0)
        {
        }
    }
}