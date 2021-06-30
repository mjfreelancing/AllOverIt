using AllOverIt.Aws.Cdk.AppSync;
using AllOverIt.Aws.Cdk.AppSync.Attributes;
using AllOverIt.Aws.Cdk.AppSync.Schema.Types;

namespace GraphqlSchema.Schema.Types
{
    [SchemaType("Country", GraphqlSchemaType.Type)]
    internal interface ICountry
    {
        [SchemaTypeRequired]
        public GraphqlTypeId Code { get; }

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