namespace AllOverIt.Aws.Cdk.AppSync.Attributes
{
    public sealed class SchemaInputAttribute : SchemaTypeBaseAttribute
    {
        public SchemaInputAttribute(string name)
            : base(name, AppSync.GraphqlSchemaType.Input)
        {
        }
    }
}