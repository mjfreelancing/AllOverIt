namespace GraphqlSchema.Schema.Mappings.Query
{
    internal sealed class AllContinentsMapping : HttpGetResponseMappingBase
    {
        // this class is used for demonstrating registration via a factory using a common base class
        public AllContinentsMapping(string apiKey)
            : base("/continents", apiKey)
        {
        }
    }
}