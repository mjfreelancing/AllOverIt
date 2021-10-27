using AllOverIt.Aws.Cdk.AppSync.Mapping;
using AllOverIt.Extensions;
using AllOverIt.Helpers;

namespace AllOverIt.Aws.Cdk.AppSync.Attributes.DataSources
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
                return StringFormatExtensions.FormatJsonString(
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

        public override string DataSourceName => _identifier;

        public SubscriptionDataSourceAttribute(string identifier, string description = default)
            : base(typeof(SubscriptionMapping), description)
        {
            _identifier = identifier.WhenNotNullOrEmpty(nameof(identifier));
        }
    }
}