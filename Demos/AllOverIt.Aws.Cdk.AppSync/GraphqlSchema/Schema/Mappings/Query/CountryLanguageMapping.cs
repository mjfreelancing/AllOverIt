namespace GraphqlSchema.Schema.Mappings.Query
{
    internal sealed class CountryLanguageMapping : NoneResponseMapping
    {
        public CountryLanguageMapping()
        {
            ResponseMapping =
                @"
                {
                  ""code"": ""code"",
                  ""name"": ""name""
                }";
        }
    }
}