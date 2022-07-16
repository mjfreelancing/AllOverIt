using AllOverIt.Assertion;
using AllOverIt.Filtering.Filters;
using AllOverIt.Patterns.Specification;
using AllOverIt.Patterns.Specification.Extensions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AllOverIt.Filtering.Builders
{
    internal sealed class FilterBuilder<TType, TFilter> : IFilterBuilder<TType, TFilter>, ILogicalFilterBuilder<TType, TFilter>
        where TType : class
        where TFilter : class, IFilter
    {
        private readonly IFilterSpecificationBuilder<TType, TFilter> _specificationBuilder;

        private ILinqSpecification<TType> _currentSpecification;

        // The final specification that can be applied to an IQueryable.Where()
        public ILinqSpecification<TType> QuerySpecification => _currentSpecification;

        public ILogicalFilterBuilder<TType, TFilter> Current => this;

        public FilterBuilder(IFilterSpecificationBuilder<TType, TFilter> specificationBuilder)
        {
            _specificationBuilder = specificationBuilder.WhenNotNull(nameof(specificationBuilder));
        }

        #region WHERE Operations
        public ILogicalFilterBuilder<TType, TFilter> Where(Expression<Func<TType, string>> propertyExpression,
            Func<TFilter, IStringFilterOperation> operation)
        {
            _currentSpecification = _specificationBuilder.Create(propertyExpression, operation);
            return this;
        }

        // Sequential Where() calls are AND' together
        public ILogicalFilterBuilder<TType, TFilter> Where<TProperty>(Expression<Func<TType, IList<TProperty>>> propertyExpression,
            Func<TFilter, IArrayFilterOperation> operation)
        {
            var nextSpecfication = _specificationBuilder.Create(propertyExpression, operation);

            _currentSpecification = _currentSpecification == null
                ? nextSpecfication
                : _currentSpecification.And(nextSpecfication);

            return this;
        }

        public ILogicalFilterBuilder<TType, TFilter> Where<TProperty>(Expression<Func<TType, TProperty>> propertyExpression,
            Func<TFilter, IFilterOperation> operation)
        {
            var nextSpecfication = _specificationBuilder.Create(propertyExpression, operation);

            _currentSpecification = _currentSpecification == null
                ? nextSpecfication
                : _currentSpecification.And(nextSpecfication);

            return this;
        }

        public ILogicalFilterBuilder<TType, TFilter> Where(ILinqSpecification<TType> specification)
        {
            _currentSpecification = _currentSpecification == null
                ? specification
                : _currentSpecification.And(specification);

            return this;
        }
        #endregion

        #region AND Operations
        public ILogicalFilterBuilder<TType, TFilter> And(Expression<Func<TType, string>> propertyExpression,
            Func<TFilter, IStringFilterOperation> operation)
        {
            var specification = _specificationBuilder.Create(propertyExpression, operation);
            _currentSpecification = _currentSpecification.And(specification);

            return this;
        }

        public ILogicalFilterBuilder<TType, TFilter> And<TProperty>(Expression<Func<TType, TProperty>> propertyExpression,
            Func<TFilter, IFilterOperation> operation)
        {
            var specification = _specificationBuilder.Create(propertyExpression, operation);
            _currentSpecification = _currentSpecification.And(specification);

            return this;
        }

        public ILogicalFilterBuilder<TType, TFilter> And(ILinqSpecification<TType> specification)
        {
            _currentSpecification = _currentSpecification.And(specification);

            return this;
        }
        #endregion

        #region OR Operations
        public ILogicalFilterBuilder<TType, TFilter> Or(Expression<Func<TType, string>> propertyExpression,
            Func<TFilter, IStringFilterOperation> operation)
        {
            var specification = _specificationBuilder.Create(propertyExpression, operation);
            _currentSpecification = _currentSpecification.Or(specification);

            return this;
        }


        public ILogicalFilterBuilder<TType, TFilter> Or<TProperty>(Expression<Func<TType, TProperty>> propertyExpression,
            Func<TFilter, IFilterOperation> operation)
        {
            var specification = _specificationBuilder.Create(propertyExpression, operation);
            _currentSpecification = _currentSpecification.Or(specification);

            return this;
        }

        public ILogicalFilterBuilder<TType, TFilter> Or(ILinqSpecification<TType> specification)
        {
            _currentSpecification = _currentSpecification.Or(specification);

            return this;
        }
        #endregion
    }
}