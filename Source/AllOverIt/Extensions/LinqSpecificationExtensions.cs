using AllOverIt.Patterns.Specification;

namespace AllOverIt.Extensions
{
    public static class LinqSpecificationExtensions
    {
        // Builds a new composite specification that performs a logical AND of one specification result with another.
        // TType is the candidate type to be tested.
        public static ILinqSpecification<TType> And<TType>(this ILinqSpecification<TType> leftSpecification, ILinqSpecification<TType> rightSpecification)
        {
            return new AndLinqSpecification<TType>(leftSpecification, rightSpecification);
        }

        // Builds a new composite specification that performs a logical AND of one specification result with another.
        // TSpecification is the the right-hand specification operand to construct and include as part of the resultant composite.
        // TSpecification must have a default constructor.
        // TType is the candidate type to be tested.
        public static ILinqSpecification<TType> And<TSpecification, TType>(this ILinqSpecification<TType> leftSpecification)
            where TSpecification : ILinqSpecification<TType>, new()
        {
            var rightSpecification = new TSpecification();
            return leftSpecification.And(rightSpecification);
        }

        // Builds a new composite specification that performs a logical AND of one specification result with another and then negates the result.
        // TType is the candidate type to be tested.
        public static ILinqSpecification<TType> AndNot<TType>(this ILinqSpecification<TType> leftSpecification, ILinqSpecification<TType> rightSpecification)
        {
            return new AndNotLinqSpecification<TType>(leftSpecification, rightSpecification);
        }

        // Builds a new composite specification that performs a logical AND of one specification result with another and then negates the result.
        // TType is the candidate type to be tested.
        // TSpecification is the the right-hand specification operand to construct and include as part of the resultant composite.
        // TSpecification must have a default constructor.
        // TType is the candidate type to be tested.
        public static ILinqSpecification<TType> AndNot<TSpecification, TType>(this ILinqSpecification<TType> leftSpecification)
            where TSpecification : ILinqSpecification<TType>, new()
        {
            var rightSpecification = new TSpecification();
            return leftSpecification.AndNot(rightSpecification);
        }

        // Builds a new composite specification that performs a logical OR of one specification result with another.
        // TType is the candidate type to be tested.
        public static ILinqSpecification<TType> Or<TType>(this ILinqSpecification<TType> leftSpecification, ILinqSpecification<TType> rightSpecification)
        {
            return new OrLinqSpecification<TType>(leftSpecification, rightSpecification);
        }

        // Builds a new composite specification that performs a logical OR of one specification result with another.
        // TSpecification is the the right-hand specification operand to construct and include as part of the resultant composite.
        // TSpecification must have a default constructor.
        // TType is the candidate type to be tested.
        public static ILinqSpecification<TType> Or<TSpecification, TType>(this ILinqSpecification<TType> leftSpecification)
            where TSpecification : ILinqSpecification<TType>, new()
        {
            var rightSpecification = new TSpecification();
            return leftSpecification.Or(rightSpecification);
        }

        // Builds a new composite specification that performs a logical OR of one specification result with another and then negates the result.
        // TType is the candidate type to be tested.
        public static ILinqSpecification<TType> OrNot<TType>(this ILinqSpecification<TType> leftSpecification, ILinqSpecification<TType> rightSpecification)
        {
            return new OrNotLinqSpecification<TType>(leftSpecification, rightSpecification);
        }

        // Builds a new composite specification that performs a logical OR of one specification result with another and then negates the result.
        // TType is the candidate type to be tested.
        // TSpecification is the the right-hand specification operand to construct and include as part of the resultant composite.
        // TSpecification must have a default constructor.
        // TType is the candidate type to be tested.
        public static ILinqSpecification<TType> OrNot<TSpecification, TType>(this ILinqSpecification<TType> leftSpecification)
            where TSpecification : ILinqSpecification<TType>, new()
        {
            var rightSpecification = new TSpecification();
            return leftSpecification.OrNot(rightSpecification);
        }

        // Builds a new specification that negates the result of the specified specification.
        // TType is the candidate type to be tested.
        public static ILinqSpecification<TType> Not<TType>(this ILinqSpecification<TType> specification)
        {
            return new NotLinqSpecification<TType>(specification);
        }
    }
}