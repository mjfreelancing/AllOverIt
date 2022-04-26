namespace AllOverIt.Aws.Cdk.AppSync.Attributes.Types
{
    /// <summary>Apply to a class that describes a schema 'scalar' type.</summary>
    public sealed class SchemaScalarAttribute : SchemaTypeBaseAttribute
    {
        /// <summary>When not null or empty, identifies the portion of the scalar's namespace to exclude from the
        /// generated name.</summary>
        public SchemaScalarAttribute()
            : base(GraphqlSchemaType.Input)
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="name">The Graphql schema 'scalar' name.</param>
        public SchemaScalarAttribute(string name)
            : base(name, GraphqlSchemaType.Input)
        {
        }

        /// <summary>Constructor. The Graphql schema scalar's name will be based on the namespace of the class/interface the
        /// attribute is associated with and an optionally specified name.</summary>
        /// <param name="excludeNamespacePrefix">The portion of the scalar's namespace to exclude from the generated name.</param>
        /// <param name="name">An optional name to append to the namespace. This can be null or an empty string if not required.</param>
        public SchemaScalarAttribute(string excludeNamespacePrefix, string name)
            : base(excludeNamespacePrefix, name, GraphqlSchemaType.Input)
        {
        }
    }
}