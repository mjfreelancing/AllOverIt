using AllOverIt.Assertion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace AllOverIt.Patterns.Specification.Utils
{
    public sealed class LinqSpecificationVisitor : ExpressionVisitor
    {
        private static readonly IDictionary<ExpressionType, string> _expressionTypeMapping = new Dictionary<ExpressionType, string>
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

        private static readonly IDictionary<Type, Func<object, string>> _valueConverters = new Dictionary<Type, Func<object, string>>
        {
            [typeof(string)] = value => $"'{value}'",
            [typeof(DateTime)] = value => $"'{((DateTime) value).ToUniversalTime():yyyy-MM-ddTHH:mm:ss.fffZ}'",
            [typeof(bool)] = value => value.ToString()
        };

        private readonly StringBuilder _queryStringBuilder = new();
        private readonly Stack<string> _fieldNames = new();

        public string AsQueryString<TType>(ILinqSpecification<TType> specification) where TType : class
        {
            _ = specification.WhenNotNull(nameof(specification));

            Visit(specification.Expression);

            var result = _queryStringBuilder.ToString();

            _queryStringBuilder.Clear();

            return result;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Object is not null)
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
                    _queryStringBuilder.Append($" {_expressionTypeMapping[node.NodeType]} ");
                    _queryStringBuilder.Append('(');

                    Visit(node.Operand);

                    _queryStringBuilder.Append(')');

                    return node;

                default:
                    throw new NotSupportedException($"Unsupported unary operator '{node.NodeType}'");
            }
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            _queryStringBuilder.Append('(');
            Visit(node.Left);

            _queryStringBuilder.Append($" {_expressionTypeMapping[node.NodeType]} ");

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

                if (value is ICollection collection)
                {
                    var items = new List<string>();

                    foreach (var item in collection)
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
                return _valueConverters.TryGetValue(type, out var converter)
                    ? converter.Invoke(input)
                    : input.ToString();
            }
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            switch (node.Expression.NodeType)
            {
                case ExpressionType.Constant:
                case ExpressionType.MemberAccess:
                    _fieldNames.Push(node.Member.Name);
                    Visit(node.Expression);
                    break;

                default:
                    _queryStringBuilder.Append(node.Member.Name);
                    break;
            }

            return node;
        }
    }
}