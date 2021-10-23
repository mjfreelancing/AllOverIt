using AllOverIt.Patterns.Specification;

namespace AllOverIt.Tests.Patterns.Specification.Dummies
{
    internal sealed class IsPositive : Specification<int>
    {
        public override bool IsSatisfiedBy(int candidate)
        {
            return candidate > 0;
        }
    }
}