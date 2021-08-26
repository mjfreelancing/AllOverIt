using AllOverIt.Aws.Cdk.AppSync;
using AllOverIt.Aws.Cdk.AppSync.Attributes;
using GraphqlSchema.Schema.Mappings;
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
        [RequestResponseMapping(typeof(ContinentsCountriesMapping))]
        [HttpDataSource(Constants.AppName, EndpointSource.ImportValue, Constants.Import.GetCountriesUrlImportName)]
        ICountry[] Countries();

        // sharing this http datasource
        [SchemaArrayRequired]
        [SchemaTypeRequired]
        [RequestResponseMapping(typeof(ContinentsCountryCodesMapping))]
        [HttpDataSource(Constants.AppName, EndpointSource.ImportValue, Constants.Import.GetCountriesUrlImportName)]
        string[] CountryCodes();
    }
}