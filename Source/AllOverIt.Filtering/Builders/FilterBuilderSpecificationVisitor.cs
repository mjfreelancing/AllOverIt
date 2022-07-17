using AllOverIt.Assertion;
using AllOverIt.Filtering.Filters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace AllOverIt.Filtering.Builders
{
    internal class FilterBuilderSpecificationVisitor : ExpressionVisitor
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

        public string AsQueryString<TType, TFilter>(IFilterBuilder<TType, TFilter> filterBuilder)
       where TType : class
       where TFilter : class, IFilter
        {
            _ = filterBuilder.WhenNotNull(nameof(filterBuilder));

            Visit(filterBuilder.AsSpecification.Expression);

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

            // Such as Contains, StartsWith, EndsWith
            _queryStringBuilder.Append($" {node.Method.Name} ");

            if (node.Arguments.Any())
            {
                if (node.Arguments.Count == 1)
                {
                    Visit(node.Arguments[0]);
                }
                else
                {
                    _queryStringBuilder.Append('(');

                    for (var i = 0; i < node.Arguments.Count; i++)
                    {
                        Visit(node.Arguments[i]);

                        if (i != node.Arguments.Count - 1)
                        {
                            _queryStringBuilder.Append(", ");
                        }
                    }                    

                    _queryStringBuilder.Append(')');
                }
            }

            return node;
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.Convert:
                    Visit(node.Operand);
                    return node;

                case ExpressionType.Not:
                    _queryStringBuilder.Append($" {_expressionOperators[node.NodeType]} ");
                    _queryStringBuilder.Append('(');

                    Visit(node.Operand);

                    _queryStringBuilder.Append(')');

                    return node;

                default:
                    throw new NotSupportedException("Only not(\"!\") unary operator is supported!");
            }
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            _queryStringBuilder.Append('(');
            Visit(node.Left);

            _queryStringBuilder.Append($" {_expressionOperators[node.NodeType]} ");

            Visit(node.Right);
            _queryStringBuilder.Append(')');

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
}