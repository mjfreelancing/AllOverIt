namespace GraphqlSchema.Schema.Mappings
{
    internal abstract class FunctionResponseMapping : RequestResponseMappingBase
    {
        public override string RequestMapping { get; }
        public override string ResponseMapping { get; }

        protected FunctionResponseMapping()
        {
            RequestMapping = GetFunctionRequestMapping();
            ResponseMapping = GetFunctionResponseMapping();
        }
    }
}