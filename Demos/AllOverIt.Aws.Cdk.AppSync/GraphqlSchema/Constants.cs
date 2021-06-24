namespace GraphqlSchema
{
    internal static class Constants
    {
        internal const string AppName = "SolarDigest";
        internal const int ServiceVersion = 1;

        internal static class Function
        {
            internal const string GetSite = "GetSite";
            internal const string AddSite = "AddSite";
            internal const string UpdateSite = "UpdateSite";
            internal const string GetSitePowerSummary = "GetSitePowerSummary";
            internal const string GetUploadUrl = "GetUploadUrl";
            internal const string GetUploadMultiPart = "GetUploadMultiPart";
            internal const string GetUploadMultiPartAbort = "GetUploadMultiPartAbort";
            internal const string GetUploadMultiPartComplete = "GetUploadMultiPartComplete";
            internal const string GetDownloadUrl = "GetDownloadUrl";
        }
    }
}