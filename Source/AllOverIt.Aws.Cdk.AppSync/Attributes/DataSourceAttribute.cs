using System;

namespace AllOverIt.Aws.Cdk.AppSync.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public abstract class DataSourceAttribute : Attribute
    {
        // used for lookup in the DataSourceFactory
        public abstract string LookupKey { get; }
    }
}