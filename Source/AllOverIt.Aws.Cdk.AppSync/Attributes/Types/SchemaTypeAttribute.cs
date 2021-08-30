namespace AllOverIt.Aws.Cdk.AppSync.Attributes.Types
{
    public sealed class SchemaTypeAttribute : SchemaTypeBaseAttribute
    {
        public SchemaTypeAttribute(string name)
            : base(name, GraphqlSchemaType.Type)
        {
        }
    }
}