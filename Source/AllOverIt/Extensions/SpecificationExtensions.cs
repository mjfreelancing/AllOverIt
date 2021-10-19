using AllOverIt.Patterns.Specification;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Extensions
{
    public static class SpecificationExtensions
    {
        // Builds a new composite specification that performs a logical AND of one specification result with another.
        // TType is the candidate type to be tested.
        public static ISpecification<TType> And<TType>(this ISpecification<TType> leftSpecification, ISpecification<TType> rightSpecification)
        {
            return new AndSpecification<TType>(leftSpecification, rightSpecification);
        }

        // Builds a new composite specification that performs a logical AND of one specification result with another.
        // TSpecification is the the right-hand specification operand to construct and include as part of the resultant composite.
        // TSpecification must have a default constructor.
        // TType is the candidate type to be tested.
        public static ISpecification<TType> And<TSpecification, TType>(this ISpecification<TType> leftSpecification)
          where TSpecification : ISpecification<TType>, new()
        {
            var rightSpecification = new TSpecification();
            return leftSpecification.And(rightSpecification);
        }

        // Builds a new composite specification that performs a logical AND of one specification result with another and then negates the result.
        // TType is the candidate type to be tested.
        public static ISpecification<TType> AndNot<TType>(this ISpecification<TType> leftSpecification,
          ISpecification<TType> rightSpecification)
        {
            return new AndSpecification<TType>(leftSpecification, rightSpecification, true);
        }

        // Builds a new composite specification that performs a logical AND of one specification result with another and then negates the result.
        // TType is the candidate type to be tested.
        // TSpecification is the the right-hand specification operand to construct and include as part of the resultant composite.
        // TSpecification must have a default constructor.
        // TType is the candidate type to be tested.
        public static ISpecification<TType> AndNot<TSpecification, TType>(this ISpecification<TType> leftSpecification)
          where TSpecification : ISpecification<TType>, new()
        {
            var rightSpecification = new TSpecification();
            return leftSpecification.AndNot(rightSpecification);
        }

        // Builds a new composite specification that performs a logical OR of one specification result with another.
        // TType is the candidate type to be tested.
        public static ISpecification<TType> Or<TType>(this ISpecification<TType> leftSpecification,
          ISpecification<TType> rightSpecification)
        {
            return new OrSpecification<TType>(leftSpecification, rightSpecification);
        }

        // Builds a new composite specification that performs a logical OR of one specification result with another.
        // TSpecification is the the right-hand specification operand to construct and include as part of the resultant composite.
        // TSpecification must have a default constructor.
        // TType is the candidate type to be tested.
        public static ISpecification<TType> Or<TSpecification, TType>(this ISpecification<TType> leftSpecification)
          where TSpecification : ISpecification<TType>, new()
        {
            var rightSpecification = new TSpecification();
            return leftSpecification.Or(rightSpecification);
        }

        // Builds a new composite specification that performs a logical OR of one specification result with another and then negates the result.
        // TType is the candidate type to be tested.
        public static ISpecification<TType> OrNot<TType>(this ISpecification<TType> leftSpecification,
          ISpecification<TType> rightSpecification)
        {
            return new OrSpecification<TType>(leftSpecification, rightSpecification, true);
        }

        // Builds a new composite specification that performs a logical OR of one specification result with another and then negates the result.
        // TType is the candidate type to be tested.
        // TSpecification is the the right-hand specification operand to construct and include as part of the resultant composite.
        // TSpecification must have a default constructor.
        // TType is the candidate type to be tested.
        public static ISpecification<TType> OrNot<TSpecification, TType>(this ISpecification<TType> leftSpecification)
          where TSpecification : ISpecification<TType>, new()
        {
            var rightSpecification = new TSpecification();
            return leftSpecification.OrNot(rightSpecification);
        }

        // Builds a new specification that negates the result of the specified specification.
        // TType is the candidate type to be tested.
        public static ISpecification<TType> Not<TType>(this ISpecification<TType> specification)
        {
            return new NotSpecification<TType>(specification);
        }

        // Gets all candidates that meet the criteria of a specified specification.
        // TType is the candidate type to be tested.
        public static IEnumerable<TType> WhereSatisfied<TType>(this IEnumerable<TType> candidates, ISpecification<TType> specification)
        {
            return candidates.Where(specification.IsSatisfiedBy);
        }

        // Determines if any specified candidates meet the criteria of a specified specification.
        // TType is the candidate type to be tested.
        public static bool AnySatisfied<TType>(this IEnumerable<TType> candidates, ISpecification<TType> specification)
        {
            return candidates.Any(specification.IsSatisfiedBy);
        }

        // Determines if all specified candidates meet the criteria of a specified specification.
        // TType is the candidate type to be tested.
        public static bool AllSatisfied<TType>(this IEnumerable<TType> candidates, ISpecification<TType> specification)
        {
            return candidates.All(specification.IsSatisfiedBy);
        }
    }
}