using AllOverIt.Assertion;
using AllOverIt.Aws.Cdk.AppSync.Mapping;
using AllOverIt.Extensions;

namespace AllOverIt.Aws.Cdk.AppSync.Attributes.DataSources
{
    /// <summary>Provides information required to declare an AppSync subscription datasource that uses a request mapping with an
    /// empty payload and a response mapping that returns an empty result.</summary>
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

        /// <inheritdoc />
        public override string DataSourceName => _identifier;

        /// <summary>Constructor.</summary>
        /// <param name="identifier">A unique identifier for this datasource.</param>
        /// <param name="description">A description for the datasource.</param>
        public SubscriptionDataSourceAttribute(string identifier, string description = default)
            : base(typeof(SubscriptionMapping), description)
        {
            _identifier = identifier.WhenNotNullOrEmpty(nameof(identifier));
        }
    }
}