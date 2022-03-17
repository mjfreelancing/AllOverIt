using AllOverIt.Patterns.Specification;

namespace AllOverIt.Tests.Patterns.Specification.Dummies
{
    internal sealed class LinqIsEven : LinqSpecification<int>
    {
        public LinqIsEven()
            : base(() => candidate => candidate % 2 == 0)
        {
        }
    }
}