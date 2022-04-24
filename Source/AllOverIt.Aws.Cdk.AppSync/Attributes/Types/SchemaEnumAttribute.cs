using AllOverIt.Assertion;
using System;

namespace AllOverIt.Aws.Cdk.AppSync.Attributes.Types
{
    /// <summary>Apply to an enum that requires a custom name to be generated in the schema.</summary>
    [AttributeUsage(AttributeTargets.Enum, Inherited = false)]
    public sealed class SchemaEnumAttribute : Attribute
    {
        /// <summary>The Graphql schema enum name.</summary>
        public string Name { get; }

        /// <summary>Constructor.</summary>
        /// <param name="name">The name of the schema enum type to apply to the Graphql schema.</param>
        public SchemaEnumAttribute(string name)
        {
            Name = name.WhenNotNullOrEmpty();
        }
    }
}