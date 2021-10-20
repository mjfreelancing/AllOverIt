using AllOverIt.Helpers;
using System;

namespace AllOverIt.Patterns.Specification
{
    // An abstract specification class that determines if an object meets the criteria defined by the specification.
    // TType is the candidate type to be tested.
    public abstract class Specification<TType> : SpecificationBase<TType>, ISpecification<TType>
    {
        protected Specification(bool negate = false)
            : base(negate)
        {
        }

        public static implicit operator Func<TType, bool>(Specification<TType> specification)
        {
            _ = specification.WhenNotNull(nameof(specification));

            return specification.IsSatisfiedBy;
        }

        public static Specification<TType> operator &(Specification<TType> leftSpecification, Specification<TType> rightSpecification)
        {
            return new AndSpecification<TType>(leftSpecification, rightSpecification);
        }

        public static Specification<TType> operator |(Specification<TType> leftSpecification, Specification<TType> rightSpecification)
        {
            return new OrSpecification<TType>(leftSpecification, rightSpecification);
        }

        public static Specification<TType> operator !(Specification<TType> specification)
        {
            return new NotSpecification<TType>(specification);
        }
    }
}