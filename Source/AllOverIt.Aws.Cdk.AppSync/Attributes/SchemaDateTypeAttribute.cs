using System;

namespace AllOverIt.Aws.Cdk.AppSync.Attributes
{
    // Add this to a DateTime to have it exported as an AWSDate
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Parameter)]
    public sealed class SchemaDateTypeAttribute : Attribute
    {
    }
}