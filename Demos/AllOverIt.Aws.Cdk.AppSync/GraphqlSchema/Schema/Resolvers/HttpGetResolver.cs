namespace GraphqlSchema.Schema.Resolvers
{
    internal class HttpGetResolver : RequestResponseResolverBase
    {
        public HttpGetResolver(string resource, string apiKey)
        {
            SetHttpRequestMapping("GET", resource, apiKey);
        }
    }
}