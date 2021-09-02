namespace GraphqlSchema.Schema.Mappings.Subscription
{
    internal sealed class AddedLanguageMapping : NoneResponseMapping
    {
        public AddedLanguageMapping()
        {
            ResponseMapping = @"$util.toJson({""code"": ""LNG"", ""name"": ""Language Name""})";
        }
    }
}