﻿using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Serialization.Abstractions;
using KeysetPaginationConsole.KeysetPagination.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace KeysetPaginationConsole.KeysetPagination
{
    public sealed class PaginationQueryBuilder<TEntity> : PaginationQueryBuilderBase
        where TEntity : class
    {
        private sealed class FirstExpression
        {
            public bool IsPending => MemberAccess == null;
            public MemberExpression MemberAccess { get; set; }
            public ConstantExpression ReferenceValue { get; set; }
        }

        private readonly List<ColumnItem<TEntity>> _columns = new();
        private readonly IJsonSerializer _jsonSerializer;
        private readonly IQueryable<TEntity> _query;
        private readonly PaginationDirection _paginationDirection;
        private readonly int _pageSize;
        
        private ContinuationTokenEncoder _continuationTokenEncoder;
        private ContinuationTokenEncoder ContinuationTokenEncoder => GetContinuationTokenEncoder();

        // A cached query based on the _direction
        private IOrderedQueryable<TEntity> _directionQuery;
        private IOrderedQueryable<TEntity> DirectionQuery => GetDirectionQuery();

        // A cached query based on the reverse _direction
        private IOrderedQueryable<TEntity> _directionReverseQuery;
        private IOrderedQueryable<TEntity> DirectionReverseQuery => GetDirectionReverseQuery();



        // TODO: Introduce a factory so the jsonSerializer can be passed down


        public PaginationQueryBuilder(IJsonSerializer jsonSerializer, IQueryable<TEntity> query, PaginationDirection direction, int pageSize)
        {
            _jsonSerializer = jsonSerializer.WhenNotNull(nameof(jsonSerializer));
            _query = query.WhenNotNull(nameof(query));
            _paginationDirection = direction;
            _pageSize = pageSize;

        }

        // **************** ?? Create static factory methods to build a suitable builder ?? ****************





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
            if (!_columns.Any())
            {
                throw new InvalidOperationException("At least one column must be defined for pagination.");
            }

            // Returns ContinuationToken.None if there is no token - which defaults to Forward
            var continuationProxy = ContinuationTokenEncoder.Decode(continuationToken);

            var requiredDirection = continuationProxy != ContinuationToken.None
                ? continuationProxy.Direction
                : _paginationDirection;

            var orderedQuery = requiredDirection == _paginationDirection
                ? DirectionQuery
                : DirectionReverseQuery;

            var filteredQuery = orderedQuery.AsQueryable();

            // There's no reference values when ContinuationToken.None so just get the first page
            if (continuationProxy != ContinuationToken.None)
            {
                filteredQuery = ApplyContinuationToken(filteredQuery, continuationProxy);
            }

            filteredQuery = filteredQuery.Take(_pageSize);

            if (requiredDirection == PaginationDirection.Backward)
            {
                // This does wrap the query within an outer select but it saves the caller having to (remember to)
                // reverse the results after they are returned and it also means we don't need to be concerned with
                // asynchronous yielding.
                filteredQuery = filteredQuery.Reverse();
            }

            return filteredQuery;
        }

        // The caller can create a previous/next page token as desired - the first/last row is selected based on the direction
        public string CreateContinuationToken(ContinuationDirection direction, IReadOnlyCollection<TEntity> references)
        {
            _ = references.WhenNotNullOrEmpty(nameof(references));        // Can't create a token when there are no results - TODO: Create a custom exception

            return ContinuationTokenEncoder.Encode(direction, references);
        }

        // Allows a continuation token to be created based on an individual reference row
        public string CreateContinuationToken(ContinuationDirection direction, TEntity reference)
        {
            _ = reference.WhenNotNull(nameof(reference));        // Can't create a token when there are no results - TODO: Create a custom exception

            return ContinuationTokenEncoder.Encode(direction, reference);
        }

        private IOrderedQueryable<TEntity> GetDirectionQuery()
        {
            _directionQuery ??= GetDirectionBasedQuery(_paginationDirection);

            return _directionQuery;
        }

        private IOrderedQueryable<TEntity> GetDirectionReverseQuery()
        {
            _directionReverseQuery ??= GetDirectionBasedQuery(_paginationDirection.Reverse());

            return _directionReverseQuery;
        }

        private ContinuationTokenEncoder GetContinuationTokenEncoder()
        {
            _continuationTokenEncoder ??= new ContinuationTokenEncoder(_columns, _paginationDirection, _jsonSerializer);
            return _continuationTokenEncoder;
        }

        private IOrderedQueryable<TEntity> GetDirectionBasedQuery(PaginationDirection direction)
        {
            return _columns.Aggregate(
                (IOrderedQueryable<TEntity>) default,
                (current, next) => current == null
                    ? next.ApplyOrderBy(_query, direction)
                    : next.ApplyThenOrderBy(current, direction));
        }

        private IQueryable<TEntity> ApplyContinuationToken(IQueryable<TEntity> filteredQuery, ContinuationToken continuationToken)
        {
            // Decode, ensuring to set the correct value type otherwise the expression comparisons may fail
            var referenceValues = continuationToken.ValueTypes.SelectAsReadOnlyList(valueType => Convert.ChangeType(valueType.Value, valueType.TypeCode));

            var predicate = CreateFilterPredicate(continuationToken.Direction, referenceValues);

            return filteredQuery.Where(predicate);
        }

        private Expression<Func<TEntity, bool>> CreateFilterPredicate(PaginationDirection direction, IReadOnlyList<object> referenceValues)
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

            var finalExpression = CompoundOuterColumnExpressions(direction, param, referenceValues, firstExpression);

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

                var comparison = GetComparisonExpression(direction, firstColumn, true);

                var accessPredicateClause = CreateCompareToExpression(
                    firstColumn,
                    firstExpression.MemberAccess,
                    firstExpression.ReferenceValue,
                    comparison);

                finalExpression = Expression.And(accessPredicateClause, finalExpression);
            }

            return Expression.Lambda<Func<TEntity, bool>>(finalExpression, param);
        }

        private BinaryExpression CompoundOuterColumnExpressions(PaginationDirection direction, ParameterExpression param, IReadOnlyList<object> referenceValues,
            FirstExpression firstExpression)
        {
            var outerExpression = default(BinaryExpression)!;

            // Compound the outer OR expressions
            for (var idx = 0; idx < _columns.Count; idx++)
            {
                // Compound the inner AND expressions
                var innerExpression = CompoundInnerColumnExpressions(direction, param, referenceValues, idx + 1, firstExpression);

                outerExpression = outerExpression == null
                    ? innerExpression
                    : Expression.Or(outerExpression, innerExpression);
            }

            return outerExpression;
        }

        private BinaryExpression CompoundInnerColumnExpressions(PaginationDirection direction, ParameterExpression param, IReadOnlyList<object> referenceValues,
            int columnCount, FirstExpression firstExpression)
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
                    var comparison = GetComparisonExpression(direction, column, false);

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

        //private static IEnumerable<object> GetColumnValues(IEnumerable<IColumnItem> columns, object reference)
        //{
        //    var referenceType = reference.GetType().GetTypeInfo();

        //    return columns.Select(item => ReflectionCache.GetPropertyInfo(referenceType, item.Property.Name).GetValue(reference));
        //}

        private static BinaryExpression CreateCompareToExpression(ColumnItem<TEntity> entity, MemberExpression memberAccess,
            ConstantExpression comparisonValue, Func<Expression, Expression, BinaryExpression> compareTo)
        {
            var propertyType = entity.Property.PropertyType;

            // Some types require the use of CompareTo() for < and > operations
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

        private static Func<Expression, Expression, BinaryExpression> GetComparisonExpression(PaginationDirection direction,
            ColumnItem<TEntity> item, bool orEqual)
        {
            var greaterThan = direction switch
            {
                PaginationDirection.Forward => item.IsAscending,
                PaginationDirection.Backward => !item.IsAscending,
                _ => throw new InvalidOperationException($"Unknown direction {direction}."),
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
