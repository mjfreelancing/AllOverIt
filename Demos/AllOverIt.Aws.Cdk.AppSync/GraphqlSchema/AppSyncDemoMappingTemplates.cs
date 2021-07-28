using AllOverIt.Aws.Cdk.AppSync.MappingTemplates;
using AllOverIt.Utils;

namespace GraphqlSchema
{
    internal sealed class AppSyncDemoMappingTemplates : MappingTemplatesBase
    {
        public AppSyncDemoMappingTemplates()
        {
            RegisterMappings("Query.Continents", GetNoneRequestMapping(), GetNoneResponsetMapping());
            RegisterMappings("Query.Continents.Countries", GetHttpRequestMapping("GET", "/countries"), GetHttpResponseMapping());
            RegisterMappings("Query.Continents.CountryCodes", GetHttpRequestMapping("GET", "/countries"), GetHttpResponseMapping());
            RegisterMappings("Query.AllContinents", GetHttpRequestMapping("GET", "/continents"), GetHttpResponseMapping());
            RegisterMappings("Query.Countries", GetHttpRequestMapping("GET", "/countries"), GetHttpResponseMapping());
            RegisterMappings("Query.AllCountries", GetNoneRequestMapping(), GetNoneResponsetMapping());
            RegisterMappings("Query.Language", GetHttpRequestMapping("GET", "/language"), GetHttpResponseMapping());
            RegisterMappings("Query.Languages", GetFunctionRequestMapping(), GetFunctionResponsetMapping());
            RegisterMappings("Query.CountryDate", GetNoneRequestMapping(), GetNoneResponsetMapping());
            RegisterMappings("Query.CountryTime", GetNoneRequestMapping(), GetNoneResponsetMapping());
            RegisterMappings("Query.CountryDateTime", GetNoneRequestMapping(), GetNoneResponsetMapping());
            RegisterMappings("Query.CountryTimestamp", GetNoneRequestMapping(), GetNoneResponsetMapping());
            RegisterMappings("Query.CountryDates", GetNoneRequestMapping(), GetNoneResponsetMapping());
            RegisterMappings("Query.CountryTimes", GetNoneRequestMapping(), GetNoneResponsetMapping());
            RegisterMappings("Query.CountryDateTimes", GetNoneRequestMapping(), GetNoneResponsetMapping());
            RegisterMappings("Query.CountryTimestamps", GetNoneRequestMapping(), GetNoneResponsetMapping());
            RegisterMappings("Query.CountryByDate", GetNoneRequestMapping(), GetNoneResponsetMapping());
            RegisterMappings("Query.CountryByTime", GetNoneRequestMapping(), GetNoneResponsetMapping());
            RegisterMappings("Query.CountryByDateTime", GetNoneRequestMapping(), GetNoneResponsetMapping());
            RegisterMappings("Query.CountryByTimestamp", GetNoneRequestMapping(), GetNoneResponsetMapping());
            RegisterMappings("Query.CountriesByDates", GetNoneRequestMapping(), GetNoneResponsetMapping());
            RegisterMappings("Query.CountriesByTimes", GetNoneRequestMapping(), GetNoneResponsetMapping());
            RegisterMappings("Query.CountriesByDateTimes", GetNoneRequestMapping(), GetNoneResponsetMapping());
            RegisterMappings("Query.CountriesByTimestamps", GetNoneRequestMapping(), GetNoneResponsetMapping());

            RegisterMappings("Mutation.AddCountry", GetFunctionRequestMapping(), GetFunctionResponsetMapping());
            RegisterMappings("Mutation.UpdateCountry", GetFunctionRequestMapping(), GetFunctionResponsetMapping());

            RegisterMappings("Subscription.AddedCountry", GetNoneRequestMapping(), GetNoneResponsetMapping());
        }

        private static string GetNoneRequestMapping()
        {
            // Using FormatJsonString() to remove the extra padding
            return Formatter.FormatJsonString(
                @"
                    {
                      ""version"": ""2018-05-09"",
                      ""payload"": ""true""
                    }"
            );
        }

        private static string GetNoneResponsetMapping()
        {
            return "$util.toJson($ctx.result)";
        }

        private static string GetFunctionRequestMapping()
        {
            // Using FormatJsonString() to remove the extra padding
            return Formatter.FormatJsonString(
                @"
                    {
                      ""version"": ""2017-02-28"",
                      ""operation"": ""Invoke"",
                      ""payload"": $util.toJson($ctx.args)
                    }"
            );
        }

        private static string GetFunctionResponsetMapping()
        {
            return "$util.toJson($ctx.result.payload)";
        }

        private static string GetHttpRequestMapping(string verb, string resourcePath)
        {
            // Using FormatJsonString() to remove the extra padding
            return Formatter.FormatJsonString(
                $@"
                    {{
                      ""version"": ""2018-05-29"",
                      ""method"": ""{verb}"",
                      ""resourcePath"": ""{resourcePath}"",
                      ""params"": {{
                        ""headers"": {{
                          ""Content-Type"": ""application/json"",
                          ""X-Api-Key"": ""some-key-here""
                        }}
                      }},
                      ""query"": {{
                        ""param1"": ""$ctx.source.id"",
                        ""param2"": ""$ctx.args.id"",
                        ""email"": ""$ctx.identity.claims['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress']""
                      }},
                    }}"
            );
        }

        private static string GetHttpResponseMapping()
        {
            return "$util.toJson($ctx.result.body)";
        }
    }
}