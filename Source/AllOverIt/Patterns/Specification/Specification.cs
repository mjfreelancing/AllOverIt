using AllOverIt.Helpers;
using System;

namespace AllOverIt.Patterns.Specification
{
    /// <inheritdoc cref="SpecificationBase{TType}"/>
    public abstract class Specification<TType> : SpecificationBase<TType>, ISpecification<TType>
    {
        /// <summary>Constructor.</summary>
        /// <param name="negate">Indicates if the result of the specification should be negated (invert true/false results).</param>
        protected Specification(bool negate = false)
            : base(negate)
        {
        }

        /// <summary>An implicit operator to return the specification as a Func&lt;TType, bool&gt; so it can be used with
        /// <see cref="System.Collections.Generic.IEnumerable{T}"/> based LINQ queries.</summary>
        /// <param name="specification">The specification to be returned as a Func&lt;TType, bool&gt;.</param>
        public static implicit operator Func<TType, bool>(Specification<TType> specification)
        {
            _ = specification.WhenNotNull(nameof(specification));

            return specification.IsSatisfiedBy;
        }

        /// <summary>Provides support for operator &amp;&amp;.</summary>
        /// <param name="leftSpecification">The left operand applied to the operator.</param>
        /// <param name="rightSpecification">The right operand applied to the operator.</param>
        /// <returns>A new specification that AND's the provided specifications.</returns>
        public static Specification<TType> operator &(Specification<TType> leftSpecification, Specification<TType> rightSpecification)
        {
            return new AndSpecification<TType>(leftSpecification, rightSpecification);
        }

        /// <summary>Provides support for operator ||.</summary>
        /// <param name="leftSpecification">The left operand applied to the operator.</param>
        /// <param name="rightSpecification">The right operand applied to the operator.</param>
        /// <returns>A new specification that OR's the provided specifications.</returns>
        public static Specification<TType> operator |(Specification<TType> leftSpecification, Specification<TType> rightSpecification)
        {
            return new OrSpecification<TType>(leftSpecification, rightSpecification);
        }

        /// <summary>Provides support for operator ! (Not).</summary>
        /// <param name="specification">The operand applied to the operator.</param>
        /// <returns>A new specification that inverts the result of the provided specification.</returns>
        public static Specification<TType> operator !(Specification<TType> specification)
        {
            return new NotSpecification<TType>(specification);
        }
    }
}