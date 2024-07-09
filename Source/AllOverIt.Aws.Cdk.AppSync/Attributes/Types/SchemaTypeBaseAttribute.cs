namespace AllOverIt.Aws.Cdk.AppSync.Attributes.Types
{
    // NOTE: Keep the 'Attribute' suffix (and not Base)

    /// <summary>Base class for all custom schema types.</summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
    public abstract class SchemaTypeBaseAttribute : Attribute
    {
        /// <summary>When not <see langword="null"/> or empty, identifies the portion of the type's namespace to exclude from the
        /// generated name.</summary>
        public string? ExcludeNamespacePrefix { get; }

        /// <summary>The Graphql schema type name. When not <see langword="null"/>, the Graphql schema type's name will be based
        /// on the name of the type the attribute is associated with.</summary>
        public string? Name { get; }

        /// <summary>The Graphql schema type.</summary>
        public GraphqlSchemaType GraphqlSchemaType { get; }

        /// <summary>Constructor. The Graphql schema type's name will be based on the name of the type the attribute is associated
        /// with.</summary>
        /// <param name="graphqlSchemaType">The Graphql schema type.</param>
        protected SchemaTypeBaseAttribute(GraphqlSchemaType graphqlSchemaType)
        {
            GraphqlSchemaType = graphqlSchemaType;
        }

        /// <summary>Constructor.</summary>
        /// <param name="name">The Graphql schema type name.</param>
        /// <param name="graphqlSchemaType">The Graphql schema type.</param>
        protected SchemaTypeBaseAttribute(string? name, GraphqlSchemaType graphqlSchemaType)
            : this(graphqlSchemaType)
        {
            Name = name;
        }

        /// <summary>Constructor. The Graphql schema type's name will be based on the namespace of the type the attribute is associated
        /// with and an optionally specified name.</summary>
        /// <param name="excludeNamespacePrefix">The portion of the type's namespace to exclude from the generated name.</param>
        /// <param name="name">An optional name to append to the namespace. This can be null or an empty string if not required.</param>
        /// <param name="graphqlSchemaType">The Graphql schema type.</param>
        protected SchemaTypeBaseAttribute(string excludeNamespacePrefix, string? name, GraphqlSchemaType graphqlSchemaType)
            : this(name, graphqlSchemaType)
        {
            ExcludeNamespacePrefix = excludeNamespacePrefix;
        }
    }
}