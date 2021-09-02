namespace GraphqlSchema.Schema.Mappings
{
    internal abstract class NoneResponseMapping : RequestResponseMappingBase
    {
        protected NoneResponseMapping()
        {
            RequestMapping = GetNoneRequestMapping();
            ResponseMapping = GetNoneResponseMapping();
        }
    }
}