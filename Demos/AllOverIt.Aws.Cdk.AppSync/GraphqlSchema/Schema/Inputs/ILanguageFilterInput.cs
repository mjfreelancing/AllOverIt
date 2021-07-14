using AllOverIt.Aws.Cdk.AppSync.Attributes;

namespace GraphqlSchema.Schema.Inputs
{
    [SchemaInput("LanguageFilterInput")]
    internal interface ILanguageFilterInput
    {
        public IStringQueryOperatorInput Code { get; }
    }
}