namespace GraphqlSchema.Schema.Resolvers
{
    internal class EventBridgeVtlResolver : VtlResolverBase
    {
        public EventBridgeVtlResolver(string source, string detailType)
        {
            RequestMapping = $$"""
                {
                    "version": "2018-05-29",
                    "operation": "PutEvents",
                    "events": [
                        {
                            "source": "{{source}}",
                            "detailType": "{{detailType}}",
                            "detail": $util.toJson($ctx.args)
                        }
                    ]
                }
                """;

            ResponseMapping = "$util.toJson($ctx.result.payload)";
        }
    }
}