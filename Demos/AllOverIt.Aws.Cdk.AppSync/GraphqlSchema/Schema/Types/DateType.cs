using AllOverIt.Aws.Cdk.AppSync.Attributes.Types;

namespace GraphqlSchema.Schema.Types
{
    // Without an attribute this will be automatically named "DateType" - unless overriden as shown by the demo
    //[SchemaEnum("JustAnotherDateType")]
    internal enum DateType
    {
        Discovered,
        Settled
    }
}