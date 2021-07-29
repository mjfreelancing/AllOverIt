using AllOverIt.Aws.Cdk.AppSync.Attributes;

namespace GraphqlSchema.Schema.Types
{
    [SchemaType("Country")]
    internal interface ICountry : ISchemaTypeBase
    {
        [SchemaTypeRequired]
        public string Name();

        [SchemaTypeRequired]
        public string Currency();

        // todo: solve non-support for circular references
        //[SchemaTypeRequired]
        //public IContinent Continent ();

        [SchemaArrayRequired]
        [SchemaTypeRequired]
        public ILanguage[] Languages();
    }
}