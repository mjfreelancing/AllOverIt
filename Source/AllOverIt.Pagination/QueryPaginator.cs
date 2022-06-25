using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Pagination.Exceptions;
using AllOverIt.Pagination.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Pagination
{
    public sealed class QueryPaginator<TEntity> : QueryPaginatorBase, IQueryPaginator<TEntity>
        where TEntity : class
    {
        private sealed class FirstExpression
        {
            public bool IsPending => MemberAccess == null;
            public MemberExpression MemberAccess { get; set; }
            public ConstantExpression ReferenceValue { get; set; }
        }

        private readonly List<ColumnDefinition<TEntity>> _columns = new();
        private readonly IQueryable<TEntity> _query;
        private readonly int _pageSize;
        private readonly PaginationDirection _paginationDirection;

        private ContinuationTokenEncoder _continuationTokenEncoder;
        private IOrderedQueryable<TEntity> _directionQuery;                     // based on the _paginationDirection        
        private IOrderedQueryable<TEntity> _directionReverseQuery;              // based on the reverse _direction

        public IContinuationTokenEncoder ContinuationTokenEncoder => GetContinuationTokenEncoder();

        public QueryPaginator(IQueryable<TEntity> query, int pageSize, PaginationDirection paginationDirection = PaginationDirection.Forward)
        {
            _query = query.WhenNotNull(nameof(query));
            _pageSize = pageSize;
            _paginationDirection = paginationDirection;
        }

        public IQueryPaginator<TEntity> ColumnAscending<TProperty>(Expression<Func<TEntity, TProperty>> expression)
        {
            AddColumnDefinition(expression, true);
            return this;
        }

        public IQueryPaginator<TEntity> ColumnDescending<TProperty>(Expression<Func<TEntity, TProperty>> expression)
        {
            AddColumnDefinition(expression, false);
            return this;
        }

        public IQueryable<TEntity> GetPageQuery(string continuationToken = default)
        {
            AssertColumnsDefined();

            // Returns ContinuationToken.None if there is no token - which defaults to Forward
            var decodedToken = GetContinuationTokenEncoder().Decode(continuationToken);

            var requiredDirection = decodedToken == ContinuationToken.None
                ? _paginationDirection
                : decodedToken.Direction;

            var requiredQuery = requiredDirection == _paginationDirection
                ? GetDirectionQuery()
                : GetDirectionReverseQuery();

            var paginatedQuery = requiredQuery.AsQueryable();

            // ContinuationToken.None indicates to get the first page (no token was provided)
            if (decodedToken != ContinuationToken.None)
            {
                // If decodedToken.Values is null/empty the original query is returned
                paginatedQuery = ApplyContinuationToken(paginatedQuery, decodedToken);
            }

            paginatedQuery = paginatedQuery.Take(_pageSize);

            if (requiredDirection == PaginationDirection.Backward)
            {
                // This does wrap the query within an outer select but it saves the caller having to
                // (remember to) reverse the results after they are returned.
                paginatedQuery = paginatedQuery.Reverse();
            }

            return paginatedQuery;
        }

        public IQueryable<TEntity> GetPreviousPageQuery(TEntity reference)
        {
            AssertColumnsDefined();

            var backQuery = GetDirectionReverseQuery().AsQueryable();

            // When reference == null, returns the last page relative to the pagination direction
            if (reference != null)
            {
                var predicate = CreatePreviousPagePredicate(reference);
                backQuery = backQuery.Where(predicate);
            }

            return backQuery
                .Take(_pageSize)
                .Reverse();
        }

        public IQueryable<TEntity> GetNextPageQuery(TEntity reference)
        {
            AssertColumnsDefined();

            var forwardQuery = GetDirectionQuery().AsQueryable();

            // When reference == null, returns the first page relative to the pagination direction
            if (reference != null)
            {
                var predicate = CreateNextPagePredicate(reference);
                forwardQuery = forwardQuery.Where(predicate);
            }

            return forwardQuery.Take(_pageSize);
        }

        public bool HasPreviousPage(TEntity reference)
        {
            AssertColumnsDefined();

            var previousQuery = GetDirectionReverseQuery().AsQueryable();
            var predicate = CreatePreviousPagePredicate(reference);

            return previousQuery.Any(predicate);
        }

        public Task<bool> HasPreviousPageAsync(TEntity reference, Func<IQueryable<TEntity>, Expression<Func<TEntity, bool>>, CancellationToken, Task<bool>> anyResolver,
            CancellationToken cancellationToken)
        {
            AssertColumnsDefined();

            var previousQuery = GetDirectionReverseQuery().AsQueryable();
            var predicate = CreatePreviousPagePredicate(reference);

            return anyResolver.Invoke(previousQuery, predicate, cancellationToken);
        }

        public bool HasNextPage(TEntity reference)
        {
            AssertColumnsDefined();

            var nextQuery = GetDirectionQuery().AsQueryable();
            var predicate = CreateNextPagePredicate(reference);

            return nextQuery.Any(predicate);
        }

        public Task<bool> HasNextPageAsync(TEntity reference, Func<IQueryable<TEntity>, Expression<Func<TEntity, bool>>, CancellationToken, Task<bool>> anyResolver,
            CancellationToken cancellationToken)
        {
            AssertColumnsDefined();

            var nextQuery = GetDirectionQuery().AsQueryable();
            var predicate = CreateNextPagePredicate(reference);

            return anyResolver.Invoke(nextQuery, predicate, cancellationToken);
        }

        private void AddColumnDefinition<TProperty>(Expression<Func<TEntity, TProperty>> propertyExpression, bool isAscending)
        {
            if (_directionQuery != null)
            {
                throw new PaginationException("Additional columns cannot be added once pagination has begun.");
            }

            var fieldOrProperty = propertyExpression.GetFieldOrProperty();

            if (fieldOrProperty is FieldInfo _)
            {
                // EF cannot translate fields, and nor should they be used for exposing the model.
                throw new PaginationException($"Paginated queries using fields is not supported.");
            }

            var property = (PropertyInfo)fieldOrProperty;

            var paginationItem = new ColumnDefinition<TEntity, TProperty>(property, isAscending);

            _columns.Add(paginationItem);
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
            AssertColumnsDefined();

            _continuationTokenEncoder ??= new ContinuationTokenEncoder(_columns, _paginationDirection);

            return _continuationTokenEncoder;
        }

        private IOrderedQueryable<TEntity> GetDirectionBasedQuery(PaginationDirection direction)
        {
            return _columns.Aggregate(
                (IOrderedQueryable<TEntity>) default,
                (currentQuery, nextColumn) => currentQuery == null
                    ? nextColumn.ApplyColumnOrderTo(_query, direction)
                    : nextColumn.ThenApplyColumnOrderTo(currentQuery, direction));
        }

        private IQueryable<TEntity> ApplyContinuationToken(IQueryable<TEntity> paginatedQuery, ContinuationToken continuationToken)
        {
            if (continuationToken.Values.IsNullOrEmpty())
            {
                // The token must be defining the first or last page
                return paginatedQuery;
            }

            // Decode, ensuring to set the correct value type otherwise the expression comparisons may fail
            var referenceValues = continuationToken.Values.AsReadOnlyList();

            var predicate = CreatePaginatedPredicate(continuationToken.Direction, referenceValues);

            return paginatedQuery.Where(predicate);
        }

        private Expression<Func<TEntity, bool>> CreatePreviousPagePredicate(TEntity reference)
        {
            _ = reference.WhenNotNull(nameof(reference));

            var referenceValues = _columns
                .GetColumnValues(reference)
                .AsReadOnlyList();

            return CreatePaginatedPredicate(_paginationDirection.Reverse(), referenceValues);
        }

        private Expression<Func<TEntity, bool>> CreateNextPagePredicate(TEntity reference)
        {
            _ = reference.WhenNotNull(nameof(reference));

            var referenceValues = _columns
                .GetColumnValues(reference)
                .AsReadOnlyList();

            return CreatePaginatedPredicate(_paginationDirection, referenceValues);
        }

        private Expression<Func<TEntity, bool>> CreatePaginatedPredicate(PaginationDirection direction, IReadOnlyList<object> referenceValues)
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

        private static BinaryExpression CreateCompareToExpression(IColumnDefinition entity, MemberExpression memberAccess,
            ConstantExpression comparisonValue, Func<Expression, Expression, BinaryExpression> compareTo)
        {
            var propertyType = entity.Property.PropertyType;

            // Some types require the use of CompareTo() for < and > operations
            if (TryGetComparisonMethodInfo(propertyType, out var compareToMethod))
            {
                MethodCallExpression methodCallExpression;

                if (comparisonValue.Type.IsEnum)
                {
                    // Enum comparisons fail unless converted to an object
                    var objValue = Expression.Convert(comparisonValue, typeof(object));

                    // entity.Property => CompareTo() => objValue
                    methodCallExpression = Expression.Call(memberAccess, compareToMethod, objValue);
                }
                else
                {
                    // entity.Property => CompareTo() => comparisonValue
                    methodCallExpression = Expression.Call(memberAccess, compareToMethod, comparisonValue);
                }

                return compareTo.Invoke(methodCallExpression, ConstantZero);
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
            if (memberExpression.Type.IsNullableType() && memberExpression.Type != targetExpression.Type)
            {
                return Expression.Convert(targetExpression, memberExpression.Type);
            }

            return targetExpression;
        }

        private static Func<Expression, Expression, BinaryExpression> GetComparisonExpression(PaginationDirection direction,
            ColumnDefinition<TEntity> item, bool orEqual)
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

        private void AssertColumnsDefined()
        {
            if (_columns.NotAny())
            {
                throw new PaginationException("At least one column must be defined for pagination.");
            }
        }
    }
}
