using AllOverIt.Aws.Cdk.AppSync;
using AllOverIt.Aws.Cdk.AppSync.Attributes;

namespace GraphqlSchema.Schema.Inputs
{
    [SchemaType("LanguageFilterInput", GraphqlSchemaType.Input)]
    internal interface ILanguageFilterInput
    {
        public IStringQueryOperatorInput Code { get; }
    }
}