namespace AllOverIt.Aws.Cdk.AppSync.Attributes.Types
{
    /// <summary>Apply to an interface that describes a schema 'input' type.</summary>
    public sealed class SchemaInputAttribute : SchemaTypeBaseAttribute
    {
        /// <summary>Constructor.</summary>
        /// <param name="name">The name of the schema 'input' type to apply to the Graphql schema.</param>
        public SchemaInputAttribute(string name)
            : base(name, GraphqlSchemaType.Input)
        {
        }
    }
}