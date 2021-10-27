namespace GraphqlSchema.Schema.Mappings
{
    internal class FunctionResponseMapping : RequestResponseMappingBase
    {
        public FunctionResponseMapping()
        {
            RequestMapping = GetFunctionRequestMapping();
            ResponseMapping = GetFunctionResponseMapping();
        }
    }
}