using AllOverIt.Aws.Cdk.AppSync.Mapping;
using AllOverIt.Utils;

namespace GraphqlSchema.Schema.Mappings
{
    internal abstract class RequestResponseMappingBase : IRequestResponseMapping
    {
        public abstract string RequestMapping { get; }
        public abstract string ResponseMapping { get; }

        protected static string GetNoneRequestMapping()
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

        protected static string GetNoneResponseMapping()
        {
            return "$util.toJson($ctx.result)";
        }

        protected static string GetFunctionRequestMapping()
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

        protected static string GetFunctionResponseMapping()
        {
            return "$util.toJson($ctx.result.payload)";
        }

        protected static string GetHttpRequestMapping(string verb, string resourcePath)
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

        protected static string GetHttpResponseMapping()
        {
            return "$util.toJson($ctx.result.body)";
        }
    }
}