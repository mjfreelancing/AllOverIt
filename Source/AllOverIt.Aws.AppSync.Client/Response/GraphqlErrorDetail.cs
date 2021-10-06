using System.Collections.Generic;

namespace AllOverIt.Aws.AppSync.Client.Response
{
    /// <summary>Contains detailed error information reported by AppSync.</summary>
    public sealed record GraphqlErrorDetail
    {
        /// <summary>The graphql error code, if applicable for the error reported.</summary>
        public int? ErrorCode { get; set; }

        /// <summary>The graphql error type.</summary>
        public string ErrorType { get; set; }

        /// <summary>A description for the reported error.</summary>
        public string Message { get; set; }

        /// <summary>When provided, this describes the location of the error within a query.</summary>
        public IEnumerable<GraphqlLocation> Locations { get; set; }

        /// <summary>When provided, this describes the node path within the query that caused the error.</summary>
        public IEnumerable<object> Path { get; set; }
    }
}