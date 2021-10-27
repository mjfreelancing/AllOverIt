namespace AllOverIt.Aws.Cdk.AppSync.Attributes.Types
{
    /// <summary>Apply to an interface that describes a schema 'type' type.</summary>
    public sealed class SchemaTypeAttribute : SchemaTypeBaseAttribute
    {
        /// <summary>Constructor.</summary>
        /// <param name="name">The name of the schema 'type' type to apply to the Graphql schema.</param>
        public SchemaTypeAttribute(string name)
            : base(name, GraphqlSchemaType.Type)
        {
        }
    }
}