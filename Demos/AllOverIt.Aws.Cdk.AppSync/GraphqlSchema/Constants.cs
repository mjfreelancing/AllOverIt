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
            internal const string GetLanguageUrlExportName = "GetLanguageUrl";
            internal const string GetAllContinentsUrlEnvironmentName = "GetAllContinentsUrl";
            internal const string GetPopulationUrl = nameof(GetPopulationUrl);
            internal const string GetCountriesUrlImportName = "GetCountriesUrl";
            internal const string GetCountryCodesUrl = nameof(GetCountryCodesUrl);
        }

        internal static class NoneDataSource
        {
            // Demonstrating use of shared NONE datasources
            internal const string Query = nameof(Query);
            internal const string Mutation = nameof(Mutation);
        }

        internal static class SubscriptionDataSource
        {
            internal const string AddedLanguage = "AddedLanguage";
        }
    }
}