namespace GraphqlSchema.Schema.Mappings.Query
{
    internal sealed class ContinentsCountriesMapping : HttpGetResponseMapping
    {
        // this class is used for demonstrating registration via a factory using a common base class
        public ContinentsCountriesMapping(string apiKey)
            : base("/countries", apiKey)
        {
        }
    }
}