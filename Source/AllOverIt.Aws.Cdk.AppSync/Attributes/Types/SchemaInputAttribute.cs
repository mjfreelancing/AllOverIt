namespace AllOverIt.Aws.Cdk.AppSync.Attributes.Types
{
    /// <summary>Apply to an interface that describes a schema 'input' type.</summary>
    public class SchemaInputAttribute : SchemaTypeBaseAttribute
    {
        /// <summary>Constructor.</summary>
        public SchemaInputAttribute()
            : base(GraphqlSchemaType.Input)
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="name">The Graphql schema 'input' name.</param>
        public SchemaInputAttribute(string name)
            : base(name, GraphqlSchemaType.Input)
        {
        }

        /// <summary>Constructor. The Graphql schema input's name will be based on the namespace of the class/interface the
        /// attribute is associated with and an optionally specified name.</summary>
        /// <param name="excludeNamespacePrefix">The portion of the input's namespace to exclude from the generated name.</param>
        /// <param name="name">An optional name to append to the namespace. This can be null or an empty string if not required.</param>
        public SchemaInputAttribute(string excludeNamespacePrefix, string name)
            : base(excludeNamespacePrefix, name, GraphqlSchemaType.Input)
        {
        }
    }
}