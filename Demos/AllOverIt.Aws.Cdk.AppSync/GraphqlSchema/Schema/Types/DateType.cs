using AllOverIt.Aws.Cdk.AppSync.Attributes.Types;

namespace GraphqlSchema.Schema.Types
{
    // Without an attribute this will be automatically named "DateType"
    internal enum DateType
    {
        Discovered,
        Settled
    }
}