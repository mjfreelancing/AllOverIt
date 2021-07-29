using AllOverIt.Aws.Cdk.AppSync.Attributes;
using AllOverIt.Aws.Cdk.AppSync.Schema.Types;

namespace GraphqlSchema.Schema.Inputs
{
    [SchemaInput("LanguageInput")]
    internal interface ILanguageInput
    {
        [SchemaTypeRequired]
        public GraphqlTypeId Code();

        [SchemaTypeRequired]
        public string Name();
    }
}