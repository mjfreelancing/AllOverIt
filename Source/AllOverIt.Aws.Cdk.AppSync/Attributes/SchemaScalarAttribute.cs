namespace AllOverIt.Aws.Cdk.AppSync.Attributes
{
    public sealed class SchemaScalarAttribute : SchemaTypeBaseAttribute
    {
        public SchemaScalarAttribute(string name)
            : base(name, GraphqlSchemaType.Scalar)
        {
        }
    }
}