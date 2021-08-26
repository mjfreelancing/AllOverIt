using System;
using System.Text.RegularExpressions;

namespace AllOverIt.Aws.Cdk.AppSync.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class DataSourceAttribute : Attribute
    {
        // used for lookup in the DataSourceFactory
        public abstract string LookupKey { get; }
        public string Description { get; }

        protected static string SanitiseLookupKey(string lookupKey)
        {
            // exclude everything exception alphanumeric and dashes
            return Regex.Replace(lookupKey, @"[^\w]", "", RegexOptions.None);
        }

        public DataSourceAttribute(string description)
        {
            Description = description;
        }
    }
}