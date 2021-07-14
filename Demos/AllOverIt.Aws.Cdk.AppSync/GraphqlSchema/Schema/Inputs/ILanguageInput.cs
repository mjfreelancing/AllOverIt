using AllOverIt.Aws.Cdk.AppSync.Attributes;

namespace GraphqlSchema.Schema.Inputs
{
    [SchemaInput("LanguageInput")]
    internal interface ILanguageInput
    {
        [SchemaTypeRequired]
        public string Code { get; }

        [SchemaTypeRequired]
        public string Name { get; }
    }
}