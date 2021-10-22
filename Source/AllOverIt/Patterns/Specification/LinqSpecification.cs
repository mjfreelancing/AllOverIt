using AllOverIt.Helpers;
using System;
using System.Linq.Expressions;

namespace AllOverIt.Patterns.Specification
{
    /// <inheritdoc cref="SpecificationBase{TType}"/>
    public abstract class LinqSpecification<TType> : SpecificationBase<TType>, ILinqSpecification<TType>
    {
        private Func<TType, bool> _compiled;

        /// <summary>Constructor.</summary>
        /// <param name="negate">Indicates if the result of the specification should be negated (invert true/false results).</param>
        protected LinqSpecification(bool negate = false)
            : base(negate)
        {
        }

        /// <inheritdoc />
        public abstract Expression<Func<TType, bool>> AsExpression();

        /// <summary>An implicit operator to return the specification as an Expression&lt;Func&lt;TType, bool&gt;&gt; so it can be used with
        /// <see cref="System.Linq.IQueryable{T}"/> based LINQ queries.</summary>
        /// <param name="specification">The specification to be returned as an Expression&lt;Func&lt;TType, bool&gt;&gt;.</param>
        public static implicit operator Expression<Func<TType, bool>>(LinqSpecification<TType> specification)
        {
            _ = specification.WhenNotNull(nameof(specification));

            return specification.AsExpression();
        }

        /// <summary>An explicit operator to return the specification as a Func&lt;TType, bool&gt; so it can be used with
        /// <see cref="System.Collections.Generic.IEnumerable{T}"/> based LINQ queries.</summary>
        /// <param name="specification">The specification to be returned as a Func&lt;TType, bool&gt;.</param>
        public static explicit operator Func<TType, bool>(LinqSpecification<TType> specification)
        {
            _ = specification.WhenNotNull(nameof(specification));

            return specification.GetCompiledExpression();
        }

        /// <summary>Provides support for operator &amp;&amp;.</summary>
        /// <param name="leftSpecification">The left operand applied to the operator.</param>
        /// <param name="rightSpecification">The right operand applied to the operator.</param>
        /// <returns>A new specification that AND's the provided specifications.</returns>
        public static LinqSpecification<TType> operator &(LinqSpecification<TType> leftSpecification, LinqSpecification<TType> rightSpecification)
        {
            return new AndLinqSpecification<TType>(leftSpecification, rightSpecification);
        }

        /// <summary>Provides support for operator ||.</summary>
        /// <param name="leftSpecification">The left operand applied to the operator.</param>
        /// <param name="rightSpecification">The right operand applied to the operator.</param>
        /// <returns>A new specification that OR's the provided specifications.</returns>
        public static LinqSpecification<TType> operator |(LinqSpecification<TType> leftSpecification, LinqSpecification<TType> rightSpecification)
        {
            return new OrLinqSpecification<TType>(leftSpecification, rightSpecification);
        }

        /// <summary>Provides support for operator ! (Not).</summary>
        /// <param name="specification">The operand applied to the operator.</param>
        /// <returns>A new specification that inverts the result of the provided specification.</returns>
        public static LinqSpecification<TType> operator !(LinqSpecification<TType> specification)
        {
            return new NotLinqSpecification<TType>(specification);
        }

        /// <inheritdoc />
        protected sealed override bool DoIsSatisfiedBy(TType candidate)
        {
            return GetCompiledExpression().Invoke(candidate);
        }

        private Func<TType, bool> GetCompiledExpression()
        {
            _compiled ??= AsExpression().Compile();
            return _compiled;
        }
    }
}