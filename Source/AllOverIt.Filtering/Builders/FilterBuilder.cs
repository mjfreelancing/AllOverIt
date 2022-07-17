using AllOverIt.Assertion;
using AllOverIt.Filtering.Filters;
using AllOverIt.Patterns.Specification;
using AllOverIt.Patterns.Specification.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace AllOverIt.Filtering.Builders
{
    public class FilterQueryVisitor : ExpressionVisitor
    {
        private static readonly IDictionary<ExpressionType, string> _expressionOperators = new Dictionary<ExpressionType, string>
        {
            [ExpressionType.Not] = "NOT",
            [ExpressionType.GreaterThan] = ">",
            [ExpressionType.GreaterThanOrEqual] = ">=",
            [ExpressionType.LessThan] = "<",
            [ExpressionType.LessThanOrEqual] = "<=",
            [ExpressionType.Equal] = "==",
            [ExpressionType.NotEqual] = "!=",
            [ExpressionType.AndAlso] = "AND",
            [ExpressionType.OrElse] = "OR"
        };

        private static readonly IDictionary<Type, Func<object, string>> _typeConverters = new Dictionary<Type, Func<object, string>>
        {
            [typeof(string)] = value => $"'{value}'",
            [typeof(DateTime)] = value => $"'{((DateTime) value).ToUniversalTime():yyyy-MM-ddTHH:mm:ss.fffZ}'",
            [typeof(bool)] = value => value.ToString().ToLower()
        };

        private readonly StringBuilder _queryStringBuilder = new();
        private readonly Stack<string> _fieldNames = new();

        public string AsQueryString(LambdaExpression predicate)
        {
            Visit(predicate.Body);
            var result = _queryStringBuilder.ToString();
            _queryStringBuilder.Clear();

            return result;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Object != null)
            {
                Visit(node.Object);
            }

            _queryStringBuilder.Append($" {node.Method.Name} ");

            Visit(node.Arguments[0]);       // TODO: Check is one of StartsWith, EndsWith, Contains

            return node;
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            if (node.NodeType == ExpressionType.Convert)
            {
                //var me = node.Operand as MemberExpression;


                Visit(node.Operand);
                return node;
            }

            if (node.NodeType != ExpressionType.Not)
            {
                throw new NotSupportedException("Only not(\"!\") unary operator is supported!");
            }

            _queryStringBuilder.Append($" {_expressionOperators[node.NodeType]} ");

            _queryStringBuilder.Append("(");
            Visit(node.Operand);
            _queryStringBuilder.Append(")");

            return node;
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            _queryStringBuilder.Append("(");
            Visit(node.Left);

            _queryStringBuilder.Append($" {_expressionOperators[node.NodeType]} ");

            Visit(node.Right);
            _queryStringBuilder.Append(")");

            return node;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            _queryStringBuilder.Append(GetValue(node.Value));

            return node;
        }

        private string GetValue(object input)
        {
            var type = input.GetType();

            if (type.IsClass && type != typeof(string))
            {
                var fieldName = _fieldNames.Pop();
                var fieldInfo = type.GetField(fieldName);

                var value = fieldInfo == null
                    ? type.GetProperty(fieldName).GetValue(input)
                    : fieldInfo.GetValue(input);

                if (value is IList list)
                {
                    var items = new List<string>();

                    foreach (var item in list)
                    {
                        items.Add(GetValue(item));
                    }

                    return $"({string.Join(", ", items)})";
                }
                else
                {
                    return GetValue(value);
                }
            }
            else
            {
                return _typeConverters.ContainsKey(type)
                    ? _typeConverters[type](input)
                    : input.ToString();
            }
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Expression.NodeType == ExpressionType.Constant || node.Expression.NodeType == ExpressionType.MemberAccess)
            {
                _fieldNames.Push(node.Member.Name);
                Visit(node.Expression);
            }
            else
            {
                _queryStringBuilder.Append(node.Member.Name);
            }

            return node;
        }
    }


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

        public override string ToString()
        {
            var visitor = new FilterQueryVisitor();
            return visitor.AsQueryString(QuerySpecification.Expression);
        }
    }
}