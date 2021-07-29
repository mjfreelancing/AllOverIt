using AllOverIt.Aws.Cdk.AppSync.Attributes;

namespace GraphqlSchema.Schema.Types
{
    [SchemaType("Language")]
    internal interface ILanguage : ISchemaTypeBase
    {
        [SchemaTypeRequired]
        public string Name();
    }
}