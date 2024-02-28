namespace GraphqlSchema.Schema.Resolvers
{
    internal class FunctionResolver : RequestResponseResolverBase
    {
        public FunctionResolver()
        {
            RequestMapping = """
                {
                    "version": "2017-02-28",
                    "operation": "Invoke",
                    "payload": $util.toJson($ctx.args)
                }
                """;

            ResponseMapping = "$util.toJson($ctx.result.payload)";
        }
    }
}