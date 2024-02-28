namespace GraphqlSchema.Schema.Resolvers
{
    internal class NoneResolver : RequestResponseResolverBase
    {
        public NoneResolver()
        {
            RequestMapping = """
                {
                  "version": "2017-02-28",
                  "payload": "{}"
                }
                """;

            ResponseMapping = "$util.toJson($ctx.result)";
        }
    }
}