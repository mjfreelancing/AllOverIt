﻿using AllOverIt.Helpers;
using System;
using System.Linq.Expressions;

namespace AllOverIt.Patterns.Specification
{
    // An abstract specification class that determines if an object meets the criteria defined by the specification.
    // TType is the candidate type to be tested.
    public abstract class Specification<TType> : ISpecification<TType>
    {
        // Indicates if the result of the specification's criteria should be negated.
        private bool Negate { get; }

        protected Specification(bool negate = false)
        {
            Negate = negate;
        }

        protected abstract bool DoIsSatisfiedBy(TType candidate);

        public bool IsSatisfiedBy(TType candidate)
        {
            var result = DoIsSatisfiedBy(candidate);

            return Negate
              ? !result
              : result;
        }





        // Use with IEnumerable LINQ
        public static implicit operator Func<TType, bool>(Specification<TType> specification)
        {
            _ = specification.WhenNotNull(nameof(specification));

            return specification.IsSatisfiedBy;
        }


        // required in combination with operator & and | to support && and ||
        public static bool operator true(Specification<TType> _)
        {
            return false;
        }

        // required in combination with operator & and | to support && and ||
        public static bool operator false(Specification<TType> _)
        {
            return false;
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




    public interface ILinqSpecification<TType> : ISpecification<TType>
    {
        Expression<Func<TType, bool>> AsExpression();
    }




    public abstract class LinqSpecification<TType> : Specification<TType>, ILinqSpecification<TType>
    {
        private Func<TType, bool> _compiled;

        protected LinqSpecification(bool negate = false)
            : base(negate)
        {
        }

        public abstract Expression<Func<TType, bool>> AsExpression();

        // Use with IQueryable LINQ
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