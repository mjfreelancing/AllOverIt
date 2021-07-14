using AllOverIt.Aws.Cdk.AppSync.Attributes;
using AllOverIt.Aws.Cdk.AppSync.Schema.Types;

namespace GraphqlSchema.Schema.Inputs
{
    [SchemaInput("CountryInput")]
    internal interface ICountryInput
    {
        [SchemaTypeRequired]
        public GraphqlTypeId Code { get; }

        [SchemaTypeRequired]
        public string Name { get; }

        [SchemaTypeRequired]
        public string Currency { get; }

        // todo: solve non-support for circular references
        //[SchemaTypeRequired]
        //public IContinentInput Continent { get; }

        [SchemaArrayRequired]
        [SchemaTypeRequired]
        public ILanguageInput[] Languages { get; }
    }
}