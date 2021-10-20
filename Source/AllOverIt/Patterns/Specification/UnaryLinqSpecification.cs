using AllOverIt.Helpers;

namespace AllOverIt.Patterns.Specification
{
    public abstract class UnaryLinqSpecification<TType> : LinqSpecification<TType>
    {
        protected ILinqSpecification<TType> Specification { get; }

        protected UnaryLinqSpecification(ILinqSpecification<TType> specification, bool negate = false)
            : base(negate)
        {
            Specification = specification.WhenNotNull(nameof(specification));
        }
    }
}