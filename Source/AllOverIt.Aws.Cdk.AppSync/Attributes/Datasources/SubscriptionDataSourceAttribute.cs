using AllOverIt.Aws.Cdk.AppSync.Mapping;
using AllOverIt.Helpers;
using AllOverIt.Utils;

namespace AllOverIt.Aws.Cdk.AppSync.Attributes.Datasources
{
    public class SubscriptionDataSourceAttribute : DataSourceAttribute
    {
        private class SubscriptionMapping : IRequestResponseMapping
        {
            public string RequestMapping { get; }
            public string ResponseMapping { get; }

            public SubscriptionMapping()
            {
                RequestMapping = GetRequestMapping();
                ResponseMapping = GetResponseMapping();
            }

            private static string GetRequestMapping()
            {
                // Using FormatJsonString() to remove the extra padding
                return Formatter.FormatJsonString(
                    @"
                    {
                      ""version"": ""2017-02-28"",
                      ""payload"": ""{}""
                    }"
                );
            }

            private static string GetResponseMapping()
            {
                return "#return()";
            }
        }

        private readonly string _identifier;

        public override string LookupKey => SanitiseLookupKey(_identifier);

        public SubscriptionDataSourceAttribute(string identifier, string description = default)
            : base(typeof(SubscriptionMapping), description)
        {
            _identifier = identifier.WhenNotNullOrEmpty(nameof(identifier));
        }
    }
}