using AllOverIt.Utils;

namespace GraphqlSchema.Schema.Mappings
{
    internal abstract class HttpGetResponseMappingBase : RequestResponseMappingBase
    {
        public override string RequestMapping { get; }
        public override string ResponseMapping { get; }

        public HttpGetResponseMappingBase(string resource, string apiKey)
        {
            RequestMapping = GetHttpRequestMapping("GET", resource, apiKey);
            ResponseMapping = GetHttpResponseMapping();
        }

        private static string GetHttpRequestMapping(string verb, string resourcePath, string apiKey)
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
                          ""X-Api-Key"": ""{apiKey}""
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
    }
}