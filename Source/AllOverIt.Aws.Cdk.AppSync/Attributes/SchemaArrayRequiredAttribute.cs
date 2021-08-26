using System;

namespace AllOverIt.Aws.Cdk.AppSync.Attributes
{
    // Used to indicate an array 'type' or 'input' is required
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter)]
    public sealed class SchemaArrayRequiredAttribute : Attribute
    {
    }
}