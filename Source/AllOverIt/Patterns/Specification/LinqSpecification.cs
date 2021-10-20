﻿using System;
using System.Linq.Expressions;
using AllOverIt.Helpers;

namespace AllOverIt.Patterns.Specification
{
    public abstract class LinqSpecification<TType> : SpecificationBase<TType>, ILinqSpecification<TType>
    {
        private Func<TType, bool> _compiled;

        protected LinqSpecification(bool negate = false)
            : base(negate)
        {
        }

        public abstract Expression<Func<TType, bool>> AsExpression();

        public static implicit operator Expression<Func<TType, bool>>(LinqSpecification<TType> specification)
        {
            _ = specification.WhenNotNull(nameof(specification));

            return specification.AsExpression();
        }

        public static LinqSpecification<TType> operator &(LinqSpecification<TType> leftSpecification, LinqSpecification<TType> rightSpecification)
        {
            return new AndLinqSpecification<TType>(leftSpecification, rightSpecification);
        }

        public static LinqSpecification<TType> operator |(LinqSpecification<TType> leftSpecification, LinqSpecification<TType> rightSpecification)
        {
            return new OrLinqSpecification<TType>(leftSpecification, rightSpecification);
        }

        public static LinqSpecification<TType> operator !(LinqSpecification<TType> specification)
        {
            return new NotLinqSpecification<TType>(specification);
        }

        protected sealed override bool DoIsSatisfiedBy(TType candidate)
        {
            _compiled ??= AsExpression().Compile();

            return _compiled.Invoke(candidate);
        }
    }
}