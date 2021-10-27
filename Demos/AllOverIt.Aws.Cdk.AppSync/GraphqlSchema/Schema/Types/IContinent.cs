using AllOverIt.Aws.Cdk.AppSync;
using AllOverIt.Aws.Cdk.AppSync.Attributes.DataSources;
using AllOverIt.Aws.Cdk.AppSync.Attributes.Directives;
using AllOverIt.Aws.Cdk.AppSync.Attributes.Types;
using GraphqlSchema.Schema.Mappings.Query;

namespace GraphqlSchema.Schema.Types
{
    [SchemaType("Continent")]
    [AuthCognitoDirective("group1", "group2")]
    [AuthApiKeyDirective]
    [AuthIamDirective]
    [AuthOidcDirective]
    internal interface IContinent : ISchemaTypeBase
    {
        string Name();

        // sharing this http datasource - just for testing the schema generator
        [HttpDataSource(EndpointSource.ImportValue, Constants.Import.GetCountriesUrlImportName, typeof(ContinentsCountriesMapping))]
        [AuthApiKeyDirective]
        ICountry[] Countries();

        // sharing this http datasource - just for testing the schema generator
        [HttpDataSource(EndpointSource.ImportValue, Constants.Import.GetCountriesUrlImportName, typeof(ContinentsCountryCodesMapping))]
        [AuthIamDirective]
        string[] CountryCodes();
    }
}