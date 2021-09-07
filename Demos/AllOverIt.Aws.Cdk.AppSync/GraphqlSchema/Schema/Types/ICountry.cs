using AllOverIt.Aws.Cdk.AppSync.Attributes.Types;

namespace GraphqlSchema.Schema.Types
{
    [SchemaType("Country")]
    internal interface ICountry : ISchemaTypeBase
    {
        public string Name();
        public string Currency();
        public ILanguage[] Languages();
        public IContinent Continent();      // this is a circular reference
    }
}