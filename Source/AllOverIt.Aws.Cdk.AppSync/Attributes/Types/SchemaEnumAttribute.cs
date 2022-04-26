using System;

namespace AllOverIt.Aws.Cdk.AppSync.Attributes.Types
{
    /// <summary>Apply to an enum that requires a custom name to be generated in the schema.</summary>
    [AttributeUsage(AttributeTargets.Enum, Inherited = false)]
    public class SchemaEnumAttribute : SchemaTypeBaseAttribute
    {
        /// <summary>When not null or empty, identifies the portion of the enum's namespace to exclude from the
        /// generated name.</summary>
        public SchemaEnumAttribute()
            : base(GraphqlSchemaType.Enum)
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="name">The Graphql schema enum name.</param>
        public SchemaEnumAttribute(string name)
            : base(name, GraphqlSchemaType.Enum)
        {
        }

        /// <summary>Constructor. The Graphql schema enum's name will be based on the namespace of the class/interface the
        /// attribute is associated with and an optionally specified name.</summary>
        /// <param name="excludeNamespacePrefix">The portion of the enum's namespace to exclude from the generated name.</param>
        /// <param name="name">An optional name to append to the namespace. This can be null or an empty string if not required.</param>
        public SchemaEnumAttribute(string excludeNamespacePrefix, string name)
            : base(excludeNamespacePrefix, name, GraphqlSchemaType.Enum)
        {
        }
    }
}