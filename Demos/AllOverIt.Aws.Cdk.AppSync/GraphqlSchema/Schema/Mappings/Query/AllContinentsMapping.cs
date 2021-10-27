namespace GraphqlSchema.Schema.Mappings.Query
{
    internal sealed class AllContinentsMapping : HttpGetResponseMapping
    {
        // this class is used for demonstrating registration via a factory using a common base class
        public AllContinentsMapping(string apiKey)
            : base("/continents", apiKey)
        {
        }
    }
}