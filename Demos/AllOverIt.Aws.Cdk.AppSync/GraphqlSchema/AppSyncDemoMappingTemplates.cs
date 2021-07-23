using AllOverIt.Aws.Cdk.AppSync.Helpers;
using AllOverIt.Aws.Cdk.AppSync.MappingTemplates;
using Amazon.CDK.AWS.AppSync;

namespace GraphqlSchema
{
    internal sealed class AppSyncDemoMappingTemplates : MappingTemplatesBase
    {
        public AppSyncDemoMappingTemplates()
{
            // WIP - just here for testing atm
            RegisterMappings("Query.Continents", "Query.Continents", "Query.Continents");
            RegisterMappings("Query.Continents.Countries", "Query.Continents.Countries", "Query.Continents.Countries");
            RegisterMappings("Query.Continents.CountryCodes", "Query.Continents.CountryCodes", "Query.Continents.CountryCodes");
            RegisterMappings("Query.AllContinents", "Query.AllContinents", "Query.AllContinents");
            RegisterMappings("Query.Countries", "Query.Countries", "Query.Countries");
            RegisterMappings("Query.AllCountries", "Query.AllCountries", "Query.AllCountries");
            RegisterMappings("Query.Language", "Query.Language", "Query.Language");
            RegisterMappings("Query.Languages", "Query.Languages", "Query.Languages");
            RegisterMappings("Query.CountryDate", "Query.CountryDate", "Query.CountryDate");
            RegisterMappings("Query.CountryTime", "Query.CountryTime", "Query.CountryTime");
            RegisterMappings("Query.CountryDateTime", "Query.CountryDateTime", "Query.CountryDateTime");
            RegisterMappings("Query.CountryTimestamp", "Query.CountryTimestamp", "Query.CountryTimestamp");
            RegisterMappings("Query.CountryDates", "Query.CountryDates", "Query.CountryDates");
            RegisterMappings("Query.CountryTimes", "Query.CountryTimes", "Query.CountryTimes");
            RegisterMappings("Query.CountryDateTimes", "Query.CountryDateTimes", "Query.CountryDateTimes");
            RegisterMappings("Query.CountryTimestamps", "Query.CountryTimestamps", "Query.CountryTimestamps");
            RegisterMappings("Query.CountryByDate", "Query.CountryByDate", "Query.CountryByDate");
            RegisterMappings("Query.CountryByTime", "Query.CountryByTime", "Query.CountryByTime");
            RegisterMappings("Query.CountryByDateTime", "Query.CountryByDateTime", "Query.CountryByDateTime");
            RegisterMappings("Query.CountryByTimestamp", "Query.CountryByTimestamp", "Query.CountryByTimestamp");
            RegisterMappings("Query.CountriesByDates", "Query.CountriesByDates", "Query.CountriesByDates");
            RegisterMappings("Query.CountriesByTimes", "Query.CountriesByTimes", "Query.CountriesByTimes");
            RegisterMappings("Query.CountriesByDateTimes", "Query.CountriesByDateTimes", "Query.CountriesByDateTimes");
            RegisterMappings("Query.CountriesByTimestamps", "Query.CountriesByTimestamps", "Query.CountriesByTimestamps");

            RegisterMappings("Mutation.AddCountry", "Mutation.AddCountry", "Mutation.AddCountry");
            RegisterMappings("Mutation.UpdateCountry", "Mutation.UpdateCountry", "Mutation.UpdateCountry");

            RegisterMappings("Subscription.AddedCountry", "Subscription.AddedCountry", "Subscription.AddedCountry");
        }
    }
}