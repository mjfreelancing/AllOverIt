namespace GraphqlSchema.Schema.Mappings.Query
{
    internal sealed class DefaultLanguageMapping : NoneResponseMapping
    {
        public DefaultLanguageMapping()
        {
            ResponseMapping = @"$util.toJson({""code"": ""LNG"", ""name"": ""Language Name""})";
        }
    }
}