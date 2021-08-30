using AllOverIt.Aws.Cdk.AppSync.Attributes.Types;

namespace GraphqlSchema.Schema.Inputs
{
    [SchemaInput("ContinentFilterInput")]
    internal interface IContinentFilterInput
    {
        public IStringQueryOperatorInput Code();
    }
}