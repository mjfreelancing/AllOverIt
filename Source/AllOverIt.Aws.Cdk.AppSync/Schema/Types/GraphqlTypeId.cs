using AllOverIt.Aws.Cdk.AppSync.Attributes.Types;

namespace AllOverIt.Aws.Cdk.AppSync.Schema.Types
{
    /// <summary>A custom scalar type that will be interpreted as ID type.</summary>
    [SchemaScalar(nameof(GraphqlTypeId))]
    public sealed class GraphqlTypeId
    {
    }
}