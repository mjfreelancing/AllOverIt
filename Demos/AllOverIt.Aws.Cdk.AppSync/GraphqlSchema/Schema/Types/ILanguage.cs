using AllOverIt.Aws.Cdk.AppSync;
using AllOverIt.Aws.Cdk.AppSync.Attributes;

namespace GraphqlSchema.Schema.Types
{
    [SchemaType("Language", GraphqlSchemaType.Type)]
    internal interface ILanguage
    {
        [SchemaTypeRequired]
        public string Code { get; }

        [SchemaTypeRequired]
        public string Name { get; }
    }
}