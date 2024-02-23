namespace GraphqlSchema.Schema.Mappings.Query
{
    internal sealed class PopulationMapping : RequestResponseMappingBase
    {
        public PopulationMapping() 
        {
            Code = GetCodeMapping();
        }
    }
}