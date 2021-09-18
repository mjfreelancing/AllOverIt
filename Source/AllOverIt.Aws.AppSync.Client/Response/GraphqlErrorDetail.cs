﻿using System.Collections.Generic;

namespace AllOverIt.Aws.AppSync.Client.Response
{
    public sealed record GraphqlErrorDetail
    {
        public int? ErrorCode { get; init; }
        public string ErrorType { get; init; }
        public string Message { get; init; }
        public IEnumerable<GraphqlLocation> Locations { get; init; }
        public IEnumerable<object> Path { get; init; }
    }
}