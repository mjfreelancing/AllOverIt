using AllOverIt.Patterns.Specification;

namespace AllOverIt.Tests.Patterns.Specification.Dummies
{
    internal sealed class IsEven : Specification<int>
    {
        protected override bool DoIsSatisfiedBy(int candidate)
        {
            return candidate % 2 == 0;
        }
    }
}