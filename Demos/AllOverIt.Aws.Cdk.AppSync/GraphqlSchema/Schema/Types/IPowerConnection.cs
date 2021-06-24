using AllOverIt.Aws.Cdk.AppSync;
using AllOverIt.Aws.Cdk.AppSync.Attributes;
using AllOverIt.Aws.Cdk.AppSync.Schema.Types;

namespace GraphqlSchema.Schema.Types
{
    [SchemaType(GraphqlSchemaType.Type, "PowerConnection")]
    internal interface IPowerConnection : IConnection<IPowerEdge, ITimeWatts>
    {
    }
}