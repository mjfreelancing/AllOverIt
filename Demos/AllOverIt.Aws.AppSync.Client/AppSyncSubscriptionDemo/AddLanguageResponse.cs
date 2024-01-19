namespace AppSyncSubscriptionDemo
{
    public sealed class AddLanguageResponse
    {
        public sealed class AddLanguageContent
        {
            public string Code { get; set; }
            public string Name { get; set; }
        }

        public AddLanguageContent AddLanguage { get; set; }
    }
}