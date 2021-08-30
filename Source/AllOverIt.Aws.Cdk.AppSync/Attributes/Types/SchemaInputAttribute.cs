namespace AllOverIt.Aws.Cdk.AppSync.Attributes.Types
{
    public sealed class SchemaInputAttribute : SchemaTypeBaseAttribute
    {
        public SchemaInputAttribute(string name)
            : base(name, GraphqlSchemaType.Input)
        {
        }
    }
}