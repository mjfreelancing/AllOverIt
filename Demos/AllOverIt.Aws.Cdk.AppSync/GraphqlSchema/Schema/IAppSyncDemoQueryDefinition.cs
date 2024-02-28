using AllOverIt.Aws.Cdk.AppSync.Attributes.Directives;
using AllOverIt.Aws.Cdk.AppSync.Attributes.Resolvers;
using AllOverIt.Aws.Cdk.AppSync.Attributes.Types;
using AllOverIt.Aws.Cdk.AppSync.Schema;
using AllOverIt.Aws.Cdk.AppSync.Schema.Types;
using GraphqlSchema.Schema.Inputs;
using GraphqlSchema.Schema.Inputs.Globe;
using GraphqlSchema.Schema.Resolvers.Query;
using GraphqlSchema.Schema.Types;
using GraphqlSchema.Schema.Types.Globe;

namespace GraphqlSchema.Schema
{
    // All schema fields must be defined as methods. Properties cannot be used.
    // Cannot use nullable return types or parameters. Use [SchemaTypeRequired] to indicate required, otherwise nullable is assumed.
    internal interface IAppSyncDemoQueryDefinition : IQueryDefinition
    {
        // NOTE: Leave this as the first item as it is testing a parameter and return type that is unknown at the time of parsing
        [UnitResolver(Constants.NoneDataSource.CountryLanguage)]     // Resolver type typeof(CountryLanguageResolver) is registered via code

#if DEBUG   // Using RELEASE mode to deploy without these (DEBUG mode is used to check Synth output)
        [AuthApiKeyDirective]
#endif
        ILanguage CountryLanguage(ICountryFilterInput country);

        // Demonstrates how to obtain the datasource mapping via a user-provided factory
        // ContinentLanguagesResolver does not have a default ctor - it has been registered with the factory
        [UnitResolver(typeof(ContinentLanguagesResolver), Constants.NoneDataSource.ContinentLanguages)]
        ILanguage[] ContinentLanguages([SchemaTypeRequired] IContinentFilterInput filter);

        [UnitResolver(typeof(DefaultLanguageResolver), Constants.NoneDataSource.DefaultLanguage)]
        ILanguage DefaultLanguage();

#if USE_CODE_FIRST_RESOLVERS
        [UnitResolver(Constants.NoneDataSource.Continents)]
#else
        [UnitResolver(typeof(ContinentsResolver), Constants.NoneDataSource.Continents)]
#endif
        IContinent[] Continents([SchemaTypeRequired] IContinentFilterInput filter);

#if USE_CODE_FIRST_RESOLVERS
        [UnitResolver(Constants.HttpDataSource.GetAllContinentsUrlEnvironmentName)]
#else
        [UnitResolver(typeof(AllContinentsResolver), Constants.HttpDataSource.GetAllContinentsUrlEnvironmentName)]
#endif

#if DEBUG   // Using RELEASE mode to deploy without these (DEBUG mode is used to check Synth output)
        [AuthLambdaDirective]
#endif
        IContinentConnection AllContinents();

        // ******************************************************************************************************************************** //
        // NOTE: This can only be deployed after an initial deployment that excludes this (as it requires the export value to be available) //
        // ******************************************************************************************************************************** //
        [SchemaArrayRequired]       // leave here for testing the schema generator
        [SchemaTypeRequired]        // leave here for testing the schema generator
        [UnitResolver(typeof(CountriesResolver), Constants.HttpDataSource.GetCountriesUrlImportName)]
        ICountry[] Countries([SchemaTypeRequired] ICountryFilterInput filter);

        [UnitResolver(typeof(AllCountriesResolver), Constants.NoneDataSource.AllCountries)]
        ICountryConnection AllCountries();

        [UnitResolver(typeof(LanguageResolver), Constants.HttpDataSource.GetLanguageUrlExplicit)]
        ILanguage Language([SchemaTypeRequired] string code);

