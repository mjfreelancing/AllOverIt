using AllOverIt.Aws.Cdk.AppSync.Attributes.Types;

namespace GraphqlSchema.Schema.Types
{
    [SchemaEnum("GraphqlSchema.Schema", "LanguageGroup")]       // Testing it will be named TypesLanguageGroup
    internal enum LanguageType
    {
        Native,
        Other
    }
}