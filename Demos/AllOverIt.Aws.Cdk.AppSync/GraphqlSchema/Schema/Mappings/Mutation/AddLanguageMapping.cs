namespace GraphqlSchema.Schema.Mappings.Mutation
{
    internal sealed class AddLanguageMapping : NoneResponseMapping
    {
        public AddLanguageMapping()
        {
            RequestMapping = GetResponseMapping();
        }

        private static string GetResponseMapping()
        {
            return
                @"
                {
                  ""version"": ""2017-02-28"",
                  ""payload"": {
                    ""code"": ""$ctx.args.language.code"",
                    ""name"": ""$ctx.args.language.name""
                  }
                }";
        }
    }
}