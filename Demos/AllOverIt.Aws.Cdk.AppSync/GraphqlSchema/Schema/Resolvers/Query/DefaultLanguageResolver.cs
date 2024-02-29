namespace GraphqlSchema.Schema.Resolvers.Query
{
    internal sealed class DefaultLanguageResolver : NoneVtlResolver
    {
        public DefaultLanguageResolver()
        {
            ResponseMapping = @"$util.toJson({""code"": ""LNG"", ""name"": ""Language Name""})";
        }
    }
}