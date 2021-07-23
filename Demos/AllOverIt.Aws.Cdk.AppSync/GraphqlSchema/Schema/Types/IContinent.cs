using AllOverIt.Aws.Cdk.AppSync;
using AllOverIt.Aws.Cdk.AppSync.Attributes;

namespace GraphqlSchema.Schema.Types
{
    [SchemaType("Continent")]
    internal interface IContinent : ISchemaTypeBase
    {
        [SchemaTypeRequired]
        public string Name { get; }

        // sharing this http datasource
        [SchemaArrayRequired]
        [SchemaTypeRequired]
        [HttpDataSource(Constants.AppName, EndpointSource.ImportValue, Constants.Import.GetCountriesUrlImportName)]
        public ICountry[] Countries { get; }

        // sharing this http datasource
        [SchemaArrayRequired]
        [SchemaTypeRequired]
        [HttpDataSource(Constants.AppName, EndpointSource.ImportValue, Constants.Import.GetCountriesUrlImportName)]
        public string[] CountryCodes();
    }
}