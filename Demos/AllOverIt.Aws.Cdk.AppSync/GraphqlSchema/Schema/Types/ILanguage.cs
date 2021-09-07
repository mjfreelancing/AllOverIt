using AllOverIt.Aws.Cdk.AppSync.Attributes.Types;

namespace GraphqlSchema.Schema.Types
{
    [SchemaType("Language")]
    internal interface ILanguage : ISchemaTypeBase
    {
        public string Name();
    }
}