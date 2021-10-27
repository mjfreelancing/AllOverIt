using System;

namespace AllOverIt.Aws.Cdk.AppSync.Attributes.Types
{
    // NOTE: Keep the 'Attribute' suffix (and not Base)

    /// <summary>Base class for all custom schema types.</summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
    public abstract class SchemaTypeBaseAttribute : Attribute
    {
        /// <summary>The Graphql schema type name.</summary>
        public string Name { get; }

        /// <summary>The Graphql schema type.</summary>
        public GraphqlSchemaType GraphqlSchemaType { get; }

        /// <summary>Constructor.</summary>
        /// <param name="name">The Graphql schema type name.</param>
        /// <param name="graphqlSchemaType">The Graphql schema type.</param>
        protected SchemaTypeBaseAttribute(string name, GraphqlSchemaType graphqlSchemaType)
        {
            GraphqlSchemaType = graphqlSchemaType;
            Name = name;
        }
    }
}