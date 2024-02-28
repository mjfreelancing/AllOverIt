using AllOverIt.Aws.Cdk.AppSync.Attributes.Resolvers;
using AllOverIt.Aws.Cdk.AppSync.Attributes.Directives;
using GraphqlSchema.Attributes;

#if !USE_CODE_FIRST_RESOLVERS
using GraphqlSchema.Schema.Resolvers.Query;
#endif

namespace GraphqlSchema.Schema.Types.Globe
{
    // Testing the use of namespaces via an inherited SchemaTypeAttribute => should produce 'GlobeContinent' (the last argument can be null/empty)
    [GlobeSchemaType("Continent")]

#if DEBUG   // Using RELEASE mode to deploy without these (DEBUG mode is used to check Synth output)
    [AuthCognitoDirective("group1", "group2")]
    [AuthApiKeyDirective]
    [AuthIamDirective]
    [AuthOidcDirective]
    [AuthLambdaDirective]
#endif
    internal interface IContinent : ISchemaTypeBase
    {
        string Name();

#if USE_CODE_FIRST_RESOLVERS
        [UnitResolver(Constants.Import.GetCountriesUrlImportName)]
#else
        [UnitResolver(typeof(ContinentsCountriesResolver), Constants.HttpDataSource.GetCountriesUrlImportName)]
#endif

#if DEBUG   // Using RELEASE mode to deploy without these (DEBUG mode is used to check Synth output)
        [AuthApiKeyDirective]
#endif
        ICountry[] Countries();

#if USE_CODE_FIRST_RESOLVERS
        [UnitResolver(Constants.Lookup.GetCountriesUrlKey)]
#else
        [UnitResolver(typeof(ContinentsCountryCodesResolver), Constants.HttpDataSource.GetCountriesUrlLookupKey)]
#endif

#if DEBUG   // Using RELEASE mode to deploy without these (DEBUG mode is used to check Synth output)
        [AuthIamDirective]
#endif
        string[] CountryCodes();

        DateFormat[] DateFormats();
    }
}