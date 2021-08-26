namespace GraphqlSchema.Schema.Mappings.Query
{
    internal sealed class AllContinentsMapping : RequestResponseMappingBase
    {
        public override string RequestMapping { get; }
        public override string ResponseMapping { get; }

        public AllContinentsMapping()
        {
            RequestMapping = GetHttpRequestMapping("GET", "/continents");
            ResponseMapping = GetHttpResponseMapping();
        }
    }
}