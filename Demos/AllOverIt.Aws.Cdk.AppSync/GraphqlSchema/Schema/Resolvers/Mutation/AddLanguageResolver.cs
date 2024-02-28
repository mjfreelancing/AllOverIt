namespace GraphqlSchema.Schema.Resolvers.Mutation
{
    internal sealed class AddLanguageResolver : NoneResolver
    {
        public AddLanguageResolver()
        {
            RequestMapping = GetResponseMapping();
        }

        private static string GetResponseMapping()
        {
            return """
                {
                  "version": "2017-02-28",
                  "payload": {
                    "code": "$ctx.args.language.code",
                    "name": "$ctx.args.language.name"
                  }
                }
                """;
        }
    }
}