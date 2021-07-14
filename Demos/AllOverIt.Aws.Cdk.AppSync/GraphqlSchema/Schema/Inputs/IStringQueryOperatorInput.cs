using AllOverIt.Aws.Cdk.AppSync.Attributes;

namespace GraphqlSchema.Schema.Inputs
{
    [SchemaInput("StringQueryOperatorInput")]
    internal interface IStringQueryOperatorInput
    {
        public string eq { get; }
        public string ne { get; }
        public string gt { get; }
        public string gte { get; }
        public string lt { get; }
        public string lte { get; }
        public string In { get; }
        public string NotIn { get; }
        public string Regex { get; }
    }
}