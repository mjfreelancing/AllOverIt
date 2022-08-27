using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Patterns.Specification.Extensions
{
    /// <summary>Provides a variety of specification extension methods for <see cref="IEnumerable{T}"/>.</summary>
    public static class EnumerableExtensions
    {
        /// <summary>Gets all candidates that meet the criteria of a provided specification.</summary>
        /// <typeparam name="TType">The candidate type the specification applies to.</typeparam>
        /// <param name="candidates">The elements to apply the specification against.</param>
        /// <param name="specification">The specification to apply against a collection of elements.</param>
        /// <returns>The candidates that meet the criteria of the provided specification.</returns>
        public static IEnumerable<TType> Where<TType>(this IEnumerable<TType> candidates, ISpecification<TType> specification)
        {
            return candidates.Where(specification.IsSatisfiedBy);
        }

        /// <summary>Determines if any candidates meet the criteria of a provided specification.</summary>
        /// <typeparam name="TType">The candidate type the specification applies to.</typeparam>
        /// <param name="candidates">The elements to apply the specification against.</param>
        /// <param name="specification">The specification to apply against a collection of elements.</param>
        /// <returns>True if any of the candidates meet the criteria of the provided specification.</returns>
        public static bool Any<TType>(this IEnumerable<TType> candidates, ISpecification<TType> specification)
        {
            return candidates.Any(specification.IsSatisfiedBy);
        }

        /// <summary>Determines if all candidates meet the criteria of a provided specification.</summary>
        /// <typeparam name="TType">The candidate type the specification applies to.</typeparam>
        /// <param name="candidates">The elements to apply the specification against.</param>
        /// <param name="specification">The specification to apply against a collection of elements.</param>
        /// <returns>True if all of the candidates meet the criteria of the provided specification.</returns>
        public static bool All<TType>(this IEnumerable<TType> candidates, ISpecification<TType> specification)
        {
            return candidates.All(specification.IsSatisfiedBy);
        }

        /// <summary>Counts the number of candidates that meet the criteria of a provided specification.</summary>
        /// <typeparam name="TType">The candidate type the specification applies to.</typeparam>
        /// <param name="candidates">The elements to apply the specification against.</param>
        /// <param name="specification">The specification to apply against a collection of elements.</param>
        /// <returns>The count of candidates that meet the criteria of a provided specification.</returns>
        public static int Count<TType>(this IEnumerable<TType> candidates, ISpecification<TType> specification)
        {
            return candidates.Count(specification.IsSatisfiedBy);
        }

        /// <summary>Gets the first candidate that meets the criteria of a provided specification.</summary>
        /// <typeparam name="TType">The candidate type the specification applies to.</typeparam>
        /// <param name="candidates">The elements to apply the specification against.</param>
        /// <param name="specification">The specification to apply against a collection of elements.</param>
        /// <returns>The first candidate that meets the criteria of a provided specification.</returns>
        public static TType First<TType>(this IEnumerable<TType> candidates, ISpecification<TType> specification)
        {
            return candidates.First(specification.IsSatisfiedBy);
        }

        /// <summary>Gets the first candidate that meets the criteria of a provided specification or the type's
        /// default if there are no matches.</summary>
        /// <typeparam name="TType">The candidate type the specification applies to.</typeparam>
        /// <param name="candidates">The elements to apply the specification against.</param>
        /// <param name="specification">The specification to apply against a collection of elements.</param>
        /// <returns>The first candidate that meets the criteria of a provided specification or the type's
        /// default if there are no matches.</returns>
        public static TType FirstOrDefault<TType>(this IEnumerable<TType> candidates, ISpecification<TType> specification)
        {
            return candidates.FirstOrDefault(specification.IsSatisfiedBy);
        }

        /// <summary>Gets the last candidate that meets the criteria of a provided specification.</summary>
        /// <typeparam name="TType">The candidate type the specification applies to.</typeparam>
        /// <param name="candidates">The elements to apply the specification against.</param>
        /// <param name="specification">The specification to apply against a collection of elements.</param>
        /// <returns>The last candidate that meets the criteria of a provided specification.</returns>
        public static TType Last<TType>(this IEnumerable<TType> candidates, ISpecification<TType> specification)
        {
            return candidates.Last(specification.IsSatisfiedBy);
        }

        /// <summary>Gets the last candidate that meets the criteria of a provided specification or the type's
        /// default if there are no matches.</summary>
        /// <typeparam name="TType">The candidate type the specification applies to.</typeparam>
        /// <param name="candidates">The elements to apply the specification against.</param>
        /// <param name="specification">The specification to apply against a collection of elements.</param>
        /// <returns>The last candidate that meets the criteria of a provided specification or the type's
        /// default if there are no matches.</returns>
        public static TType LastOrDefault<TType>(this IEnumerable<TType> candidates, ISpecification<TType> specification)
        {
            return candidates.LastOrDefault(specification.IsSatisfiedBy);
        }

        /// <summary>Gets the elements from the first element of the input candidates that meets the criteria of a provided
        /// specification.</summary>
        /// <typeparam name="TType">The candidate type the specification applies to.</typeparam>
        /// <param name="candidates">The elements to apply the specification against.</param>
        /// <param name="specification">The specification to apply against a collection of elements.</param>
        /// <returns>An enumerable that contains the elements from the first element of the input candidates that meets
        /// the criteria of a provided specification.</returns>
        public static IEnumerable<TType> SkipWhile<TType>(this IEnumerable<TType> candidates, ISpecification<TType> specification)
        {
            return candidates.SkipWhile(specification.IsSatisfiedBy);
        }

        /// <summary>Gets the elements from the input candidates while they meet the criteria of a provided specification.</summary>
        /// <typeparam name="TType">The candidate type the specification applies to.</typeparam>
        /// <param name="candidates">The elements to apply the specification against.</param>
        /// <param name="specification">The specification to apply against a collection of elements.</param>
        /// <returns>An enumerable that contains the elements from the input candidates while they meet the criteria of a provided
        /// specification.</returns>
        public static IEnumerable<TType> TakeWhile<TType>(this IEnumerable<TType> candidates, ISpecification<TType> specification)
        {
            return candidates.TakeWhile(specification.IsSatisfiedBy);
        }
    }
}