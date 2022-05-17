﻿using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Reflection;
using AllOverIt.Serialization.Abstractions;
using KeysetPaginationConsole.KeysetPagination.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace KeysetPaginationConsole.KeysetPagination
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
            _ = references.WhenNotNullOrEmpty(nameof(references));        // Can't create a token when there are no results - TODO: Create a custom exception

            // The first row will be the reference for the previous page and the last row will be the reference for the next page
            var reference = continuationDirection == ContinuationDirection.PreviousPage
               ? references.First()
               : references.Last();

            return Encode(continuationDirection, reference);
        }

        // Allows a continuation token to be created based on an individual reference row
        public string Encode<TEntity>(ContinuationDirection direction, TEntity reference)
            where TEntity : class
        {
            _ = reference.WhenNotNull(nameof(reference));        // Can't create a token when there are no results - TODO: Create a custom exception

            IReadOnlyCollection<ContinuationToken.ValueType> GetValueTypes(TEntity result)
            {
                return GetColumnValues(_columns, result)
                    .SelectAsReadOnlyCollection(value => new ContinuationToken.ValueType
                    {
                        TypeCode = Type.GetTypeCode(value.GetType()),
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
                ValueTypes = tokenValues
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

            return columns.Select(item => ReflectionCache.GetPropertyInfo(referenceType, item.Property.Name).GetValue(reference));
        }
    }
}
