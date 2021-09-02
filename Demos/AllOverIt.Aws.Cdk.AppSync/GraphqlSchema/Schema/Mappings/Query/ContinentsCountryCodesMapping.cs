namespace GraphqlSchema.Schema.Mappings.Query
{
    internal sealed class ContinentsCountryCodesMapping : RequestResponseMappingBase
    {
        public ContinentsCountryCodesMapping()
        {
            RequestMapping = GetHttpRequestMapping("GET", "/countryCodes");
            ResponseMapping = GetHttpResponseMapping();
        }
    }
}