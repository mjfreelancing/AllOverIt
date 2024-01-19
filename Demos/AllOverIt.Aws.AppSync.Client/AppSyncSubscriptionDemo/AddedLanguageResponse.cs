namespace AppSyncSubscriptionDemo
{
    public sealed class AddedLanguageResponse
    {
        public sealed class AddedLanguageContent
        {
            public string Code { get; set; }
            public string Name { get; set; }
        }

        public AddedLanguageContent AddedLanguage { get; set; }
    }
}