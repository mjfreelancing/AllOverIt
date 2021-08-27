using AllOverIt.Aws.Cdk.AppSync;
using AllOverIt.Aws.Cdk.AppSync.Attributes;
using GraphqlSchema.Schema.Mappings.Query;

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
        [HttpDataSource(Constants.AppName, EndpointSource.ImportValue, Constants.Import.GetCountriesUrlImportName, typeof(ContinentsCountriesMapping))]
        ICountry[] Countries();

        // sharing this http datasource
        [SchemaArrayRequired]
        [SchemaTypeRequired]
        [HttpDataSource(Constants.AppName, EndpointSource.ImportValue, Constants.Import.GetCountriesUrlImportName, typeof(ContinentsCountryCodesMapping))]
        string[] CountryCodes();
    }
}