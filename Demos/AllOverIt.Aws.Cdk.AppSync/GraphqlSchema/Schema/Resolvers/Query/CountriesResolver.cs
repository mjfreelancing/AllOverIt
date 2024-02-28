namespace GraphqlSchema.Schema.Resolvers.Query
{
    internal sealed class CountriesResolver : RequestResponseResolverBase
    {
        public CountriesResolver()
        {
            SetHttpRequestMapping("GET", "/countries");
        }
    }
}