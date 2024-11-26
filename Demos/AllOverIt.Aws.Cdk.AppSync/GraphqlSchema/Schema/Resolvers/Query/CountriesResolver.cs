namespace GraphqlSchema.Schema.Resolvers.Query
{
    internal sealed class CountriesResolver : VtlResolverBase
    {
        public CountriesResolver()
        {
            SetHttpRequestMapping("GET", "/countries");
        }
    }
}