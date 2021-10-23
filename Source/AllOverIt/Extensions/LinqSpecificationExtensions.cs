using AllOverIt.Patterns.Specification;

namespace AllOverIt.Extensions
{
    /// <summary>Provides extensions methods that simplify the composition of LINQ compatible specifications.</summary>
    public static class LinqSpecificationExtensions
    {
        /// <summary>Gets an expression that composes two specifications to perform a logical AND operation.</summary>
        /// <typeparam name="TType">The candidate type the specification applies to.</typeparam>
        /// <param name="leftSpecification">The left specification applied against a candidate.</param>
        /// <param name="rightSpecification">The right specification applied against a candidate.</param>
        /// <returns>An expression that composes two specifications to perform a logical AND operation.</returns>
        public static ILinqSpecification<TType> And<TType>(this ILinqSpecification<TType> leftSpecification, ILinqSpecification<TType> rightSpecification)
        {
            return new AndLinqSpecification<TType>(leftSpecification, rightSpecification);
        }

        /// <summary>Gets an expression that composes two specifications to perform a logical AND operation after negating
        /// the result of the right operand.</summary>
        /// <typeparam name="TType">The candidate type the specification applies to.</typeparam>
        /// <param name="leftSpecification">The left specification applied against a candidate.</param>
        /// <param name="rightSpecification">The right specification applied against a candidate.</param>
        /// <returns>An expression that composes two specifications to perform a logical AND operation.</returns>
        public static ILinqSpecification<TType> AndNot<TType>(this ILinqSpecification<TType> leftSpecification, ILinqSpecification<TType> rightSpecification)
        {
            return new AndNotLinqSpecification<TType>(leftSpecification, rightSpecification);
        }

        /// <summary>Gets an expression that composes two specifications to perform a logical OR operation.</summary>
        /// <typeparam name="TType">The candidate type the specification applies to.</typeparam>
        /// <param name="leftSpecification">The left specification applied against a candidate.</param>
        /// <param name="rightSpecification">The right specification applied against a candidate.</param>
        /// <returns>An expression that composes two specifications to perform a logical OR operation.</returns>
        public static ILinqSpecification<TType> Or<TType>(this ILinqSpecification<TType> leftSpecification, ILinqSpecification<TType> rightSpecification)
        {
            return new OrLinqSpecification<TType>(leftSpecification, rightSpecification);
        }

        /// <summary>Gets an expression that composes two specifications to perform a logical OR operation after negating
        /// the result of the right operand.</summary>
        /// <typeparam name="TType">The candidate type the specification applies to.</typeparam>
        /// <param name="leftSpecification">The left specification applied against a candidate.</param>
        /// <param name="rightSpecification">The right specification applied against a candidate.</param>
        /// <returns>An expression that composes two specifications to perform a logical OR operation.</returns>
        public static ILinqSpecification<TType> OrNot<TType>(this ILinqSpecification<TType> leftSpecification, ILinqSpecification<TType> rightSpecification)
        {
            return new OrNotLinqSpecification<TType>(leftSpecification, rightSpecification);
        }

        /// <summary>Gets an expression that negates a provided specification.</summary>
        /// <typeparam name="TType">The candidate type the specification applies to.</typeparam>
        /// <param name="specification">The specification applied against a candidate.</param>
        /// <returns>An expression that negates a provided specification.</returns>
        public static ILinqSpecification<TType> Not<TType>(this ILinqSpecification<TType> specification)
        {
            return new NotLinqSpecification<TType>(specification);
        }
    }
}