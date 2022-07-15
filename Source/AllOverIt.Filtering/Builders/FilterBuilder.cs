using AllOverIt.Assertion;
using AllOverIt.Filtering.Filters;
using AllOverIt.Patterns.Specification;
using AllOverIt.Patterns.Specification.Extensions;
using System;
using System.Linq.Expressions;

namespace AllOverIt.Filtering.Builders
{
    internal sealed class FilterBuilder<TType, TFilter> : IFilterBuilder<TType, TFilter>, ILogicalFilterBuilder<TType, TFilter>
        where TType : class
        where TFilter : class, IFilter
    {
        private readonly IFilterSpecificationBuilder<TType, TFilter> _specificationBuilder;

        private ILinqSpecification<TType> _currentSpecification;

        public ILinqSpecification<TType> QuerySpecification => _currentSpecification;

        public FilterBuilder(IFilterSpecificationBuilder<TType, TFilter> specificationBuilder)
        {
            _specificationBuilder = specificationBuilder.WhenNotNull(nameof(specificationBuilder));
        }

        #region WHERE Operations
        public ILogicalFilterBuilder<TType, TFilter> Where(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringOperation> operation)
        {
            _currentSpecification = _specificationBuilder.GetSpecification(propertyExpression, operation);

            return this;
        }

        public ILogicalFilterBuilder<TType, TFilter> Where<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IOperation> operation)
        {
            _currentSpecification = _specificationBuilder.GetSpecification(propertyExpression, operation);

            return this;
        }

        public ILogicalFilterBuilder<TType, TFilter> Where(ILinqSpecification<TType> specification)
        {
            _currentSpecification = specification;

            return this;
        }
        #endregion

        #region AND Operations
        public ILogicalFilterBuilder<TType, TFilter> And(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringOperation> operation)
        {
            var specification = _specificationBuilder.GetSpecification(propertyExpression, operation);
            _currentSpecification = _currentSpecification.And(specification);

            return this;
        }

        public ILogicalFilterBuilder<TType, TFilter> And<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IOperation> operation)
        {
            var specification = _specificationBuilder.GetSpecification(propertyExpression, operation);
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
        public ILogicalFilterBuilder<TType, TFilter> Or(Expression<Func<TType, string>> propertyExpression, Func<TFilter, IStringOperation> operation)
        {
            var specification = _specificationBuilder.GetSpecification(propertyExpression, operation);
            _currentSpecification = _currentSpecification.Or(specification);

            return this;
        }


        public ILogicalFilterBuilder<TType, TFilter> Or<TProperty>(Expression<Func<TType, TProperty>> propertyExpression, Func<TFilter, IOperation> operation)
        {
            var specification = _specificationBuilder.GetSpecification(propertyExpression, operation);
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