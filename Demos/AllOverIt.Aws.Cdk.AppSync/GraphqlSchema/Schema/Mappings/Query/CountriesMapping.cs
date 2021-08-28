namespace GraphqlSchema.Schema.Mappings.Query
{
    internal sealed class CountriesMapping : RequestResponseMappingBase
    {
        public override string RequestMapping { get; }
        public override string ResponseMapping { get; }

        public CountriesMapping()
        {
            RequestMapping = GetHttpRequestMapping("GET", "/countries");
            ResponseMapping = GetHttpResponseMapping();
        }
    }
}