using AllOverIt.Patterns.Specification;

namespace AllOverIt.Tests.Patterns.Specification.Dummies
{
    internal sealed class IsEven : Specification<int>
    {
        public override bool IsSatisfiedBy(int candidate)
        {
            return candidate % 2 == 0;
        }
    }
}