using AllOverIt.Patterns.Specification;

namespace AllOverIt.Extensions
{
    /// <summary>Provides extensions methods that simplify the composition of specifications.</summary>
    public static class SpecificationExtensions
    {
        /// <summary>Gets an expression that composes two specifications to perform a logical AND operation.</summary>
        /// <typeparam name="TType">The candidate type the specification applies to.</typeparam>
        /// <param name="leftSpecification">The left specification applied against a candidate.</param>
        /// <param name="rightSpecification">The right specification applied against a candidate.</param>
        /// <returns>An expression that composes two specifications to perform a logical AND operation.</returns>
        public static ISpecification<TType> And<TType>(this ISpecification<TType> leftSpecification, ISpecification<TType> rightSpecification)
        {
            return new AndSpecification<TType>(leftSpecification, rightSpecification);
        }

        /// <summary>Gets an expression that composes two specifications to perform a logical AND operation after negating
        /// the result of the right operand.</summary>
        /// <typeparam name="TType">The candidate type the specification applies to.</typeparam>
        /// <param name="leftSpecification">The left specification applied against a candidate.</param>
        /// <param name="rightSpecification">The right specification applied against a candidate.</param>
        /// <returns>An expression that composes two specifications to perform a logical AND operation.</returns>
        public static ISpecification<TType> AndNot<TType>(this ISpecification<TType> leftSpecification, ISpecification<TType> rightSpecification)
        {
            return new AndNotSpecification<TType>(leftSpecification, rightSpecification);
        }

        /// <summary>Gets an expression that composes two specifications to perform a logical OR operation.</summary>
        /// <typeparam name="TType">The candidate type the specification applies to.</typeparam>
        /// <param name="leftSpecification">The left specification applied against a candidate.</param>
        /// <param name="rightSpecification">The right specification applied against a candidate.</param>
        /// <returns>An expression that composes two specifications to perform a logical OR operation.</returns>
        public static ISpecification<TType> Or<TType>(this ISpecification<TType> leftSpecification, ISpecification<TType> rightSpecification)
        {
            return new OrSpecification<TType>(leftSpecification, rightSpecification);
        }

        /// <summary>Gets an expression that composes two specifications to perform a logical OR operation after negating
        /// the result of the right operand.</summary>
        /// <typeparam name="TType">The candidate type the specification applies to.</typeparam>
        /// <param name="leftSpecification">The left specification applied against a candidate.</param>
        /// <param name="rightSpecification">The right specification applied against a candidate.</param>
        /// <returns>An expression that composes two specifications to perform a logical OR operation.</returns>
        public static ISpecification<TType> OrNot<TType>(this ISpecification<TType> leftSpecification, ISpecification<TType> rightSpecification)
        {
            return new OrNotSpecification<TType>(leftSpecification, rightSpecification);
        }

        /// <summary>Gets an expression that negates a provided specification.</summary>
        /// <typeparam name="TType">The candidate type the specification applies to.</typeparam>
        /// <param name="specification">The specification applied against a candidate.</param>
        /// <returns>An expression that negates a provided specification.</returns>
        public static ISpecification<TType> Not<TType>(this ISpecification<TType> specification)
        {
            return new NotSpecification<TType>(specification);
        }
    }
}