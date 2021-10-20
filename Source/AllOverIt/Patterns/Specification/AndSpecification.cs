using AllOverIt.Expressions;
using System;
using System.Linq.Expressions;

namespace AllOverIt.Patterns.Specification
{
    // Determines if a specified candidate meets the criteria defined by two specifications.
    // TType is the candidate type to be tested.
    public sealed class AndSpecification<TType> : BinarySpecification<TType>
    {
        public AndSpecification(ISpecification<TType> leftSpecification, ISpecification<TType> rightSpecification, bool negate = false)
            : base(leftSpecification, rightSpecification, negate)
        {
        }

        protected override bool DoIsSatisfiedBy(TType candidate)
        {
            return LeftSpecification.IsSatisfiedBy(candidate) && RightSpecification.IsSatisfiedBy(candidate);
        }
    }



    //
    public sealed class AndLinqSpecification<TType> : BinaryLinqSpecification<TType>
    {
        public AndLinqSpecification(ILinqSpecification<TType> leftSpecification, ILinqSpecification<TType> rightSpecification, bool negate = false)
            : base(leftSpecification, rightSpecification, negate)
        {
        }

        public override Expression<Func<TType, bool>> AsExpression()
        {
            var leftExpression = LeftSpecification.AsExpression();
            var rightExpression = RightSpecification.AsExpression();

            return leftExpression.And(rightExpression);
        }
    }













    // ???? are the items below different to using And / Or with negate - could be good for convenience ????


    public sealed class AndNotSpecification<TType> : BinarySpecification<TType>
    {
        public AndNotSpecification(ISpecification<TType> leftSpecification, ISpecification<TType> rightSpecification/*, bool negate = false*/)
            : base(leftSpecification, rightSpecification, false)
        {
        }

        protected override bool DoIsSatisfiedBy(TType candidate)
        {
            return LeftSpecification.IsSatisfiedBy(candidate) && !RightSpecification.IsSatisfiedBy(candidate);
        }
    }



    public sealed class OrNotSpecification<TType> : BinarySpecification<TType>
    {
        public OrNotSpecification(ISpecification<TType> leftSpecification, ISpecification<TType> rightSpecification/*, bool negate = false*/)
            : base(leftSpecification, rightSpecification, false)
        {
        }

        protected override bool DoIsSatisfiedBy(TType candidate)
        {
            return LeftSpecification.IsSatisfiedBy(candidate) || !RightSpecification.IsSatisfiedBy(candidate);
        }
    }



}