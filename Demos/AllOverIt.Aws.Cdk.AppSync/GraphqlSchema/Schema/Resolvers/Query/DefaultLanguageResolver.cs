namespace GraphqlSchema.Schema.Resolvers.Query
{
    internal sealed class DefaultLanguageResolver : NoneResolver
    {
        public DefaultLanguageResolver()
        {
            ResponseMapping = @"$util.toJson({""code"": ""LNG"", ""name"": ""Language Name""})";
        }
    }
}