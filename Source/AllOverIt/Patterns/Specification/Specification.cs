using AllOverIt.Assertion;
using System;

namespace AllOverIt.Patterns.Specification
{
    /// <inheritdoc cref="SpecificationBase{TType}"/>
    public abstract class Specification<TType> : SpecificationBase<TType>, ISpecification<TType>
    {
        private sealed class AdHocSpecification : Specification<TType>
        {
            private readonly Func<TType, bool> _predicate;

            public AdHocSpecification(Func<TType, bool> predicate)
            {
                _predicate = predicate.WhenNotNull(nameof(predicate));
            }

            public override bool IsSatisfiedBy(TType candidate)
            {
                return _predicate.Invoke(candidate);
            }
        }

        /// <summary>Creates an ad-hoc specification based on the provided predicate.</summary>
        /// <param name="predicate">The predicate to be used by the specification.</param>
        /// <returns>An ad-hoc specification based on the provided predicate.</returns>
        public static ISpecification<TType> Create(Func<TType, bool> predicate)
        {
            return new AdHocSpecification(predicate);
        }

        /// <summary>Provides support for operator &amp;&amp;.</summary>
        /// <param name="leftSpecification">The left operand applied to the operator.</param>
        /// <param name="rightSpecification">The right operand applied to the operator.</param>
        /// <returns>A new specification that performs a logical AND operation between two expressions.</returns>
        public static Specification<TType> operator &(Specification<TType> leftSpecification, Specification<TType> rightSpecification)
        {
            return new AndSpecification<TType>(leftSpecification, rightSpecification);
        }

        /// <summary>Provides support for operator ||.</summary>
        /// <param name="leftSpecification">The left operand applied to the operator.</param>
        /// <param name="rightSpecification">The right operand applied to the operator.</param>
        /// <returns>A new specification that performs a logical OR operation between two expressions.</returns>
        public static Specification<TType> operator |(Specification<TType> leftSpecification, Specification<TType> rightSpecification)
        {
            return new OrSpecification<TType>(leftSpecification, rightSpecification);
        }

        /// <summary>Provides support for operator ! (Not).</summary>
        /// <param name="specification">The operand applied to the operator.</param>
        /// <returns>A new specification that that performs a logical NOT operation between two expressions.</returns>
        public static Specification<TType> operator !(Specification<TType> specification)
        {
            return new NotSpecification<TType>(specification);
        }
    }
}