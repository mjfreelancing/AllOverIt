using AllOverIt.Aws.Cdk.AppSync.Attributes.Types;

namespace AllOverIt.Aws.Cdk.AppSync.Schema.Types
{
    /// <summary>A custom scalar type that will be interpreted as an AwsDateTime type.</summary>
    [SchemaScalar(nameof(AwsTypeDateTime))]
    public sealed class AwsTypeDateTime
    {
    }
}