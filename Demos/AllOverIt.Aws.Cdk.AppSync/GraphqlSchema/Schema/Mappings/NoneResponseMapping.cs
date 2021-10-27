namespace GraphqlSchema.Schema.Mappings
{
    internal class NoneResponseMapping : RequestResponseMappingBase
    {
        public NoneResponseMapping()
        {
            RequestMapping = GetNoneRequestMapping();
            ResponseMapping = GetNoneResponseMapping();
        }
    }
}