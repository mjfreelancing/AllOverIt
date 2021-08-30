using AllOverIt.Aws.Cdk.AppSync.Attributes.Types;

namespace GraphqlSchema.Schema.Types
{
    [SchemaType("Country")]
    internal interface ICountry : ISchemaTypeBase
    {
        [SchemaTypeRequired]
        public string Name();

        [SchemaTypeRequired]
        public string Currency();

        [SchemaArrayRequired]
        [SchemaTypeRequired]
        public ILanguage[] Languages();

        [SchemaTypeRequired]
        public IContinent Continent();      // this is a circular reference
    }
}