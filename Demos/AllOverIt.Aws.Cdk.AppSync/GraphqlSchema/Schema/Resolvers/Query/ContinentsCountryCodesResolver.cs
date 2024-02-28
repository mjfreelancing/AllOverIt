namespace GraphqlSchema.Schema.Resolvers.Query
{
    internal sealed class ContinentsCountryCodesResolver : RequestResponseResolverBase
    {
        public ContinentsCountryCodesResolver()
        {
            SetHttpRequestMapping("GET", "/countryCodes");
        }
    }
}