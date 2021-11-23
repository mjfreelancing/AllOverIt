using AllOverIt.Aws.Cdk.AppSync.Attributes.Types;

namespace GraphqlSchema.Schema.Types
{
    [SchemaEnum("LanguageGroup")]
    internal enum LanguageType
    {
        Native,
        Other
    }
}