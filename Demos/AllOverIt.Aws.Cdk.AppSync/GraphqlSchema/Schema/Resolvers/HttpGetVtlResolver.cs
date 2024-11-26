namespace GraphqlSchema.Schema.Resolvers
{
    internal class HttpGetVtlResolver : VtlResolverBase
    {
        public HttpGetVtlResolver(string resource, string apiKey)
        {
            SetHttpRequestMapping("GET", resource, apiKey);
        }
    }
}