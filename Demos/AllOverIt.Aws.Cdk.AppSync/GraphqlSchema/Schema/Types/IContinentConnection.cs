using AllOverIt.Aws.Cdk.AppSync.Attributes.Types;
using AllOverIt.Aws.Cdk.AppSync.Schema.Types;

namespace GraphqlSchema.Schema.Types
{
    [SchemaType("ContinentConnection")]
    internal interface IContinentConnection : IConnection<IContinentEdge, IContinent>
    {
    }
}