using System;

namespace AllOverIt.Aws.Cdk.AppSync.Attributes
{
    // Used to indicate a schema scalar, custom 'type', or 'input' is required
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter)]
    public sealed class SchemaTypeRequiredAttribute : Attribute
    {
    }
}