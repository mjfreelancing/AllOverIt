namespace GraphqlSchema.Schema.Mappings.Query
{
    internal sealed class ContinentsCountryCodesMapping : RequestResponseMappingBase
    {
        public override string RequestMapping { get; }
        public override string ResponseMapping { get; }

        public ContinentsCountryCodesMapping()
        {
            RequestMapping = GetHttpRequestMapping("GET", "/countryCodes");
            ResponseMapping = GetHttpResponseMapping();
        }
    }
}