namespace GraphqlSchema.Schema.Mappings.Query
{
    internal sealed class LanguageMapping : RequestResponseMappingBase
    {
        public override string RequestMapping { get; }
        public override string ResponseMapping { get; }

        public LanguageMapping()
        {
            RequestMapping = GetHttpRequestMapping("GET", "/language");
            ResponseMapping = GetHttpResponseMapping();
        }
    }
}