using AllOverIt.Aws.Cdk.AppSync.Resolvers;

namespace GraphqlSchema.Schema.Resolvers
{
    internal abstract class VtlResolverBase : IVtlRuntime
    {
        public string RequestMapping { get; protected set; }
        public string ResponseMapping { get; protected set; }

        protected VtlResolverBase()
        {
            ResponseMapping = "$util.toJson($ctx.result.body)";
        }

        protected void SetHttpRequestMapping(string verb, string resourcePath)
        {
            RequestMapping = $$"""
                    {
                      "version": "2018-05-29",
                      "method": "{{verb}}",
                      "resourcePath": "{{resourcePath}}",
                      "params": {
                        "headers": {
                          "Content-Type": "application/json",
                          "X-Api-Key": "some-key-here"
                        }
                      },
                      "query": {
                        "param1": "$ctx.source.id",
                        "param2": "$ctx.args.id",
                        "email": "$ctx.identity.claims['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress']"
                      },
                    }
                    """;
        }



        protected void SetHttpRequestMapping(string verb, string resourcePath, string apiKey)
        {
            // Using FormatJsonString() to remove the extra padding
            RequestMapping = $$"""
                    {
                      "version": "2018-05-29",
                      "method": "{{verb}}",
                      "resourcePath": "{{resourcePath}}",
                      "params": {
                        "headers": {
                          "Content-Type": "application/json",
                          "X-Api-Key": "{{apiKey}}"
                        }
                      },
                      "query": {
                        "param1": "$ctx.source.id",
                        "param2": "$ctx.args.id",
                        "email": "$ctx.identity.claims['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress']"
                      },
                    }
                    """;
        }
    }
}