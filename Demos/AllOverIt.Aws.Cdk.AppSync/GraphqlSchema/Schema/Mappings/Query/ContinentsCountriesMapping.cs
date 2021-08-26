namespace GraphqlSchema.Schema.Mappings.Query
{
    internal sealed class ContinentsCountriesMapping : RequestResponseMappingBase
    {
        public override string RequestMapping { get; }
        public override string ResponseMapping { get; }

        public ContinentsCountriesMapping()
        {
            RequestMapping = GetHttpRequestMapping("GET", "/countries");
            ResponseMapping = GetHttpResponseMapping();
        }
    }
}