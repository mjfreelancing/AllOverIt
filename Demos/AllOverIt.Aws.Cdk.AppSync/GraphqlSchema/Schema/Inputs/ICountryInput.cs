using AllOverIt.Aws.Cdk.AppSync.Attributes.Types;
using AllOverIt.Aws.Cdk.AppSync.Schema.Types;

namespace GraphqlSchema.Schema.Inputs
{
    [SchemaInput("CountryInput")]
    internal interface ICountryInput
    {
        [SchemaTypeRequired]
        public GraphqlTypeId Code();

        [SchemaTypeRequired]
        public string Name();

        [SchemaTypeRequired]
        public string Currency();

        [SchemaArrayRequired]
        [SchemaTypeRequired]
        public ILanguageInput[] Languages();
    }
}