using AllOverIt.Aws.Cdk.AppSync.Attributes.Types;
using AllOverIt.Aws.Cdk.AppSync.Schema.Types;

namespace GraphqlSchema.Schema.Types.Globe
{
    // Testing the use of namespaces => should produce 'GlobeContinentEdge' (the last argument can be null/empty)
    [SchemaType("GraphqlSchema.Schema.Types", "ContinentEdge")]
    internal interface IContinentEdge : IEdge<IContinent>
    {
    }
}