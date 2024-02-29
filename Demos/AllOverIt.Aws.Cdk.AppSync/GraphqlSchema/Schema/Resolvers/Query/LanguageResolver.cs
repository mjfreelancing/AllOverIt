namespace GraphqlSchema.Schema.Resolvers.Query
{
    internal sealed class LanguageResolver : VtlResolverBase
    {
        public LanguageResolver()
        {
            SetHttpRequestMapping("GET", "/language");
        }
    }
}