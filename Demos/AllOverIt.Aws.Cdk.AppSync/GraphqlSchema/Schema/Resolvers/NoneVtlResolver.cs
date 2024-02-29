namespace GraphqlSchema.Schema.Resolvers
{
    internal class NoneVtlResolver : VtlResolverBase
    {
        public NoneVtlResolver()
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