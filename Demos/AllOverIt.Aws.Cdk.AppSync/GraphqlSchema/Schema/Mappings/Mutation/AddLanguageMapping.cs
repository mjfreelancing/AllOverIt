namespace GraphqlSchema.Schema.Mappings.Mutation
{
    internal sealed class AddLanguageMapping : NoneResponseMapping
    {
        public AddLanguageMapping()
        {
            ResponseMapping = @"$util.toJson({""code"": ""LNG"", ""name"": ""Language Name""})";
        }
    }
}