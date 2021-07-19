using AllOverIt.Aws.Cdk.AppSync.Attributes;
using AllOverIt.Aws.Cdk.AppSync.Schema.Types;

namespace GraphqlSchema.Schema.Types
{
    [SchemaType("Country")]
    internal interface ICountry : ISchemaTypeBase
    {
        [SchemaTypeRequired]
        public string Name { get; }

        [SchemaTypeRequired]
        public string Currency { get; }

        // todo: solve non-support for circular references
        //[SchemaTypeRequired]
        //public IContinent Continent { get; }

        [SchemaArrayRequired]
        [SchemaTypeRequired]
        public ILanguage[] Languages { get; }
    }
}