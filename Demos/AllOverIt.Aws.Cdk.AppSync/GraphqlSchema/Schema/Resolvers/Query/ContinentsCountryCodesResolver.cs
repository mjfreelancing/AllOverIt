namespace GraphqlSchema.Schema.Resolvers.Query
{
    internal sealed class ContinentsCountryCodesResolver : VtlResolverBase
    {
        public ContinentsCountryCodesResolver()
        {
            SetHttpRequestMapping("GET", "/countryCodes");
        }
    }
}