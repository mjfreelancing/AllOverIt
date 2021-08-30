namespace AllOverIt.Aws.Cdk.AppSync.Attributes.Types
{
    public sealed class SchemaScalarAttribute : SchemaTypeBaseAttribute
    {
        public SchemaScalarAttribute(string name)
            : base(name, GraphqlSchemaType.Scalar)
        {
        }
    }
}