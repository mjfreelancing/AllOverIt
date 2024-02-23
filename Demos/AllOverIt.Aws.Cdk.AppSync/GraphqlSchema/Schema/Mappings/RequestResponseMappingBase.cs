using AllOverIt.Aws.Cdk.AppSync.Mapping;
using AllOverIt.Formatters.Strings.Extensions;

namespace GraphqlSchema.Schema.Mappings
{
    internal abstract class RequestResponseMappingBase : IRequestResponseMapping
    {
        public string RequestMapping { get; protected set; }
        public string ResponseMapping { get; protected set; }
        public string Code { get; protected set; }

        protected static string GetNoneRequestMapping()
        {
            // Using FormatJsonString() to remove the extra padding
            return @"
                    {
                      ""version"": ""2017-02-28"",
                      ""payload"": ""{}""
                    }".FormatJsonString();
        }

        protected static string GetNoneResponseMapping()
        {
            return "$util.toJson($ctx.result)";
        }

        protected static string GetFunctionRequestMapping()
        {
            // Using FormatJsonString() to remove the extra padding
            return @"
                    {
                      ""version"": ""2017-02-28"",
                      ""operation"": ""Invoke"",
                      ""payload"": $util.toJson($ctx.args)
                    }".FormatJsonString();
        }

        protected static string GetFunctionResponseMapping()
        {
            return "$util.toJson($ctx.result.payload)";
        }

        protected static string GetHttpRequestMapping(string verb, string resourcePath)
        {
            // Using FormatJsonString() to remove the extra padding
            return $@"
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
                    }}".FormatJsonString();
        }

        protected static string GetHttpResponseMapping()
        {
            return "$util.toJson($ctx.result.body)";
        }

        protected static string GetCodeMapping()
        {
            return @"
                    function request() {
                    };

                    function response() {
                    };";
        }
    }
}