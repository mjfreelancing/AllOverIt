﻿using AllOverIt.Assertion;
using AllOverIt.Filtering.Filters;
using AllOverIt.Patterns.Specification;
using AllOverIt.Patterns.Specification.Extensions;
using System;
using System.Linq.Expressions;

namespace AllOverIt.Filtering.Builders
{
    // Use the AsQueryString() extension method to get a string representation
    internal sealed class FilterBuilder<TType, TFilter> : IFilterBuilder<TType, TFilter>, ILogicalFilterBuilder<TType, TFilter>
        where TType : class
        where TFilter : class
    {
        private readonly IFilterSpecificationBuilder<TType, TFilter> _specificationBuilder;

        private ILinqSpecification<TType> _currentSpecification;

        // The final specification that can be applied to an IQueryable.Where()
        public ILinqSpecification<TType> AsSpecification => _currentSpecification;

        // Gets the current logical expression to cater for additional chaining.
        public ILogicalFilterBuilder<TType, TFilter> Current => this;

        public FilterBuilder(IFilterSpecificationBuilder<TType, TFilter> specificationBuilder)
        {
            _specificationBuilder = specificationBuilder.WhenNotNull(nameof(specificationBuilder));
        }

        #region WHERE Operations
        public ILogicalFilterBuilder<TType, TFilter> Where(Expression<Func<TType, string>> propertyExpression,
            Func<TFilter, IStringFilterOperation> operation)
        {
            var specification = _specificationBuilder.Create(propertyExpression, operation);

            ApplyNextSpecification(specification, LinqSpecificationExtensions.And);

            return this;
        }

        public ILogicalFilterBuilder<TType, TFilter> Where<TProperty>(Expression<Func<TType, TProperty>> propertyExpression,
            Func<TFilter, IBasicFilterOperation> operation)
        {
            var specification = _specificationBuilder.Create(propertyExpression, operation);

            ApplyNextSpecification(specification, LinqSpecificationExtensions.And);

            return this;
        }

        public ILogicalFilterBuilder<TType, TFilter> Where(ILinqSpecification<TType> specification)
        {
            ApplyNextSpecification(specification, LinqSpecificationExtensions.And);

            return this;
        }
        #endregion

        #region AND Operations
        public ILogicalFilterBuilder<TType, TFilter> And(Expression<Func<TType, string>> propertyExpression,
            Func<TFilter, IStringFilterOperation> operation)
        {
            var specification = _specificationBuilder.Create(propertyExpression, operation);

            ApplyNextSpecification(specification, LinqSpecificationExtensions.And);

            return this;
        }

        public ILogicalFilterBuilder<TType, TFilter> And<TProperty>(Expression<Func<TType, TProperty>> propertyExpression,
            Func<TFilter, IBasicFilterOperation> operation)
        {
            var specification = _specificationBuilder.Create(propertyExpression, operation);

            ApplyNextSpecification(specification, LinqSpecificationExtensions.And);

            return this;
        }

        public ILogicalFilterBuilder<TType, TFilter> And(ILinqSpecification<TType> specification)
        {
            ApplyNextSpecification(specification, LinqSpecificationExtensions.And);

            return this;
        }
        #endregion

        #region OR Operations
        public ILogicalFilterBuilder<TType, TFilter> Or(Expression<Func<TType, string>> propertyExpression,
            Func<TFilter, IStringFilterOperation> operation)
        {
            var specification = _specificationBuilder.Create(propertyExpression, operation);

            ApplyNextSpecification(specification, LinqSpecificationExtensions.Or);

            return this;
        }

        public ILogicalFilterBuilder<TType, TFilter> Or<TProperty>(Expression<Func<TType, TProperty>> propertyExpression,
            Func<TFilter, IBasicFilterOperation> operation)
        {
            var specification = _specificationBuilder.Create(propertyExpression, operation);

            ApplyNextSpecification(specification, LinqSpecificationExtensions.Or);

            return this;
        }

        public ILogicalFilterBuilder<TType, TFilter> Or(ILinqSpecification<TType> specification)
        {
            ApplyNextSpecification(specification, LinqSpecificationExtensions.Or);

            return this;
        }
        #endregion

        private void ApplyNextSpecification(ILinqSpecification<TType> specification,
           Func<ILinqSpecification<TType>, ILinqSpecification<TType>, ILinqSpecification<TType>> action)
        {
            if (Accept(specification))
            {
                _currentSpecification = _currentSpecification == null
                    ? specification
                    : action.Invoke(_currentSpecification, specification);
            }
        }

        private static bool Accept(ILinqSpecification<TType> specification)
        {
            return specification != FilterSpecificationBuilder<TType, TFilter>.SpecificationIgnore;
        }
    }
}