using AllOverIt.Assertion;
using AllOverIt.Reflection;
using System.Collections;
using System.Linq.Expressions;
using System.Text;

namespace AllOverIt.Patterns.Specification.Utils
{
    /// <summary>Converts the expression of an <see cref="ILinqSpecification{TType}"/> to a query-like string.</summary>
    public sealed class LinqSpecificationVisitor : ExpressionVisitor
    {
        private static readonly Dictionary<ExpressionType, string> ExpressionTypeMapping = new()
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

        private static readonly Dictionary<Type, Func<object, string>> TypeValueConverters = new()
        {
            [CommonTypes.StringType] = value => $"'{value}'",
            [CommonTypes.DateTimeType] = value => $"'{((DateTime) value).ToUniversalTime():yyyy-MM-ddTHH:mm:ss.fffZ}'",
            [CommonTypes.BoolType] = value => value.ToString()!
        };

        private Dictionary<Type, Func<object, string>>? _customTypeValueConverters;
        private readonly StringBuilder _queryStringBuilder = new();
        private readonly Stack<string> _fieldNames = new();

        /// <summary>Adds a value converter for a custom type. This converter is used when needing to convert
        /// the value of an expression to a string.</summary>
        /// <param name="type">The custom type.</param>
        /// <param name="converter">The action used to convert the value to a string.</param>
        public void AddTypeValueConverter(Type type, Func<object, string> converter)
        {
            _customTypeValueConverters ??= [];

            _customTypeValueConverters.Add(type, converter);
        }

        /// <summary>Converts the expression of the provided <see cref="ILinqSpecification{TType}"/> to a query-like string.</summary>
        /// <typeparam name="TType">The candidate type the specification applies to.</typeparam>
        /// <param name="specification">The Linq-based specification.</param>
        /// <returns>A query-like string representation of the provided <see cref="ILinqSpecification{TType}"/>.</returns>
        public string AsQueryString<TType>(ILinqSpecification<TType> specification) where TType : class
        {
            _ = specification.WhenNotNull();

            try
            {
                Visit(specification.Expression);

                return _queryStringBuilder.ToString();
            }
            finally
            {
                _queryStringBuilder.Clear();
            }
        }

        /// <inheritdoc />
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Object is not null)
            {
                Visit(node.Object);
            }

            // Such as Contains, StartsWith, EndsWith, ...
            if (node.Object is null)
            {
                _queryStringBuilder.Append($"{node.Method.Name}(");
            }
            else
            {
                _queryStringBuilder.Append($".{node.Method.Name}(");
            }

            if (node.Arguments.Count != 0)
            {
                if (node.Arguments.Count == 1)
                {
                    Visit(node.Arguments[0]);
                }
                else
                {
                    for (var i = 0; i < node.Arguments.Count; i++)
                    {
                        Visit(node.Arguments[i]);

                        if (i != node.Arguments.Count - 1)
                        {
                            _queryStringBuilder.Append(", ");
                        }
                    }
                }
            }

            _queryStringBuilder.Append(')');

            return node;
        }

        /// <inheritdoc />
        protected override Expression VisitUnary(UnaryExpression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.Convert:
                    Visit(node.Operand);
                    return node;

                case ExpressionType.Not:
                    _queryStringBuilder.Append($"{ExpressionTypeMapping[node.NodeType]} ");
                    _queryStringBuilder.Append('(');

                    Visit(node.Operand);

                    _queryStringBuilder.Append(')');

                    return node;

                default:
                    throw new NotSupportedException($"Unsupported unary operator '{node.NodeType}'");
            }
        }

        /// <inheritdoc />
        protected override Expression VisitBinary(BinaryExpression node)
        {
            _queryStringBuilder.Append('(');
            Visit(node.Left);

            _queryStringBuilder.Append($" {ExpressionTypeMapping[node.NodeType]} ");

            Visit(node.Right);
            _queryStringBuilder.Append(')');

            return node;
        }

        /// <inheritdoc />
        protected override Expression VisitConstant(ConstantExpression node)
        {
            var value = GetValue(node.Value!);

            _queryStringBuilder.Append(value);

            return node;
        }

        /// <inheritdoc />
        protected override Expression VisitMember(MemberExpression node)
        {
            switch (node.Expression!.NodeType)
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

        private string GetValue(object input)
        {
            if (input is null)
            {
                return "null";
            }

            var type = input.GetType();

            if (TypeValueConverters.TryGetValue(type, out var converter))
            {
                return converter.Invoke(input);
            }

            if (_customTypeValueConverters?.TryGetValue(type, out var customConverter) ?? false)
            {
                return customConverter.Invoke(input);
            }

            if (type.IsClass && type != CommonTypes.StringType)
            {
                if (input is ICollection collection)
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
                    var fieldName = _fieldNames.Pop();
                    var fieldInfo = type.GetField(fieldName);

                    var value = fieldInfo is null
                        ? type.GetProperty(fieldName)!.GetValue(input)
                        : fieldInfo.GetValue(input);

                    return GetValue(value!);
                }
            }

            return input.ToString()!;
        }
    }
}