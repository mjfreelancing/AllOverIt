namespace GraphqlSchema.Schema.Mappings
{
    internal abstract class FunctionResponseMapping : RequestResponseMappingBase
    {
        protected FunctionResponseMapping()
        {
            RequestMapping = GetFunctionRequestMapping();
            ResponseMapping = GetFunctionResponseMapping();
        }
    }
}