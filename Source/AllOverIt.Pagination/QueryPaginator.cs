using AllOverIt.Assertion;
using AllOverIt.Expressions;
using AllOverIt.Extensions;
using AllOverIt.Pagination.Exceptions;
using AllOverIt.Pagination.Extensions;
using AllOverIt.Pagination.TokenEncoding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Pagination
{
    /// <summary>A keyset paginated query builder that supports ordering multiple columns suitable for forward and backward navigation.</summary>
    /// <typeparam name="TEntity">The entity type that the generated query is based on.</typeparam>
    public sealed class QueryPaginator<TEntity> : QueryPaginatorBase, IQueryPaginator<TEntity>
        where TEntity : class
    {
        private sealed class FirstExpression
        {
            public bool IsPending => MemberAccess == null;
            public MemberExpression MemberAccess { get; set; }
            public Expression ReferenceValue { get; set; }
        }

        private readonly List<ColumnDefinition<TEntity>> _columns = [];
        private readonly QueryPaginatorConfiguration _configuration;
        private readonly IContinuationTokenEncoderFactory _continuationTokenEncoderFactory;

        private IContinuationTokenEncoder _continuationTokenEncoder;
        private IOrderedQueryable<TEntity> _directionQuery;                     // based on the _paginationDirection        
        private IOrderedQueryable<TEntity> _directionReverseQuery;              // based on the reverse _direction

        /// <inheritdoc />
        public IQueryable<TEntity> BaseQuery { get; }

        /// <inheritdoc />
        public IContinuationTokenEncoder TokenEncoder => GetContinuationTokenEncoder();

        /// <summary>Constructor.</summary>
        /// <param name="query">The base query to apply pagination to.</param>
        /// <param name="configuration">Provides paginator options that define how the paginated query will be generated.</param>
        /// <param name="continuationTokenEncoderFactory">A factory to create a continuation token encoder..</param>
        public QueryPaginator(IQueryable<TEntity> query, QueryPaginatorConfiguration configuration, IContinuationTokenEncoderFactory continuationTokenEncoderFactory)
        {
            BaseQuery = query.WhenNotNull(nameof(query));
            _configuration = configuration.WhenNotNull(nameof(configuration));
            _continuationTokenEncoderFactory = continuationTokenEncoderFactory.WhenNotNull(nameof(continuationTokenEncoderFactory));
        }

        /// <summary>A factory method to create a query paginator when not using dependency injection. It is preferable to create a
        /// paginator via <see cref="IQueryPaginatorFactory"/> (and dependency injection) as it will use fewer resources through
        /// the use of internal singleton serializer and encoder factory instances.</summary>
        /// <param name="query">The base query to apply pagination to.</param>
        /// <param name="configuration">Provides paginator options that define how the paginated query will be generated.</param>
        /// <returns>A new query paginator specific to the provided query and configuration.</returns>
        public static QueryPaginator<TEntity> Create(IQueryable<TEntity> query, QueryPaginatorConfiguration configuration)
        {
            var serializerFactory = new ContinuationTokenSerializerFactory();
            var encoderFactory = new ContinuationTokenEncoderFactory(serializerFactory);

            return new QueryPaginator<TEntity>(query, configuration, encoderFactory);
        }

        /// <inheritdoc />
        public PaginationDirection GetQueryDirection(string continuationToken = default)
        {
            // continuationToken can be null
            var decodedToken = TokenEncoder.Decode(continuationToken);

            return decodedToken == ContinuationToken.None
                ? _configuration.PaginationDirection
                : decodedToken.Direction;
        }

        /// <inheritdoc />
        public IQueryPaginator<TEntity> ColumnAscending<TProperty>(Expression<Func<TEntity, TProperty>> expression)
        {
            _ = expression.WhenNotNull(nameof(expression));

            AddColumnDefinition(expression, true);
            return this;
        }

        /// <inheritdoc />
        public IQueryPaginator<TEntity> ColumnDescending<TProperty>(Expression<Func<TEntity, TProperty>> expression)
        {
            _ = expression.WhenNotNull(nameof(expression));

            AddColumnDefinition(expression, false);
            return this;
        }

        /// <inheritdoc />
        public IQueryable<TEntity> GetPageQuery(string continuationToken = default)
        {
            AssertColumnsDefined();

            // Returns ContinuationToken.None if there is no token - which defaults to Forward
            var decodedToken = TokenEncoder.Decode(continuationToken);

            var requiredDirection = decodedToken == ContinuationToken.None
                ? _configuration.PaginationDirection
                : decodedToken.Direction;

            var requiredQuery = requiredDirection == _configuration.PaginationDirection
                ? GetDirectionQuery()
                : GetDirectionReverseQuery();

            // When decodedToken = ContinuationToken.None (decodedToken.Values is null/empty) this indicates
            // to get the first page so the original query is returned.
            var paginatedQuery = ApplyContinuationToken(requiredQuery.AsQueryable(), decodedToken);

            paginatedQuery = paginatedQuery.Take(_configuration.PageSize);

            if (requiredDirection == PaginationDirection.Backward)
            {
                // This does wrap the query within an outer select but it saves the caller having to
                // (remember to) reverse the results after they are returned.
                paginatedQuery = paginatedQuery.Reverse();
            }

            return paginatedQuery;
        }

        /// <inheritdoc />
        public IQueryable<TEntity> GetPreviousPageQuery(TEntity reference)
        {
            // When reference == null, returns the first page relative to the pagination direction

            AssertColumnsDefined();

            var backQuery = GetDirectionReverseQuery().AsQueryable();

            if (reference != null)
            {
                var predicate = CreatePreviousPagePredicate(reference);
                backQuery = backQuery.Where(predicate);
            }

            backQuery = backQuery.Take(_configuration.PageSize);

            if (_configuration.PaginationDirection == PaginationDirection.Forward)
            {
                backQuery = backQuery.Reverse();
            }

            return backQuery;
        }

        /// <inheritdoc />
        public IQueryable<TEntity> GetNextPageQuery(TEntity reference)
        {
            // When reference == null, returns the first page relative to the pagination direction

            AssertColumnsDefined();

            var forwardQuery = GetDirectionQuery().AsQueryable();

            if (reference != null)
            {
                var predicate = CreateNextPagePredicate(reference);
                forwardQuery = forwardQuery.Where(predicate);
            }

            forwardQuery = forwardQuery.Take(_configuration.PageSize);

            if (_configuration.PaginationDirection == PaginationDirection.Backward)
            {
                forwardQuery = forwardQuery.Reverse();
            }

            return forwardQuery;
        }

        /// <inheritdoc />
        public bool HasPreviousPage(TEntity reference)
        {
            _ = reference.WhenNotNull(nameof(reference));

            AssertColumnsDefined();

            var previousQuery = GetDirectionReverseQuery().AsQueryable();
            var predicate = CreatePreviousPagePredicate(reference);

            return previousQuery.Any(predicate);
        }

        /// <inheritdoc />
        public Task<bool> HasPreviousPageAsync(TEntity reference, Func<IQueryable<TEntity>, Expression<Func<TEntity, bool>>, CancellationToken, Task<bool>> anyResolver,
            CancellationToken cancellationToken)
        {
            _ = reference.WhenNotNull(nameof(reference));

            AssertColumnsDefined();

            var previousQuery = GetDirectionReverseQuery().AsQueryable();
            var predicate = CreatePreviousPagePredicate(reference);

            return anyResolver.Invoke(previousQuery, predicate, cancellationToken);
        }

        /// <inheritdoc />
        public bool HasNextPage(TEntity reference)
        {
            _ = reference.WhenNotNull(nameof(reference));

            AssertColumnsDefined();

            var nextQuery = GetDirectionQuery().AsQueryable();
            var predicate = CreateNextPagePredicate(reference);

            return nextQuery.Any(predicate);
        }

        /// <inheritdoc />
        public Task<bool> HasNextPageAsync(TEntity reference, Func<IQueryable<TEntity>, Expression<Func<TEntity, bool>>, CancellationToken, Task<bool>> anyResolver,
            CancellationToken cancellationToken)
        {
            _ = reference.WhenNotNull(nameof(reference));

            AssertColumnsDefined();

            var nextQuery = GetDirectionQuery().AsQueryable();
            var predicate = CreateNextPagePredicate(reference);

            return anyResolver.Invoke(nextQuery, predicate, cancellationToken);
        }

        private void AddColumnDefinition<TProperty>(Expression<Func<TEntity, TProperty>> propertyExpression, bool isAscending)
        {
            Throw<PaginationException>.WhenNotNull(_directionQuery, "Additional columns cannot be added once pagination has begun.");

            var fieldOrProperty = propertyExpression.GetPropertyOrFieldMemberInfo();

            // EF cannot translate fields, and nor should they be used for exposing the model.
            Throw<PaginationException>.When(fieldOrProperty is FieldInfo, "Paginated queries do not support fields.");

            var property = (PropertyInfo)fieldOrProperty;

            var paginationItem = new ColumnDefinition<TEntity, TProperty>(property, isAscending);

            _columns.Add(paginationItem);
        }

        private IOrderedQueryable<TEntity> GetDirectionQuery()
        {
            _directionQuery ??= GetDirectionBasedQuery(_configuration.PaginationDirection);

            return _directionQuery;
        }

        private IOrderedQueryable<TEntity> GetDirectionReverseQuery()
        {
            _directionReverseQuery ??= GetDirectionBasedQuery(_configuration.PaginationDirection.Reverse());

            return _directionReverseQuery;
        }

        private IContinuationTokenEncoder GetContinuationTokenEncoder()
        {
            AssertColumnsDefined();

            _continuationTokenEncoder ??= _continuationTokenEncoderFactory.CreateContinuationTokenEncoder(
                _columns, _configuration.PaginationDirection, _configuration.ContinuationTokenOptions);

            return _continuationTokenEncoder;
        }

        private IOrderedQueryable<TEntity> GetDirectionBasedQuery(PaginationDirection direction)
        {
            return _columns.Aggregate(
                (IOrderedQueryable<TEntity>) default,
                (currentQuery, nextColumn) => currentQuery == null
                    ? nextColumn.ApplyColumnOrderTo(BaseQuery, direction)
                    : nextColumn.ThenApplyColumnOrderTo(currentQuery, direction));
        }

        private IQueryable<TEntity> ApplyContinuationToken(IQueryable<TEntity> paginatedQuery, IContinuationToken continuationToken)
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
            var referenceValues = _columns
                .GetColumnValues(reference)
                .AsReadOnlyList();

            return CreatePaginatedPredicate(_configuration.PaginationDirection.Reverse(), referenceValues);
        }

        private Expression<Func<TEntity, bool>> CreateNextPagePredicate(TEntity reference)
        {
            var referenceValues = _columns
                .GetColumnValues(reference)
                .AsReadOnlyList();

            return CreatePaginatedPredicate(_configuration.PaginationDirection, referenceValues);
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
            var referenceParameterCache = new Dictionary<int, Expression>();

            var param = Expression.Parameter(typeof(TEntity), "entity");    // Represents entity =>

            var finalExpression = CompoundOuterColumnExpressions(direction, param, referenceValues, firstExpression, referenceParameterCache);

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

        private Expression CompoundOuterColumnExpressions(PaginationDirection direction, ParameterExpression param, IReadOnlyList<object> referenceValues,
            FirstExpression firstExpression, IDictionary<int, Expression> referenceParameterCache)
        {
            Expression outerExpression = default;

            // Compound the outer OR expressions
            for (var idx = 0; idx < _columns.Count; idx++)
            {
                // Compound the inner AND expressions
                var innerExpression = CompoundInnerColumnExpressions(direction, param, referenceValues, idx + 1, firstExpression, referenceParameterCache);

                outerExpression = outerExpression == null
                    ? innerExpression
                    : Expression.Or(outerExpression, innerExpression);
            }

            return outerExpression;
        }

        private Expression CompoundInnerColumnExpressions(PaginationDirection direction, ParameterExpression param, IReadOnlyList<object> referenceValues,
            int columnCount, FirstExpression firstExpression, IDictionary<int, Expression> referenceParameterCache)
        {
            Expression innerExpression = default;

            // Compound the inner AND expressions
            for (var idx = 0; idx < columnCount; idx++)
            {
                var column = _columns[idx];
                var memberAccess = Expression.MakeMemberAccess(param, column.Property);
                var referenceValue = CreateReferenceParameter(referenceValues, idx, column.Property.PropertyType, referenceParameterCache);

                // May be used to apply a predicate optimization
                if (firstExpression.IsPending)
                {
                    firstExpression.MemberAccess = memberAccess;
                    firstExpression.ReferenceValue = referenceValue;
                }

                Expression columnExpression;

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

        private static Expression CreateCompareToExpression(IColumnDefinition entity, MemberExpression memberAccess,
            Expression comparisonValue, Func<Expression, Expression, Expression> compareTo)
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

                return compareTo.Invoke(methodCallExpression, ExpressionConstants.Zero);
            }
            else
            {
                // Ensure the value used in the comparison matches the type of the 'memberAccess'
                var comparisonValueExpression = EnsureMatchingType(memberAccess, comparisonValue);
                return compareTo.Invoke(memberAccess, comparisonValueExpression);
            }
        }

        private Expression CreateReferenceParameter(IReadOnlyList<object> referenceValues, int index, Type valueType,
            IDictionary<int, Expression> referenceParameterCache)
        {
            if (referenceParameterCache.TryGetValue(index, out var expression))
            {
                return expression;
            }

            var value = referenceValues[index];

            if (!_configuration.UseParameterizedQueries)
            {
                return Expression.Constant(value);
            }

            // Create an expression that will result in a parameterized query being generated (typically for EF queries).
            // The benefit here is removing the risk of SQL injection as well as keeping the EF cache optimized.
            expression = ExpressionUtils.CreateParameterizedValue(value, valueType);

            referenceParameterCache.Add(index, expression);

            return expression;
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
            var greaterThan = direction == PaginationDirection.Forward
                ? item.IsAscending
                : !item.IsAscending;

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
            Throw<PaginationException>.When(_columns.NotAny(), "At least one column must be defined for pagination.");
        }
    }
}
