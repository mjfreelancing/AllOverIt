using AllOverIt.Aws.Cdk.AppSync.Attributes.Types;

namespace GraphqlSchema.Schema.Types
{
    [SchemaType("Language")]
    internal interface ILanguage : ISchemaTypeBase
    {
        string Name();

        // The LanguageType is renamed to LanguageGroup in the schema (see the attribute on its definition)
        LanguageType LanguageType();

        LanguageType[] LanguageTypes();
    }
}