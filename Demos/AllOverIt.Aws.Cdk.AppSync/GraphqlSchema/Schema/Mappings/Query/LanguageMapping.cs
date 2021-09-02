namespace GraphqlSchema.Schema.Mappings.Query
{
    internal sealed class LanguageMapping : RequestResponseMappingBase
    {
        public LanguageMapping()
        {
            RequestMapping = GetHttpRequestMapping("GET", "/language");
            ResponseMapping = GetHttpResponseMapping();
        }
    }
}