using AllOverIt.Aws.Cdk.AppSync.Attributes;
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

        // todo: solve non-support for circular references
        //[SchemaTypeRequired]
        //public IContinentInput Continent();

        [SchemaArrayRequired]
        [SchemaTypeRequired]
        public ILanguageInput[] Languages();
    }
}