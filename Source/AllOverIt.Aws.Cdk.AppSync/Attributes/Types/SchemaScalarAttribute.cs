namespace AllOverIt.Aws.Cdk.AppSync.Attributes.Types
{
    /// <summary>Apply to a class that describes a schema 'scalar' type.</summary>
    public sealed class SchemaScalarAttribute : SchemaTypeBaseAttribute
    {
        /// <summary>Constructor.</summary>
        /// <param name="name">The name of the schema 'scalar' type to apply to the Graphql schema.</param>
        public SchemaScalarAttribute(string name)
            : base(name, GraphqlSchemaType.Scalar)
        {
        }
    }
}