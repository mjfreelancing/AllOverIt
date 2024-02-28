namespace GraphqlSchema.Schema.Resolvers.Query
{
    internal sealed class LanguageResolver : RequestResponseResolverBase
    {
        public LanguageResolver()
        {
            SetHttpRequestMapping("GET", "/language");
        }
    }
}