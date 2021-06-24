using System;

namespace AllOverIt.Aws.Cdk.AppSync.Attributes
{
    // Add this to a DateTime to have it exported as an AWSTime
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Parameter)]
    public sealed class SchemaTimeTypeAttribute : Attribute
    {
    }
}