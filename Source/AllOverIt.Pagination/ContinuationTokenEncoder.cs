using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Pagination.Extensions;
using AllOverIt.Reflection;
using AllOverIt.Serialization.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AllOverIt.Pagination
{
    internal sealed class ContinuationTokenEncoder
    {
        private readonly IReadOnlyCollection<IColumnItem> _columns;
        private readonly PaginationDirection _paginationDirection;
        private readonly IJsonSerializer _jsonSerializer;

        public ContinuationTokenEncoder(IEnumerable<IColumnItem> columns, PaginationDirection paginationDirection, IJsonSerializer jsonSerializer)
        {
            _columns = columns.WhenNotNullOrEmpty(nameof(columns)).AsReadOnlyCollection();
            _paginationDirection = paginationDirection;
            _jsonSerializer = jsonSerializer.WhenNotNull(nameof(jsonSerializer));
        }

        // The caller can create a previous/next page token as desired - the first/last row is selected based on the direction
        public string Encode<TEntity>(ContinuationDirection continuationDirection, IReadOnlyCollection<TEntity> references)
            where TEntity : class
        {
            // Should have been asserted by QueryPaginator
            _ = references.WhenNotNullOrEmpty(nameof(references));

            //TEntity reference;

            //if (_paginationDirection == PaginationDirection.Forward)
            //{
            //    reference = continuationDirection == ContinuationDirection.NextPage
            //       ? references.Last()
            //       : references.First();
            //}
            //else
            //{
            //    reference = continuationDirection == ContinuationDirection.NextPage
            //       ? references.First()
            //       : references.Last();
            //}

            // Determine the required reference to use based on the pagination direction and the continuation direction
            var reference = (_paginationDirection, continuationDirection) switch
            {
                (PaginationDirection.Forward, ContinuationDirection.NextPage) => references.Last(),
                (PaginationDirection.Forward, ContinuationDirection.PreviousPage) => references.First(),
                (PaginationDirection.Backward, ContinuationDirection.NextPage) => references.First(),
                (PaginationDirection.Backward, ContinuationDirection.PreviousPage) => references.Last(),
                _ => throw new InvalidOperationException($"Unknown pagination / continuation combination: {_paginationDirection} / {continuationDirection}")
            };

            return Encode(continuationDirection, reference);
        }

        // Allows a continuation token to be created based on an individual reference row
        public string Encode<TEntity>(ContinuationDirection direction, TEntity reference)
            where TEntity : class
        {
            // Should have been asserted by QueryPaginator
            _ = reference.WhenNotNull(nameof(reference));

            IReadOnlyCollection<ContinuationToken.ValueType> GetValueTypes(TEntity result)
            {
                return GetColumnValues(_columns, result)
                    .SelectAsReadOnlyCollection(value => new ContinuationToken.ValueType
                    {
                        Type = Type.GetTypeCode(value.GetType()),
                        Value = value
                    });
            }

            // Get the reference column values and their types
            var tokenValues = GetValueTypes(reference);

            // Determine the page direction that needs to be used in order to get the required next/previous page
            var continuationPageDirection = direction == ContinuationDirection.PreviousPage
                ? _paginationDirection.Reverse()
                : _paginationDirection;

            // Serialize the resultant token information
            var proxy = new ContinuationToken
            {
                Direction = continuationPageDirection,
                Values = tokenValues
            };

            return _jsonSerializer.SerializeObject(proxy).ToBase64();
        }

        public ContinuationToken Decode(string continuationToken)
        {
            return continuationToken.IsNotNullOrEmpty()
                ? _jsonSerializer.DeserializeObject<ContinuationToken>(continuationToken.FromBase64())
                : ContinuationToken.None;
        }

        private static IEnumerable<object> GetColumnValues(IEnumerable<IColumnItem> columns, object reference)
        {
            var referenceType = reference.GetType().GetTypeInfo();

            return columns.Select(column => ReflectionCache.GetPropertyInfo(referenceType, column.Property.Name).GetValue(reference));
        }
    }
}
