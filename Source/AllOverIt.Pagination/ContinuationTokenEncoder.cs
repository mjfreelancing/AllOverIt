using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Pagination.Exceptions;
using AllOverIt.Pagination.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Pagination
{
    internal sealed class ContinuationTokenEncoder : IContinuationTokenEncoder
    {
        private readonly IReadOnlyCollection<IColumnDefinition> _columns;
        private readonly PaginationDirection _paginationDirection;
        
        public IContinuationTokenSerializer Serializer { get; }

        internal ContinuationTokenEncoder(IReadOnlyCollection<IColumnDefinition> columns, PaginationDirection paginationDirection,
            IContinuationTokenSerializer continuationTokenSerializer)
        {
            _columns = columns.WhenNotNullOrEmpty(nameof(columns)).AsReadOnlyCollection();
            _paginationDirection = paginationDirection;
            Serializer = continuationTokenSerializer.WhenNotNull(nameof(continuationTokenSerializer));
        }

        public string EncodePreviousPage<TEntity>(IReadOnlyCollection<TEntity> references) where TEntity : class
        {
            // Encode() checks for null/empty
            return Encode(ContinuationDirection.PreviousPage, references);
        }

        public string EncodeNextPage<TEntity>(IReadOnlyCollection<TEntity> references) where TEntity : class
        {
            // Encode() checks for null/empty
            return Encode(ContinuationDirection.NextPage, references);
        }

        public string EncodePreviousPage(object reference)
        {
            return Encode(ContinuationDirection.PreviousPage, reference);
        }

        public string EncodeNextPage(object reference)
        {
            return Encode(ContinuationDirection.NextPage, reference);
        }

        public string EncodeFirstPage()
        {
            return string.Empty;        // Could also have been null
        }

        public string EncodeLastPage()
        {
            // The decode process implicitly interprets null Values as requiring the last page
            var continuationToken = new ContinuationToken
            {
                Direction = _paginationDirection.Reverse(),
                //Values = 
            };

            return this.Encode(continuationToken);
        }

        private string Encode<TEntity>(ContinuationDirection continuationDirection, IReadOnlyCollection<TEntity> references)
            where TEntity : class
        {
            if (references.IsNullOrEmpty())
            {
                throw new PaginationException("At least one reference entity is required to create a continuation token.");
            }

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

        private string Encode(ContinuationDirection direction, object reference)
        {
            if (reference == null)
            {
                throw new PaginationException("A reference entity is required to create a continuation token.");
            }

            // Determine the page direction that needs to be used in order to get the required next/previous page
            var continuationPageDirection = direction == ContinuationDirection.PreviousPage
                ? _paginationDirection.Reverse()
                : _paginationDirection;

            // Get the reference column values and their types
            var columnValues = _columns.GetColumnValues(reference);

            // Serialize the resultant token information
            var continuationToken = new ContinuationToken
            {
                Direction = continuationPageDirection,
                Values = columnValues
            };

            return this.Encode(continuationToken);
        }
    }
}
