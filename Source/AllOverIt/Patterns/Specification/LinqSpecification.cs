using AllOverIt.Assertion;
using System;
using System.Linq.Expressions;

namespace AllOverIt.Patterns.Specification
{
    /// <inheritdoc cref="SpecificationBase{TType}"/>
    public abstract class LinqSpecification<TType> : SpecificationBase<TType>, ILinqSpecification<TType>
    {
        private sealed class AdHocSpecification : LinqSpecification<TType>
        {
            public AdHocSpecification(Expression<Func<TType, bool>> predicateExpression)
                : base(() => predicateExpression)
            {
            }
        }

        private Func<TType, bool> _compiled;

        /// <summary>Creates an ad-hoc specification based on the provided predicate.</summary>
        /// <param name="predicate">The predicate to be used by the specification.</param>
        /// <returns>An ad-hoc specification based on the provided predicate.</returns>
        public static ILinqSpecification<TType> Create(Expression<Func<TType, bool>> predicate)
        {
            return new AdHocSpecification(predicate);
        }

        /// <inheritdoc />
        public Expression<Func<TType, bool>> Expression { get; }

        /// <summary>Constructor.</summary>
        /// <param name="expressionResolver"></param>
        protected LinqSpecification(Func<Expression<Func<TType, bool>>> expressionResolver)
        {
            _ = expressionResolver.WhenNotNull(nameof(expressionResolver));

            Expression = expressionResolver.Invoke();
        }

        /// <summary>An implicit operator to return the specification as an Expression&lt;Func&lt;TType, bool&gt;&gt; so it can be used with
        /// <see cref="System.Linq.IQueryable{T}"/> based LINQ queries.</summary>
        /// <param name="specification">The specification to be returned as an Expression&lt;Func&lt;TType, bool&gt;&gt;.</param>
        public static implicit operator Expression<Func<TType, bool>>(LinqSpecification<TType> specification)
        {
            _ = specification.WhenNotNull(nameof(specification));

            return specification.Expression;
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
        /// <returns>A new specification that ANDs the provided specifications.</returns>
        public static LinqSpecification<TType> operator &(LinqSpecification<TType> leftSpecification, LinqSpecification<TType> rightSpecification)
        {
            return new AndLinqSpecification<TType>(leftSpecification, rightSpecification);
        }

        /// <summary>Provides support for operator ||.</summary>
        /// <param name="leftSpecification">The left operand applied to the operator.</param>
        /// <param name="rightSpecification">The right operand applied to the operator.</param>
        /// <returns>A new specification that ORs the provided specifications.</returns>
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

        /// <inheritdoc cref="SpecificationBase{TType}"/>
        public override bool IsSatisfiedBy(TType candidate)
        {
            return GetCompiledExpression().Invoke(candidate);
        }

        private Func<TType, bool> GetCompiledExpression()
        {
            _compiled ??= Expression.Compile();

            return _compiled;
        }
    }
}