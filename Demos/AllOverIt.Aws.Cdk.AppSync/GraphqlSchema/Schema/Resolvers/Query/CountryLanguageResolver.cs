namespace GraphqlSchema.Schema.Resolvers.Query
{
    internal sealed class CountryLanguageResolver : NoneVtlResolver
    {
        public CountryLanguageResolver(string code, string name)
        {
            ResponseMapping = $$"""
                {
                  "code": "{{code}}",
                  "name": "{{name}}"
                }
                """;
        }
    }
}