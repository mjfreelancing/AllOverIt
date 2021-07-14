using AllOverIt.Aws.Cdk.AppSync;
using AllOverIt.Aws.Cdk.AppSync.Attributes;
using AllOverIt.Aws.Cdk.AppSync.Schema;
using AllOverIt.Aws.Cdk.AppSync.Schema.Types;
using GraphqlSchema.Schema.Inputs;
using GraphqlSchema.Schema.Types;

namespace GraphqlSchema.Schema
{
    internal interface IAppSyncDemoQueryDefinition : IQueryDefinition
    {
        [SchemaArrayRequired]
        [SchemaTypeRequired]
        //[NoneDataSource(Constants.AppName, nameof(Continents))]
        IContinent[] Continents([SchemaTypeRequired] IContinentFilterInput filter);

        [SchemaTypeRequired]
        [HttpDataSource(Constants.AppName, EndpointSource.EnvironmentVariable, Constants.HttpDataSource.GetAllContinentsUrlEnvironmentName)]
        IContinentConnection AllContinents();

        // NOTE: This can only be deployed after an initial deployment that excludes this (as it requires the export value to be available)
        [SchemaArrayRequired]
        [SchemaTypeRequired]
        [HttpDataSource(Constants.AppName, EndpointSource.ImportValue, Constants.Import.GetCountriesUrlImportName)]
        ICountry[] Countries([SchemaTypeRequired] ICountryFilterInput filter);

        [SchemaTypeRequired]
        [NoneDataSource(Constants.AppName, nameof(AllCountries))]
        ICountryConnection AllCountries();

        [SchemaTypeRequired]
        [HttpDataSource(Constants.AppName, Constants.HttpDataSource.GetLanguageUrlExplicit)]
        ILanguage Language([SchemaTypeRequired] string code);

        [SchemaArrayRequired]
        [SchemaTypeRequired]
        [LambdaDataSource(Constants.AppName, Constants.Function.GetLanguages)]
        ILanguage[] Languages(ILanguageFilterInput filter);     // optional filter in this case, implies all languages will be returned if omitted

        #region Date, Time, DateTime, Timestamp responses
       
        [SchemaTypeRequired]
        [NoneDataSource(Constants.AppName, nameof(CountryDate))]
        AwsTypeDate CountryDate([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType);

        [SchemaTypeRequired]
        [NoneDataSource(Constants.AppName, nameof(CountryTime))]
        AwsTypeTime CountryTime([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType);

        [SchemaTypeRequired]
        [NoneDataSource(Constants.AppName, nameof(CountryDateTime))]
        AwsTypeDateTime CountryDateTime([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType);

        [SchemaTypeRequired]
        [NoneDataSource(Constants.AppName, nameof(CountryTimestamp))]
        AwsTypeTimestamp CountryTimestamp([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType);

        #endregion

        #region Date, Time, DateTime, timestamp array responses

        [SchemaArrayRequired]
        [SchemaTypeRequired]
        [NoneDataSource(Constants.AppName, nameof(CountryDates))]
        AwsTypeDate[] CountryDates([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType);

        [SchemaArrayRequired]
        [SchemaTypeRequired]
        [NoneDataSource(Constants.AppName, nameof(CountryTimes))]
        AwsTypeTime[] CountryTimes([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType);

        [SchemaArrayRequired]
        [SchemaTypeRequired]
        [NoneDataSource(Constants.AppName, nameof(CountryDateTimes))]
        AwsTypeDateTime[] CountryDateTimes([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType);

        [SchemaArrayRequired]
        [SchemaTypeRequired]
        [NoneDataSource(Constants.AppName, nameof(CountryTimestamps))]
        AwsTypeTimestamp[] CountryTimestamps([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType);

        #endregion

        #region Date, Time, DateTime, timestamp input

        [SchemaTypeRequired]
        [NoneDataSource(Constants.AppName, nameof(CountryByDate))]
        ICountry CountryByDate([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType,
            [SchemaTypeRequired] AwsTypeDate date);

        [SchemaTypeRequired]
        [NoneDataSource(Constants.AppName, nameof(CountryByTime))]
        ICountry CountryByTime([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType,
            [SchemaTypeRequired] AwsTypeTime time);

        [SchemaTypeRequired]
        [NoneDataSource(Constants.AppName, nameof(CountryByDateTime))]
        ICountry CountryByDateTime([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType,
            [SchemaTypeRequired] AwsTypeDateTime dateTime);

        [SchemaTypeRequired]
        [NoneDataSource(Constants.AppName, nameof(CountryByTimestamp))]
        ICountry CountryByTimestamp([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType,
            [SchemaTypeRequired] AwsTypeTimestamp timestamp);

        #endregion

        #region Date, Time, DateTime, timestamp array input

        [SchemaArrayRequired]
        [SchemaTypeRequired]
        [NoneDataSource(Constants.AppName, nameof(CountriesByDates))]
        ICountry[] CountriesByDates([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType,
            [SchemaTypeRequired]
            [SchemaArrayRequired]
            AwsTypeDate[] dates);

        [SchemaArrayRequired]
        [SchemaTypeRequired]
        [NoneDataSource(Constants.AppName, nameof(CountriesByTimes))]
        ICountry[] CountriesByTimes([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType,
            [SchemaTypeRequired]
            [SchemaArrayRequired]
            AwsTypeTime[] times);

        [SchemaArrayRequired]
        [SchemaTypeRequired]
        [NoneDataSource(Constants.AppName, nameof(CountriesByDateTimes))]
        ICountry[] CountriesByDateTimes([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType,
            [SchemaArrayRequired] AwsTypeDateTime[] dateTimes);

        [SchemaArrayRequired]
        [SchemaTypeRequired]
        [NoneDataSource(Constants.AppName, nameof(CountriesByTimestamps))]
        ICountry[] CountriesByTimestamps([SchemaTypeRequired] GraphqlTypeId countryId, [SchemaTypeRequired] DateType dateType,
            [SchemaArrayRequired] AwsTypeTimestamp[] timestamps);

        #endregion
    }
}