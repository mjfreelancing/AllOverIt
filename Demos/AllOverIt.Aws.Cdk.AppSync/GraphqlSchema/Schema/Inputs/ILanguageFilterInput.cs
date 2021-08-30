using AllOverIt.Aws.Cdk.AppSync.Attributes.Types;

namespace GraphqlSchema.Schema.Inputs
{
    [SchemaInput("LanguageFilterInput")]
    internal interface ILanguageFilterInput
    {
        public IStringQueryOperatorInput Code();
    }
}