using AllOverIt.Aws.Cdk.AppSync.Attributes.Types;

namespace GraphqlSchema.Schema.Inputs
{
    [SchemaInput("StringQueryOperatorInput")]
    internal interface IStringQueryOperatorInput
    {
        public string Eq();
        public string Ne();
        public string Gt();
        public string Gte();
        public string Lt();
        public string Lte();
        public string In();
        public string NotIn();
        public string Regex();
    }
}