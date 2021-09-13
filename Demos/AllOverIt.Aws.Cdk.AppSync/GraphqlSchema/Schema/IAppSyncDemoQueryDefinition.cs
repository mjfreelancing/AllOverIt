using AllOverIt.Aws.Cdk.AppSync;
using AllOverIt.Aws.Cdk.AppSync.Attributes.Datasources;
using AllOverIt.Aws.Cdk.AppSync.Attributes.Directives;
using AllOverIt.Aws.Cdk.AppSync.Attributes.Types;
using AllOverIt.Aws.Cdk.AppSync.Schema;
using AllOverIt.Aws.Cdk.AppSync.Schema.Types;
using GraphqlSchema.Schema.Inputs;
using GraphqlSchema.Schema.Mappings.Query;
using GraphqlSchema.Schema.Types;

namespace GraphqlSchema.Schema
{
    // All schema fields must be defined as methods. Properties cannot be used.
    // Cannot use nullable return types are parameters. Use [SchemaTypeRequired] to indicate required, otherwise nullable is assumed.
    internal interface IAppSyncDemoQueryDefinition : IQueryDefinition
    {
        // NOTE: Leave this as the first item as it is testing a parameter and return type that is unknown at the time of parsing
        [NoneDataSource(nameof(CountryLanguage)/*, typeof(CountryLanguageMapping)*/)]        // providing this mapping via code
        [AuthApiKeyDirective]
        ILanguage CountryLanguage([SchemaTypeRequired] ICountryFilterInput country);

        // demonstrates how to obtain the datasource mapping via a user-provided factory
        // ContinentLanguagesMapping does not have a default ctor - it has been registered with the factory
        [NoneDataSource(nameof(ContinentLanguages), typeof(ContinentLanguagesMapping))]
        ILanguage[] ContinentLanguages([SchemaTypeRequired] IContinentFilterInput filter);

        [NoneDataSource(nameof(DefaultLanguage), typeof(DefaultLanguageMapping))]
        ILanguage DefaultLanguage();

        [NoneDataSource(nameof(Continents), typeof(ContinentsMapping))]
        IContinent[] Continents([SchemaTypeRequired] IContinentFilterInput filter);

        [HttpDataSource(EndpointSource.EnvironmentVariable, Constants.HttpDataSource.GetAllContinentsUrlEnvironmentName, typeof(AllContinentsMapping))]
        IContinentConnection AllContinents();

        // ******************************************************************************************************************************** //
        // NOTE: This can only be deployed after an initial deployment that excludes this (as it requires the export value to be available) //
        // ******************************************************************************************************************************** //
        [SchemaArrayRequired]       // leave here for testing the schema generator
        [SchemaTypeRequired]        // leave here for testing the schema generator
        [HttpDataSource(EndpointSource.ImportValue, Constants.Import.GetCountriesUrlImportName, typeof(CountriesMapping))]
        ICountry[] Countries([SchemaTypeRequired] ICountryFilterInput filter);

        [NoneDataSource(nameof(AllCountries), typeof(AllCountriesMapping))]
        ICountryConnection AllCountries();

        [HttpDataSource(Constants.HttpDataSource.GetLanguageUrlExplicit, typeof(LanguageMapping))]
        ILanguage Language([SchemaTypeRequired] string code);

        [LambdaDataSource(Constants.Function.GetLanguages, typeof(LanguagesMapping))]
        ILanguage[] Languages(ILanguageFilterInput filter);     // optional filter in this case, implies all languages will be returned if omitted

#region Date, Time, DateTime, Timestamp responses

        [NoneDataSource(nameof(CountryDate), typeof(CountryDateMapping))]
        AwsTypeDate CountryDate([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType);

        [NoneDataSource(nameof(CountryTime), typeof(CountryTimeMapping))]
        AwsTypeTime CountryTime([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType);

        [NoneDataSource(nameof(CountryDateTime), typeof(CountryDateTimeMapping))]
        AwsTypeDateTime CountryDateTime([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType);

        [NoneDataSource(nameof(CountryTimestamp), typeof(CountryTimestampMapping))]
        AwsTypeTimestamp CountryTimestamp([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType);

#endregion

#region Date, Time, DateTime, timestamp array responses

        [NoneDataSource(nameof(CountryDates), typeof(CountryDatesMapping))]
        AwsTypeDate[] CountryDates([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType);

        [NoneDataSource(nameof(CountryTimes), typeof(CountryTimesMapping))]
        AwsTypeTime[] CountryTimes([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType);

        [NoneDataSource(nameof(CountryDateTimes), typeof(CountryDateTimesMapping))]
        AwsTypeDateTime[] CountryDateTimes([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType);

        [NoneDataSource(nameof(CountryTimestamps), typeof(CountryTimestampsMapping))]
        AwsTypeTimestamp[] CountryTimestamps([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType);

#endregion

#region Date, Time, DateTime, timestamp input

        [NoneDataSource(nameof(CountryByDate), typeof(CountryByDateMapping))]
        ICountry CountryByDate([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType,
            [SchemaTypeRequired] AwsTypeDate date);

        [NoneDataSource(nameof(CountryByTime), typeof(CountryByTimeMapping))]
        ICountry CountryByTime([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType,
            [SchemaTypeRequired] AwsTypeTime time);

        [NoneDataSource(nameof(CountryByDateTime), typeof(CountryByDateTimeMapping))]
        ICountry CountryByDateTime([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType,
            [SchemaTypeRequired] AwsTypeDateTime dateTime);

        [NoneDataSource(nameof(CountryByTimestamp), typeof(CountryByTimestampMapping))]
        ICountry CountryByTimestamp([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType,
            [SchemaTypeRequired] AwsTypeTimestamp timestamp);

#endregion

#region Date, Time, DateTime, timestamp array input

        [NoneDataSource(nameof(CountriesByDates), typeof(CountriesByDatesMapping))]
        ICountry[] CountriesByDates([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType,
            [SchemaTypeRequired]
            [SchemaArrayRequired]
            AwsTypeDate[] dates);

        [NoneDataSource(nameof(CountriesByTimes), typeof(CountriesByTimesMapping))]
        ICountry[] CountriesByTimes([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType,
            [SchemaTypeRequired]
            [SchemaArrayRequired]
            AwsTypeTime[] times);

        [NoneDataSource(nameof(CountriesByDateTimes), typeof(CountriesByDateTimesMapping))]
        ICountry[] CountriesByDateTimes([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType,
            [SchemaArrayRequired] AwsTypeDateTime[] dateTimes);

        [NoneDataSource(nameof(CountriesByTimestamps), typeof(CountriesByTimestampsMapping))]
        ICountry[] CountriesByTimestamps([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType,
            [SchemaArrayRequired] AwsTypeTimestamp[] timestamps);

#endregion
    }
}