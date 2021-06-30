using AllOverIt.Aws.Cdk.AppSync;
using AllOverIt.Aws.Cdk.AppSync.Attributes;
using AllOverIt.Aws.Cdk.AppSync.Schema.Types;

namespace GraphqlSchema.Schema.Types
{
    [SchemaType("ContinentConnection", GraphqlSchemaType.Type)]
    internal interface IContinentConnection : IConnection<IContinentEdge, IContinent>
    {
    }
}