        [UnitResolver(typeof(LanguagesResolver), Constants.LambdaDataSource.GetLanguages)]
        ILanguage[] Languages(ILanguageFilterInput filter);     // optional filter in this case, implies all languages will be returned if omitted

        [UnitResolver(typeof(GlobeResolver), Constants.NoneDataSource.Globe)]
        IGlobe Globe();

        #region Date, Time, DateTime, Timestamp responses

        [UnitResolver(typeof(CountryDateResolver), Constants.NoneDataSource.CountryDate)]
        AwsTypeDate CountryDate([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType, DateFormat dateFormat);

        [UnitResolver(typeof(CountryTimeResolver), Constants.NoneDataSource.CountryTime)]
        AwsTypeTime CountryTime([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType);

        [UnitResolver(typeof(CountryDateTimeResolver), Constants.NoneDataSource.CountryDateTime)]
        AwsTypeDateTime CountryDateTime([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType);

        [UnitResolver(typeof(CountryTimestampResolver), Constants.NoneDataSource.CountryTimestamp)]
        AwsTypeTimestamp CountryTimestamp([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType);

        #endregion

        #region Date, Time, DateTime, timestamp array responses

        [UnitResolver(typeof(CountryDatesResolver), Constants.NoneDataSource.CountryDates)]
        AwsTypeDate[] CountryDates([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType);

        [UnitResolver(typeof(CountryTimesResolver), Constants.NoneDataSource.CountryTimes)]
        AwsTypeTime[] CountryTimes([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType);

        [UnitResolver(typeof(CountryDateTimesResolver), Constants.NoneDataSource.CountryDateTimes)]
        AwsTypeDateTime[] CountryDateTimes([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType);

        [UnitResolver(typeof(CountryTimestampsResolver), Constants.NoneDataSource.CountryTimestamps)]
        AwsTypeTimestamp[] CountryTimestamps([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType);

        #endregion

        #region Date, Time, DateTime, timestamp input

        [UnitResolver(typeof(CountryByDateResolver), Constants.NoneDataSource.CountryByDate)]
        ICountry CountryByDate([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType,
            [SchemaTypeRequired] AwsTypeDate date);

        [UnitResolver(typeof(CountryByTimeResolver), Constants.NoneDataSource.CountryByTime)]
        ICountry CountryByTime([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType,
            [SchemaTypeRequired] AwsTypeTime time);

        [UnitResolver(typeof(CountryByDateTimeResolver), Constants.NoneDataSource.CountryByDateTime)]
        ICountry CountryByDateTime([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType,
            [SchemaTypeRequired] AwsTypeDateTime dateTime);

        [UnitResolver(typeof(CountryByTimestampResolver), Constants.NoneDataSource.CountryByTimestamp)]
        ICountry CountryByTimestamp([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType,
            [SchemaTypeRequired] AwsTypeTimestamp timestamp);

        #endregion

        #region Date, Time, DateTime, timestamp array input

        [UnitResolver(typeof(CountriesByDatesResolver), Constants.NoneDataSource.CountriesByDates)]
        ICountry[] CountriesByDates([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType,
            [SchemaTypeRequired]
            [SchemaArrayRequired]
            AwsTypeDate[] dates);

        [UnitResolver(typeof(CountriesByTimesResolver), Constants.NoneDataSource.CountriesByTimes)]
        ICountry[] CountriesByTimes([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType,
            [SchemaTypeRequired]
            [SchemaArrayRequired]
            AwsTypeTime[] times);

        [UnitResolver(typeof(CountriesByDateTimesResolver), Constants.NoneDataSource.CountriesByDateTimes)]
        ICountry[] CountriesByDateTimes([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType,
            [SchemaArrayRequired] AwsTypeDateTime[] dateTimes);

        [UnitResolver(typeof(CountriesByTimestampsResolver), Constants.NoneDataSource.CountriesByTimestamps)]
        ICountry[] CountriesByTimestamps([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType,
            [SchemaArrayRequired] AwsTypeTimestamp[] timestamps);

        #endregion
    }
}