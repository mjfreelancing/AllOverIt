using AllOverIt.Patterns.Specification;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Extensions
{
    public static class QueryableExtensions
    {
        // Gets all candidates that meet the criteria of a specified specification.
        public static IEnumerable<TType> Where<TType>(this IQueryable<TType> candidates, ILinqSpecification<TType> specification)
        {
            return candidates.Where(specification.AsExpression());
        }

        // Determines if any specified candidates meet the criteria of a specified specification.
        public static bool Any<TType>(this IQueryable<TType> candidates, ILinqSpecification<TType> specification)
        {
            return candidates.Any(specification.AsExpression());
        }

        // Determines if all specified candidates meet the criteria of a specified specification.
        public static bool All<TType>(this IQueryable<TType> candidates, ILinqSpecification<TType> specification)
        {
            return candidates.All(specification.AsExpression());
        }

        public static int Count<TType>(this IQueryable<TType> candidates, ILinqSpecification<TType> specification)
        {
            return candidates.Count(specification.AsExpression());
        }

        public static TType First<TType>(this IQueryable<TType> candidates, ILinqSpecification<TType> specification)
        {
            return candidates.First(specification.AsExpression());
        }

        public static TType FirstOrDefault<TType>(this IQueryable<TType> candidates, ILinqSpecification<TType> specification)
        {
            return candidates.FirstOrDefault(specification.AsExpression());
        }

        public static TType Last<TType>(this IQueryable<TType> candidates, ILinqSpecification<TType> specification)
        {
            return candidates.Last(specification.AsExpression());
        }

        public static TType LastOrDefault<TType>(this IQueryable<TType> candidates, ILinqSpecification<TType> specification)
        {
            return candidates.LastOrDefault(specification.AsExpression());
        }

        public static IQueryable<TType> SkipWhile<TType>(this IQueryable<TType> candidates, ILinqSpecification<TType> specification)
        {
            return candidates.SkipWhile(specification.AsExpression());
        }

        public static IQueryable<TType> TakeWhile<TType>(this IQueryable<TType> candidates, ILinqSpecification<TType> specification)
        {
            return candidates.TakeWhile(specification.AsExpression());
        }
    }
}