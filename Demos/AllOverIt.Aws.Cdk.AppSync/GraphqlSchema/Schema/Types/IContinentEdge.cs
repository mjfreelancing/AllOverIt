using AllOverIt.Aws.Cdk.AppSync.Attributes;
using AllOverIt.Aws.Cdk.AppSync.Schema.Types;

namespace GraphqlSchema.Schema.Types
{
    [SchemaType("ContinentEdge")]
    internal interface IContinentEdge : IEdge<IContinent>
    {
    }
}