using AllOverIt.Aws.Cdk.AppSync.Attributes.Types;

namespace GraphqlSchema.Schema.Types.Globe
{
    // Testing the use of namespaces => should produce 'GlobeCountry' (the last argument can be null/empty)
    [SchemaType("GraphqlSchema.Schema.Types", "Country")]
    internal interface ICountry : ISchemaTypeBase
    {
        public string Name();
        public string Currency();
        public ILanguage[] Languages();
        public IContinent Continent();      // this is a circular reference
    }
}