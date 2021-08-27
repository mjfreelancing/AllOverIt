﻿using AllOverIt.Aws.Cdk.AppSync;
using AllOverIt.Aws.Cdk.AppSync.Attributes;
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
        // NOTE: Leave this as the first item as it is testing a parameter type that is unknown at the time of parsing
        [RequestResponseMapping(typeof(CountryLanguageMapping))]
        [NoneDataSource(Constants.AppName, nameof(CountryLanguage))]
        ILanguage CountryLanguage([SchemaTypeRequired] ICountryFilterInput country);

        [SchemaArrayRequired]
        [SchemaTypeRequired]
        [RequestResponseMapping(typeof(ContinentsMapping))]
        [NoneDataSource(Constants.AppName, nameof(Continents))]
        IContinent[] Continents([SchemaTypeRequired] IContinentFilterInput filter);

        [SchemaTypeRequired]
        [RequestResponseMapping(typeof(AllContinentsMapping))]
        [HttpDataSource(Constants.AppName, EndpointSource.EnvironmentVariable, Constants.HttpDataSource.GetAllContinentsUrlEnvironmentName)]
        IContinentConnection AllContinents();

        // NOTE: This can only be deployed after an initial deployment that excludes this (as it requires the export value to be available)
        [SchemaArrayRequired]
        [SchemaTypeRequired]
        [RequestResponseMapping(typeof(CountriesMapping))]
        [HttpDataSource(Constants.AppName, EndpointSource.ImportValue, Constants.Import.GetCountriesUrlImportName)]
        ICountry[] Countries([SchemaTypeRequired] ICountryFilterInput filter);

        [SchemaTypeRequired]
        [RequestResponseMapping(typeof(AllCountriesMapping))]
        [NoneDataSource(Constants.AppName, nameof(AllCountries))]
        ICountryConnection AllCountries();

        [SchemaTypeRequired]
        [RequestResponseMapping(typeof(LanguageMapping))]
        [HttpDataSource(Constants.AppName, Constants.HttpDataSource.GetLanguageUrlExplicit)]
        ILanguage Language([SchemaTypeRequired] string code);

        [SchemaArrayRequired]
        [SchemaTypeRequired]
        [RequestResponseMapping(typeof(LanguagesMapping))]
        [LambdaDataSource(Constants.AppName, Constants.Function.GetLanguages)]
        ILanguage[] Languages(ILanguageFilterInput filter);     // optional filter in this case, implies all languages will be returned if omitted

#region Date, Time, DateTime, Timestamp responses

        [SchemaTypeRequired]
        [RequestResponseMapping(typeof(CountryDateMapping))]
        [NoneDataSource(Constants.AppName, nameof(CountryDate))]
        AwsTypeDate CountryDate([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType);

        [SchemaTypeRequired]
        [RequestResponseMapping(typeof(CountryTimeMapping))]
        [NoneDataSource(Constants.AppName, nameof(CountryTime))]
        AwsTypeTime CountryTime([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType);

        [SchemaTypeRequired]
        [RequestResponseMapping(typeof(CountryDateTimeMapping))]
        [NoneDataSource(Constants.AppName, nameof(CountryDateTime))]
        AwsTypeDateTime CountryDateTime([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType);

        [SchemaTypeRequired]
        [RequestResponseMapping(typeof(CountryTimestampMapping))]
        [NoneDataSource(Constants.AppName, nameof(CountryTimestamp))]
        AwsTypeTimestamp CountryTimestamp([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType);

#endregion

#region Date, Time, DateTime, timestamp array responses

        [SchemaArrayRequired]
        [SchemaTypeRequired]
        [RequestResponseMapping(typeof(CountryDatesMapping))]
        [NoneDataSource(Constants.AppName, nameof(CountryDates))]
        AwsTypeDate[] CountryDates([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType);

        [SchemaArrayRequired]
        [SchemaTypeRequired]
        [RequestResponseMapping(typeof(CountryTimesMapping))]
        [NoneDataSource(Constants.AppName, nameof(CountryTimes))]
        AwsTypeTime[] CountryTimes([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType);

        [SchemaArrayRequired]
        [SchemaTypeRequired]
        [RequestResponseMapping(typeof(CountryDateTimesMapping))]
        [NoneDataSource(Constants.AppName, nameof(CountryDateTimes))]
        AwsTypeDateTime[] CountryDateTimes([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType);

        [SchemaArrayRequired]
        [SchemaTypeRequired]
        [RequestResponseMapping(typeof(CountryTimestampsMapping))]
        [NoneDataSource(Constants.AppName, nameof(CountryTimestamps))]
        AwsTypeTimestamp[] CountryTimestamps([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType);

#endregion

#region Date, Time, DateTime, timestamp input

        [SchemaTypeRequired]
        [RequestResponseMapping(typeof(CountryByDateMapping))]
        [NoneDataSource(Constants.AppName, nameof(CountryByDate))]
        ICountry CountryByDate([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType,
            [SchemaTypeRequired] AwsTypeDate date);

        [SchemaTypeRequired]
        [RequestResponseMapping(typeof(CountryByTimeMapping))]
        [NoneDataSource(Constants.AppName, nameof(CountryByTime))]
        ICountry CountryByTime([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType,
            [SchemaTypeRequired] AwsTypeTime time);

        [SchemaTypeRequired]
        [RequestResponseMapping(typeof(CountryByDateTimeMapping))]
        [NoneDataSource(Constants.AppName, nameof(CountryByDateTime))]
        ICountry CountryByDateTime([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType,
            [SchemaTypeRequired] AwsTypeDateTime dateTime);

        [SchemaTypeRequired]
        [RequestResponseMapping(typeof(CountryByTimestampMapping))]
        [NoneDataSource(Constants.AppName, nameof(CountryByTimestamp))]
        ICountry CountryByTimestamp([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType,
            [SchemaTypeRequired] AwsTypeTimestamp timestamp);

#endregion

#region Date, Time, DateTime, timestamp array input

        [SchemaArrayRequired]
        [SchemaTypeRequired]
        [RequestResponseMapping(typeof(CountriesByDatesMapping))]
        [NoneDataSource(Constants.AppName, nameof(CountriesByDates))]
        ICountry[] CountriesByDates([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType,
            [SchemaTypeRequired]
            [SchemaArrayRequired]
            AwsTypeDate[] dates);

        [SchemaArrayRequired]
        [SchemaTypeRequired]
        [RequestResponseMapping(typeof(CountriesByTimesMapping))]
        [NoneDataSource(Constants.AppName, nameof(CountriesByTimes))]
        ICountry[] CountriesByTimes([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType,
            [SchemaTypeRequired]
            [SchemaArrayRequired]
            AwsTypeTime[] times);

        [SchemaArrayRequired]
        [SchemaTypeRequired]
        [RequestResponseMapping(typeof(CountriesByDateTimesMapping))]
        [NoneDataSource(Constants.AppName, nameof(CountriesByDateTimes))]
        ICountry[] CountriesByDateTimes([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType,
            [SchemaArrayRequired] AwsTypeDateTime[] dateTimes);

        [SchemaArrayRequired]
        [SchemaTypeRequired]
        [RequestResponseMapping(typeof(CountriesByTimestampsMapping))]
        [NoneDataSource(Constants.AppName, nameof(CountriesByTimestamps))]
        ICountry[] CountriesByTimestamps([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType,
            [SchemaArrayRequired] AwsTypeTimestamp[] timestamps);

#endregion
    }
}