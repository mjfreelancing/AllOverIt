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

        private readonly List<ColumnItem<TEntity>> _columns = new();

        private readonly IJsonSerializer _jsonSerializer;
        private readonly IQueryable<TEntity> _query;
        private readonly PaginationDirection _direction;
        private readonly int _pageSize;
        private readonly string _continuationToken;

        // TODO: Introduce a factory so the jsonSerializer can be passed down

        public PaginationQueryBuilder(IJsonSerializer jsonSerializer, IQueryable<TEntity> query, PaginationDirection direction,
            int pageSize, string continuationToken)
        {
            _jsonSerializer = jsonSerializer.WhenNotNull(nameof(jsonSerializer));
            _query = query.WhenNotNull(nameof(query));
            _direction = direction;
            _pageSize = pageSize;
            _continuationToken = continuationToken;     // can be null/empty
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

        public IQueryable<TEntity> Build()
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

            var filteredQuery = ApplyContinuationToken(orderedQuery, _continuationToken);

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

        // TODO: This can be static if we don't store the continuation
        private IQueryable<TEntity> ApplyContinuationToken(IOrderedQueryable<TEntity> orderedQuery, string _continuationToken)
        {
            var filteredQuery = orderedQuery.AsQueryable();

            if (_continuationToken.IsNullOrEmpty())
            {
                return filteredQuery;
            }

            var proxy = _jsonSerializer.DeserializeObject<ContinuationTokenProxy>(_continuationToken.FromBase64());

            var valueTypes = _direction == PaginationDirection.Forward
                ? proxy.ForwardValues
                : proxy.BackwardValues;

            // Decode, ensuring to set the correct value type otherwise the expression comparisons may fail
            var referenceValues = valueTypes.SelectAsReadOnlyCollection(valueType => Convert.ChangeType(valueType.Value, valueType.TypeCode));

            var predicate = CreateFilterPredicate(referenceValues);
            return filteredQuery.Where(predicate);
        }

        private Expression<Func<TEntity, bool>> CreateFilterPredicate(IReadOnlyCollection<object> referenceValues)
        {
            // A composite keyset pagination in sql looks something like this:
            //   (x, y, ...) > (a, b, ...)
            // Where, x/y/... represent the column and a/b/... represent the reference's respective values.
            //
            // In sql standard this syntax is called "row value". Check here: https://use-the-index-luke.com/sql/partial-results/fetch-next-page#sb-row-values
            // Unfortunately, not all databases support this properly.
            // Further, if we were to use this we would somehow need EF Core to recognise it and translate it
            // perhaps by using a new DbFunction (https://docs.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbfunctions).
            // There's an ongoing issue for this here: https://github.com/dotnet/efcore/issues/26822
            //
            // In addition, row value won't work for mixed ordered columns. i.e if x > a but y < b.
            // So even if we can use it we'll still have to fallback to this logic in these cases.
            //
            // The generalized expression for this in pseudocode is:
            //   (x > a) OR
            //   (x = a AND y > b) OR
            //   (x = a AND y = b AND z > c) OR...
            //
            // Of course, this will be a bit more complex when ASC and DESC are mixed.
            // Assume x is ASC, y is DESC, and z is ASC:
            //   (x > a) OR
            //   (x = a AND y < b) OR
            //   (x = a AND y = b AND z > c) OR...
            //
            // An optimization is to include an additional redundant wrapping clause for the 1st column when there are
            // more than one column we're acting on, which would allow the db to use it as an access predicate on the 1st column.
            // See here: https://use-the-index-luke.com/sql/partial-results/fetch-next-page#sb-equivalent-logic

            var firstMemberAccessExpression = default(MemberExpression);
            var firstReferenceValueExpression = default(ConstantExpression);

            // entity =>
            var param = Expression.Parameter(typeof(TEntity), "entity");

            var orExpression = default(BinaryExpression)!;
            var innerLimit = 1;
            // This loop compounds the outer OR expressions.
            for (var i = 0; i < _columns.Count; i++)
            {
                var andExpression = default(BinaryExpression)!;

                // This loop compounds the inner AND expressions.
                // innerLimit implicitly grows from 1 to columns.Count by each iteration.
                for (var j = 0; j < innerLimit; j++)
                {
                    var isInnerLastOperation = j + 1 == innerLimit;
                    var item = _columns.ElementAt(j);
                    var memberAccess = Expression.MakeMemberAccess(param, item.Property);
                    var referenceValueExpression = Expression.Constant(referenceValues.ElementAt(j));

                    if (firstMemberAccessExpression == null)
                    {
                        // This might be used later on in an optimization.
                        firstMemberAccessExpression = memberAccess;
                        firstReferenceValueExpression = referenceValueExpression;
                    }

                    BinaryExpression innerExpression;

                    if (!isInnerLastOperation)
                    {
                        innerExpression = Expression.Equal(
                            memberAccess,
                            EnsureMatchingType(memberAccess, referenceValueExpression));
                    }
                    else
                    {
                        var compare = GetComparisonExpression(item, false);

                        innerExpression = MakeComparisonExpression(
                            item,
                            memberAccess, referenceValueExpression,
                            compare);
                    }

                    andExpression = andExpression == null ? innerExpression : Expression.And(andExpression, innerExpression);
                }

                orExpression = orExpression == null ? andExpression : Expression.Or(orExpression, andExpression);

                innerLimit++;
            }

            var finalExpression = orExpression;

            if (_columns.Count > 1)
            {
                // Implement the optimization that allows an access predicate on the 1st column.
                // This is done by generating the following expression:
                //   (x >=|<= a) AND (previous generated expression)
                //
                // This effectively adds a redundant clause on the 1st column, but it's a clause all dbs
                // understand and can use as an access predicate (most commonly when the column is indexed).

                var firstItem = _columns.First();

                var compare = GetComparisonExpression(firstItem, true);

                var accessPredicateClause = MakeComparisonExpression(
                    firstItem,
                    firstMemberAccessExpression!, firstReferenceValueExpression!,
                    compare);

                finalExpression = Expression.And(accessPredicateClause, finalExpression);
            }

            return Expression.Lambda<Func<TEntity, bool>>(finalExpression, param);
        }

        private static IEnumerable<object> GetColumnValues(IEnumerable<ColumnItem<TEntity>> columns, object result)
        {
            var referenceType = result.GetType().GetTypeInfo();

            return columns.Select(item => ReflectionCache.GetPropertyInfo(referenceType, item.Property.Name).GetValue(result));
        }

        private static BinaryExpression MakeComparisonExpression(ColumnItem<TEntity> entity, MemberExpression memberAccess, ConstantExpression comparisonValue,
            Func<Expression, Expression, BinaryExpression> compareTo)
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
                var comparisonExpression = EnsureMatchingType(memberAccess, comparisonValue);
                return compareTo.Invoke(memberAccess, comparisonExpression);
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
