namespace GraphqlSchema.Schema.Mappings.Query
{
    internal sealed class CountriesMapping : RequestResponseMappingBase
    {
        public CountriesMapping()
        {
            RequestMapping = GetHttpRequestMapping("GET", "/countries");
            ResponseMapping = GetHttpResponseMapping();
        }
    }
}