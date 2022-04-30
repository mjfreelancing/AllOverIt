using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace EnrichedEnumModelBinding.Problems
{
    public abstract class ProblemBase : StatusCodeProblemDetails
    {
        // Same as the key used by Hellang.Middleware
        private const string TraceIdKey = "traceId";

        protected ProblemBase(int statusCode)
            : base(statusCode)
        {
        }

        protected void TryAddTraceId(HttpContext httpContext)
        {
            if (!Extensions.ContainsKey(TraceIdKey))
            {
                // Same approach used by Hellang.Middleware
                Extensions.Add(TraceIdKey, Activity.Current?.Id ?? httpContext.TraceIdentifier);
            }
        }
    }
}