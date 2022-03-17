﻿using AllOverIt.Expressions;
using System;
using System.Linq.Expressions;

namespace AllOverIt.Patterns.Specification
{
    /// <summary>A specification that performs a logical OR operation between two expressions after negating the result of the right operand.</summary>
    /// <typeparam name="TType">The candidate type the specification applies to.</typeparam>
    public sealed class OrNotLinqSpecification<TType> : LinqSpecification<TType>
    {
        /// <summary>Constructor.</summary>
        /// <param name="leftSpecification">The left specification applied against a candidate.</param>
        /// <param name="rightSpecification">The right specification applied against a candidate.</param>
        public OrNotLinqSpecification(ILinqSpecification<TType> leftSpecification, ILinqSpecification<TType> rightSpecification)
            : base(() => GetExpression(leftSpecification, rightSpecification))
        {
        }

        private static Expression<Func<TType, bool>> GetExpression(ILinqSpecification<TType> leftSpecification, ILinqSpecification<TType> rightSpecification)
        {
            var leftExpression = leftSpecification.Expression;
            var rightExpression = rightSpecification.Expression;

            return leftExpression.Or(rightExpression.Not());
        }
    }
}