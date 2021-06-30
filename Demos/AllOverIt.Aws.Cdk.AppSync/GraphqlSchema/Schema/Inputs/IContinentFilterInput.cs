using AllOverIt.Aws.Cdk.AppSync;
using AllOverIt.Aws.Cdk.AppSync.Attributes;

namespace GraphqlSchema.Schema.Inputs
{
    [SchemaType("ContinentFilterInput", GraphqlSchemaType.Input)]
    internal interface IContinentFilterInput
    {
        public IStringQueryOperatorInput Code { get; }
    }
}