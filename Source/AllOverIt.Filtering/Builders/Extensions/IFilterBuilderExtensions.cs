﻿using AllOverIt.Assertion;
using AllOverIt.Filtering.Filters;
using AllOverIt.Patterns.Specification.Utils;

namespace AllOverIt.Filtering.Builders.Extensions
{
    public static class IFilterBuilderExtensions
    {
        public static string AsQueryString<TType, TFilter>(this IFilterBuilder<TType, TFilter> filterBuilder)
           where TType : class
           where TFilter : class, IFilter
        {
            _ = filterBuilder.WhenNotNull(nameof(filterBuilder));

            var visitor = new LinqSpecificationVisitor();

            return visitor.AsQueryString(filterBuilder.AsSpecification);
        }
    }
}