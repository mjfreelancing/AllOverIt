namespace GraphqlSchema.Schema.Resolvers.Query
{
    internal sealed class CountryLanguageResolver : NoneResolver
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