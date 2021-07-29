using AllOverIt.Aws.Cdk.AppSync.Attributes;

namespace GraphqlSchema.Schema.Inputs
{
    [SchemaInput("ContinentFilterInput")]
    internal interface IContinentFilterInput
    {
        public IStringQueryOperatorInput Code();
    }
}