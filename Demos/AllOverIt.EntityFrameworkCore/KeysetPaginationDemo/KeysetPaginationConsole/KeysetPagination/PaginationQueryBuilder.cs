using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Reflection;
using AllOverIt.Serialization.Abstractions;

namespace KeysetPaginationConsole.KeysetPagination
{
    public sealed class PaginationQueryBuilder<TEntity> : PaginationQueryBuilderBase
        where TEntity : class
    {
        private sealed class ValueType
        {
            public TypeCode TypeCode { get; init; }
            public object Value { get; init; }
        }

        private sealed class ContinuationTokenProxy
        {
            public IReadOnlyCollection<ValueType> ForwardValues { get; init; }
            public IReadOnlyCollection<ValueType> BackwardValues { get; init; }
        }

        private sealed class FirstExpression
        {
            public bool IsPending => MemberAccess == null;
            public MemberExpression MemberAccess { get; set; }
            public ConstantExpression ReferenceValue { get; set; }
        }

        private readonly List<ColumnItem<TEntity>> _columns = new();
        private readonly IJsonSerializer _jsonSerializer;
        private readonly IQueryable<TEntity> _query;
        private readonly PaginationDirection _direction;
        private readonly int _pageSize;
        private IOrderedQueryable<TEntity> _orderedQuery;


        // TODO: Introduce a factory so the jsonSerializer can be passed down


        public PaginationQueryBuilder(IJsonSerializer jsonSerializer, IQueryable<TEntity> query, PaginationDirection direction,
            int pageSize)
        {
            _jsonSerializer = jsonSerializer.WhenNotNull(nameof(jsonSerializer));
            _query = query.WhenNotNull(nameof(query));
            _direction = direction;
            _pageSize = pageSize;
        }

        public PaginationQueryBuilder<TEntity> ColumnAscending<TProperty>(Expression<Func<TEntity, TProperty>> expression)
        {
            AddColumnItem(expression, true);
            return this;
        }

        public PaginationQueryBuilder<TEntity> ColumnDescending<TProperty>(Expression<Func<TEntity, TProperty>> expression)
        {
            AddColumnItem(expression, false);
            return this;
        }

        private void AddColumnItem<TProperty>(Expression<Func<TEntity, TProperty>> propertyExpression, bool isAscending)
        {
            var property = (PropertyInfo) propertyExpression.GetFieldOrProperty();

            var paginationItem = new ColumnItem<TEntity, TProperty>
            {
                Property = property,
                IsAscending = isAscending
            };

            _columns.Add(paginationItem);
        }





        public IQueryable<TEntity> Build(string continuationToken = default)
        {
            if (_orderedQuery == null)
            {
                if (!_columns.Any())
                {
                    throw new InvalidOperationException("At least one column must be defined for pagination.");
                }

                var orderedQuery = _columns.Aggregate(
                    (IOrderedQueryable<TEntity>) default,
                    (current, next) => current == null
                        ? next.ApplyOrderBy(_query, _direction)
                        : next.ApplyThenOrderBy(current, _direction));

                _orderedQuery = orderedQuery;
            }

            var filteredQuery = ApplyContinuationToken(_orderedQuery, continuationToken);

            return filteredQuery.Take(_pageSize);
        }




        public string CreateContinuationToken(IReadOnlyCollection<TEntity> results)
        {
            _ = results.WhenNotNullOrEmpty(nameof(results));        // Can't create a token when there are no results

            IReadOnlyCollection<ValueType> GetValueTypes(TEntity result)
            {
                return GetColumnValues(_columns, result)
                    .SelectAsReadOnlyCollection(value => new ValueType
                    {
                        TypeCode = Type.GetTypeCode(value.GetType()),
                        Value = value
                    });
            }

            var forwardValues = GetValueTypes(results.Last());
            var backwardValues = GetValueTypes(results.First());

            var proxy = new ContinuationTokenProxy
            {
                ForwardValues = forwardValues,
                BackwardValues = backwardValues,
            };

            return _jsonSerializer.SerializeObject(proxy).ToBase64();
        }






        private IQueryable<TEntity> ApplyContinuationToken(IOrderedQueryable<TEntity> orderedQuery, string continuationToken)
        {
            var filteredQuery = orderedQuery.AsQueryable();

            if (continuationToken.IsNullOrEmpty())
            {
                return filteredQuery;
            }

            var proxy = _jsonSerializer.DeserializeObject<ContinuationTokenProxy>(continuationToken.FromBase64());

            var valueTypes = _direction == PaginationDirection.Forward
                ? proxy.ForwardValues
                : proxy.BackwardValues;

            // Decode, ensuring to set the correct value type otherwise the expression comparisons may fail
            var referenceValues = valueTypes.SelectAsReadOnlyList(valueType => Convert.ChangeType(valueType.Value, valueType.TypeCode));

            var predicate = CreateFilterPredicate(referenceValues);
            return filteredQuery.Where(predicate);
        }





        private Expression<Func<TEntity, bool>> CreateFilterPredicate(IReadOnlyList<object> referenceValues)
        {
            // Fully explained at https://use-the-index-luke.com/sql/partial-results/fetch-next-page
            //
            // row-value syntax is part of the SQL standard and looks like this:
            //
            //    (x, y, ...) > (a, b, ...)
            //
            // But is isn't supported by all databases. Also, row-value does not work for mixed column ordering, such as this pseudocode:
            //
            //    (x > a) AND (y < b)
            //
            // The alternative is a variant that looks like this pseudocode:
            //
            //   (x > a) OR
            //   (x = a AND y > b) OR
            //   (x = a AND y = b AND z > c)
            //
            // With additional consideration for when columns are ascending vs descending (< vs >).
            //
            // A further optimization is to include the first column in an additional leading predicate that allows the database to
            // more efficiently apply an access predicate (typically when the column is indexed), such as this pseudocode:
            //
            //   (x >= a) AND (
            //       (x > a) OR
            //       (x = a AND y > b) OR
            //       (x = a AND y = b AND z > c)
            //   )

            var firstExpression = new FirstExpression();
            
            var param = Expression.Parameter(typeof(TEntity), "entity");    // Represents entity =>

            var finalExpression = CompoundOuterColumnExpressions(param, referenceValues, firstExpression);

            if (_columns.Count > 1)
            {
                // Apply the optimization that converts this pseudocode:
                //
                // (X < a) OR (X = a AND Y < b)
                //
                // To:
                // (X <= a) AND ((X < a) OR (X = a AND Y < b))
                //
                var firstColumn = _columns.First();

                var comparison = GetComparisonExpression(firstColumn, true);

                var accessPredicateClause = CreateCompareToExpression(
                    firstColumn,
                    firstExpression.MemberAccess,
                    firstExpression.ReferenceValue,
                    comparison);

                finalExpression = Expression.And(accessPredicateClause, finalExpression);
            }

            return Expression.Lambda<Func<TEntity, bool>>(finalExpression, param);
        }

        private BinaryExpression CompoundOuterColumnExpressions(ParameterExpression param, IReadOnlyList<object> referenceValues,
            FirstExpression firstExpression)
        {
            var outerExpression = default(BinaryExpression)!;

            // Compound the outer OR expressions
            for (var idx = 0; idx < _columns.Count; idx++)
            {
                // Compound the inner AND expressions
                var innerExpression = CompoundInnerColumnExpressions(param, referenceValues, idx + 1, firstExpression);

                outerExpression = outerExpression == null
                    ? innerExpression
                    : Expression.Or(outerExpression, innerExpression);
            }

            return outerExpression;
        }

        private BinaryExpression CompoundInnerColumnExpressions(ParameterExpression param, IReadOnlyList<object> referenceValues, int columnCount,
            FirstExpression firstExpression)
        {
            var innerExpression = default(BinaryExpression)!;

            // Compound the inner AND expressions
            for (var idx = 0; idx < columnCount; idx++)
            {
                var column = _columns[idx];
                var memberAccess = Expression.MakeMemberAccess(param, column.Property);
                var referenceValue = Expression.Constant(referenceValues[idx]);

                // May be used to apply a predicate optimization
                if (firstExpression.IsPending)
                {
                    firstExpression.MemberAccess = memberAccess;
                    firstExpression.ReferenceValue = referenceValue;
                }

                BinaryExpression columnExpression;

                if (idx == columnCount - 1)
                {
                    // The last column is a 'greater than' or 'less than' comparison
                    var comparison = GetComparisonExpression(column, false);

                    columnExpression = CreateCompareToExpression(
                        column,
                        memberAccess,
                        referenceValue,
                        comparison);
                }
                else
                {
                    // Ensure the value used in the comparison matches the type of the 'memberAccess'
                    var referenceValueExpression = EnsureMatchingType(memberAccess, referenceValue);

                    // All columns, except the last, are 'equal' comparisons
                    columnExpression = Expression.Equal(memberAccess, referenceValueExpression);
                }

                innerExpression = innerExpression == null
                    ? columnExpression
                    : Expression.And(innerExpression, columnExpression);
            }

            return innerExpression;
        }

        private static IEnumerable<object> GetColumnValues(IEnumerable<ColumnItem<TEntity>> columns, object result)
        {
            var referenceType = result.GetType().GetTypeInfo();

            return columns.Select(item => ReflectionCache.GetPropertyInfo(referenceType, item.Property.Name).GetValue(result));
        }

        private static BinaryExpression CreateCompareToExpression(ColumnItem<TEntity> entity, MemberExpression memberAccess,
            ConstantExpression comparisonValue, Func<Expression, Expression, BinaryExpression> compareTo)
        {
            var propertyType = entity.Property.PropertyType;

            // Some types, such as string/Guid, require the use of CompareTo() for < and > operations
            if (TryGetComparisonMethodInfo(propertyType, out var compareToMethod))
            {
                // entity.Property.CompareTo(comparisonValue)
                var methodCallExpression = Expression.Call(memberAccess, compareToMethod, comparisonValue);
                return compareTo.Invoke(methodCallExpression, ConstantZeroExpression);
            }
            else
            {
                // Ensure the value used in the comparison matches the type of the 'memberAccess'
                var comparisonValueExpression = EnsureMatchingType(memberAccess, comparisonValue);
                return compareTo.Invoke(memberAccess, comparisonValueExpression);
            }
        }

        private static Expression EnsureMatchingType(MemberExpression memberExpression, Expression targetExpression)
        {
            // If the member is nullable but the target isn't then apply a conversion to ensure the comparison expressions work
            if (memberExpression.Type.IsGenericNullableType() && memberExpression.Type != targetExpression.Type)
            {
                return Expression.Convert(targetExpression, memberExpression.Type);
            }

            return targetExpression;
        }

        private Func<Expression, Expression, BinaryExpression> GetComparisonExpression(ColumnItem<TEntity> item, bool orEqual)
        {
            var greaterThan = _direction switch
            {
                PaginationDirection.Forward => item.IsAscending,
                PaginationDirection.Backward => !item.IsAscending,
                _ => throw new InvalidOperationException($"Unknown direction {_direction}."),
            };

            return (greaterThan, orEqual) switch
            {
                (true, true) => Expression.GreaterThanOrEqual,
                (true, false) => Expression.GreaterThan,
                (false, true) => Expression.LessThanOrEqual,
                (false, false) => Expression.LessThan,
            };
        }
    }
}
