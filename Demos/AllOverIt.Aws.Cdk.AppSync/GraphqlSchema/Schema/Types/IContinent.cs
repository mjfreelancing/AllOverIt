using AllOverIt.Aws.Cdk.AppSync;
using AllOverIt.Aws.Cdk.AppSync.Attributes;

namespace GraphqlSchema.Schema.Types
{
    [SchemaType("Continent")]
    internal interface IContinent : ISchemaTypeBase
    {
        [SchemaTypeRequired]
        string Name();

        // sharing this http datasource
        [SchemaArrayRequired]
        [SchemaTypeRequired]
        [HttpDataSource(Constants.AppName, EndpointSource.ImportValue, Constants.Import.GetCountriesUrlImportName)]
        ICountry[] Countries();

        // sharing this http datasource
        [SchemaArrayRequired]
        [SchemaTypeRequired]
        [HttpDataSource(Constants.AppName, EndpointSource.ImportValue, Constants.Import.GetCountriesUrlImportName)]
        string[] CountryCodes();
    }
}