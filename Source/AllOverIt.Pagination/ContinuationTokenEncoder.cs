using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Pagination.Extensions;
using AllOverIt.Serialization.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Pagination
{
    internal sealed class ContinuationTokenEncoder
    {
        private readonly IReadOnlyCollection<IColumnDefinition> _columns;
        private readonly PaginationDirection _paginationDirection;
        private readonly IJsonSerializer _jsonSerializer;

        public ContinuationTokenEncoder(IEnumerable<IColumnDefinition> columns, PaginationDirection paginationDirection, IJsonSerializer jsonSerializer)
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

            // Determine the page direction that needs to be used in order to get the required next/previous page
            var continuationPageDirection = direction == ContinuationDirection.PreviousPage
                ? _paginationDirection.Reverse()
                : _paginationDirection;

            // Get the reference column values and their types
            var columnValues = _columns.GetColumnValueTypes(reference);

            // Serialize the resultant token information
            var continuationToken = new ContinuationToken
            {
                Direction = continuationPageDirection,
                Values = columnValues
            };

            return _jsonSerializer.SerializeObject(continuationToken).ToBase64();
        }

        public string EncodeFirstPage()
        {
            return Encode(ContinuationDirection.NextPage);
        }

        public string EncodeLastPage()
        {
            return Encode(ContinuationDirection.PreviousPage);
        }

        public ContinuationToken Decode(string continuationToken)
        {
            return continuationToken.IsNotNullOrEmpty()
                ? _jsonSerializer.DeserializeObject<ContinuationToken>(continuationToken.FromBase64())
                : ContinuationToken.None;
        }

        // Allows a continuation token to be created that represents the first or last page, relative to the pagination direction
        private string Encode(ContinuationDirection direction)
        {
            if (direction == ContinuationDirection.NextPage)
            {
                return string.Empty;        // Could also have been null
            }

            // Determine the page direction that needs to be used in order to get the required next/previous page
            var continuationPageDirection = direction == ContinuationDirection.PreviousPage
                ? _paginationDirection.Reverse()
                : _paginationDirection;

            // Serialize the resultant token information
            var continuationToken = new ContinuationToken
            {
                Direction = continuationPageDirection,
                //Values = 
            };

            return _jsonSerializer.SerializeObject(continuationToken).ToBase64();
        }
    }
}
