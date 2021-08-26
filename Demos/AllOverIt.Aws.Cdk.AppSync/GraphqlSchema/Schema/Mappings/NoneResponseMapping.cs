namespace GraphqlSchema.Schema.Mappings
{
    internal abstract class NoneResponseMapping : RequestResponseMappingBase
    {
        public override string RequestMapping { get; }
        public override string ResponseMapping { get; }

        protected NoneResponseMapping()
        {
            RequestMapping = GetNoneRequestMapping();
            ResponseMapping = GetNoneResponseMapping();
        }
    }
}