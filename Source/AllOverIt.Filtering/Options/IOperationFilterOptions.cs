using AllOverIt.Filtering.Builders;
using AllOverIt.Filtering.Filters;
using System;

namespace AllOverIt.Filtering.Options
{
    /// <summary>Represents options that control how individual predicates are built using an <see cref="IFilterBuilder{TType, TFilter}"/>.</summary>
    public interface IOperationFilterOptions
    {
        /// <summary>Instructs the filter builder to generate parameterized queries for all criteria arguments. This is enabled by default
        /// to ensure database queries are not subject to SQL injection. This option can be disabled for memory based queries.</summary>
        bool UseParameterizedQueries { get; }

        /// <summary>Not configued by default as this is not applicable to database queries. Setting this option for memory based queries
        /// provides support for case-insensitive, and other, string comparisons.</summary>
        StringComparison? StringComparison { get; }

        /// <summary>When building a predicate via a filter builder (as an <see cref="IPredicateFilterBuilder{TType, TFilter}"/>
        /// or <see cref="ILogicalFilterBuilder{TType, TFilter}"/>) the comparison value is extracted from the provided expression (of
        /// type <see cref="IBasicFilterOperation"/> or <see cref="IArrayFilterOperation"/> or <see cref="IStringFilterOperation"/>).
        /// This option instructs the builder to ignore the predicate if the filter's value is null. This can be handy for automatically
        /// excluding optional filters.</summary>
        bool IgnoreNullFilterValue { get; }
    }
}