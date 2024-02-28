namespace GraphqlSchema
{
    internal static class Constants
    {
        internal const string AppName = "AppSyncDemo";
        internal const int ServiceVersion = 1;

        internal static class LambdaDataSource
        {
            internal const string GetLanguages = "GetLanguages";
            internal const string AddCountry = "AddCountry";
            internal const string UpdateCountry = "UpdateCountry";
        }

        internal static class HttpDataSource
        {
            // need a real URL for the deployment to succeed
            internal const string GetLanguageUrlExplicit = "https://www.google.com";
            internal const string GetAllContinentsUrlEnvironmentName = "GetAllContinents";
            internal const string GetPopulationUrlExplicit = "https://www.microsoft.com";
            internal const string GetCountriesUrlImportName = "GetCountriesImport";
            internal const string GetCountriesUrlLookupKey = "GetCountriesLookup";
        }

        internal static class NoneDataSource
        {
            // need a real URL for the deployment to succeed
            internal const string AddLanguage = nameof(AddLanguage);
            internal const string CountryLanguage = nameof(CountryLanguage);
            internal const string ContinentLanguages = nameof(ContinentLanguages);
            internal const string DefaultLanguage = nameof(DefaultLanguage);
            internal const string Continents = nameof(Continents);
            internal const string AllCountries = nameof(AllCountries);
            internal const string Globe = nameof(Globe);
            internal const string CountryDate = nameof(CountryDate);
            internal const string CountryTime = nameof(CountryTime);
            internal const string CountryDateTime = nameof(CountryDateTime);
            internal const string CountryTimestamp = nameof(CountryTimestamp);
            internal const string CountryDates = nameof(CountryDates);
            internal const string CountryTimes = nameof(CountryTimes);
            internal const string CountryDateTimes = nameof(CountryDateTimes);
            internal const string CountryTimestamps = nameof(CountryTimestamps);
            internal const string CountryByDate = nameof(CountryByDate);
            internal const string CountryByTime = nameof(CountryByTime);
            internal const string CountryByDateTime = nameof(CountryByDateTime);
            internal const string CountryByTimestamp = nameof(CountryByTimestamp);
            internal const string CountriesByDates = nameof(CountriesByDates);
            internal const string CountriesByTimes = nameof(CountriesByTimes);
            internal const string CountriesByDateTimes = nameof(CountriesByDateTimes);
            internal const string CountriesByTimestamps = nameof(CountriesByTimestamps);
        }

        internal static class SubscriptionDataSource
        {
            internal const string AddedLanguage = "AddedLanguage";
        }
    }
}