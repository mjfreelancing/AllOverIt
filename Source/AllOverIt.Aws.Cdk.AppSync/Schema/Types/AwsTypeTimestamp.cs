using AllOverIt.Aws.Cdk.AppSync.Attributes.Types;

namespace AllOverIt.Aws.Cdk.AppSync.Schema.Types
{
    /// <summary>A custom scalar type that will be interpreted as an AwsTimestamp type.</summary>
    [SchemaScalar(nameof(AwsTypeTimestamp))]
    public sealed class AwsTypeTimestamp
    {
    }
}