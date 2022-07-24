using AllOverIt.Filtering.Builders.Extensions;
using AllOverIt.Filtering.Extensions;
using AllOverIt.Filtering.Options;
using AllOverIt.Patterns.Specification;
using System;
using System.Linq;

namespace AllOverIt.Filtering.Builders
{
    /// <summary>
    /// <para>
    /// Defines a queryable filter builder for a specified <typeparamref name="TType"/> object and filter type. This builder can
    /// be used for building any general purpose queryable specification but is typically used in conjunction with
    /// <see cref="QueryableExtensions.ApplyFilter{TType, TFilter}(IQueryable{TType}, TFilter, Action{IFilterSpecificationBuilder{TType, TFilter}, IFilterBuilder{TType, TFilter}}, DefaultQueryFilterOptions)"/>
    /// method.
    /// </para>
    /// <para>
    /// The current state of the filter builder can be expressed as a string for debugging purposes using the
    /// <see cref="FilterSpecificationExtensions.ToQueryString{TType, TFilter}"/> method.
    /// </para>
    /// </summary>
    /// <typeparam name="TType">The object type to apply the filter operation to.</typeparam>
    /// <typeparam name="TFilter">A custom filter type used for defining each operation or comparison in a specification.</typeparam>
    public interface IFilterBuilder<TType, TFilter> : IPredicateFilterBuilder<TType, TFilter>, ILogicalFilterBuilder<TType, TFilter>
      where TType : class
      where TFilter : class
    {
        /// <summary>
        /// <para>
        /// Gets the current logical expression to cater for additional of <see cref="Where(ILinqSpecification{TType})"/> or
        /// <see cref="ILogicalFilterBuilder{TType, TFilter}.And(ILinqSpecification{TType})"/> or <see cref="ILogicalFilterBuilder{TType, TFilter}.Or(ILinqSpecification{TType})"/>
        /// calls (or one of their overloads).
        /// </para>
        /// <para>
        /// This property returns the current filter state. Subsequent calls to <see cref="ILogicalFilterBuilder{TType, TFilter}.And(ILinqSpecification{TType})"/> or
        /// <see cref="ILogicalFilterBuilder{TType, TFilter}.Or(ILinqSpecification{TType})"/> (or one of their overloads) will apply the new expression as a binary operation
        /// against <see cref="Current"/>.
        /// </para>
        /// </summary>
        ILogicalFilterBuilder<TType, TFilter> Current { get; }
    }
}