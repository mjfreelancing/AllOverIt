namespace GraphqlSchema
{
    internal static class Constants
    {
        internal const string AppName = "AppSyncDemo";
        internal const int ServiceVersion = 1;

        internal static class Function
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
        }

        internal static class Import
        {
            internal const string GetCountriesUrlImportName = "GetCountriesImport";
        }

        internal static class Lookup
        {
            internal const string GetCountriesUrlKey = "GetCountriesLookup";
        }
    }
}