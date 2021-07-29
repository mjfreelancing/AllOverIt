using AllOverIt.Aws.Cdk.AppSync.Attributes;

namespace GraphqlSchema.Schema.Inputs
{
    [SchemaInput("StringQueryOperatorInput")]
    internal interface IStringQueryOperatorInput
    {
#pragma warning disable IDE1006 // Naming Styles
        public string eq();
        public string ne();
        public string gt();
        public string gte();
        public string lt();
        public string lte();
#pragma warning restore IDE1006 // Naming Styles

        public string In();
        public string NotIn();
        public string Regex();
    }
}