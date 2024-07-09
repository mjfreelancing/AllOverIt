namespace AllOverIt.Aws.Cdk.AppSync.Attributes.Types
{
    /// <summary>Apply to an interface that describes a schema 'type' type.</summary>
    public class SchemaTypeAttribute : SchemaTypeBaseAttribute
    {
        /// <summary>When not null or empty, identifies the portion of the type's namespace to exclude from the
        /// generated name.</summary>
        public SchemaTypeAttribute()
            : base(GraphqlSchemaType.Type)
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="name">The Graphql schema type name.</param>
        public SchemaTypeAttribute(string name)
            : base(name, GraphqlSchemaType.Type)
        {
        }

        /// <summary>Constructor. The Graphql schema type's name will be based on the namespace of the class/interface the
        /// attribute is associated with and an optionally specified name.</summary>
        /// <param name="excludeNamespacePrefix">The portion of the type's namespace to exclude from the generated name.</param>
        /// <param name="name">An optional name to append to the namespace. This can be null or an empty string if not required.</param>
        public SchemaTypeAttribute(string excludeNamespacePrefix, string? name)
            : base(excludeNamespacePrefix, name, GraphqlSchemaType.Type)
        {
        }
    }
}