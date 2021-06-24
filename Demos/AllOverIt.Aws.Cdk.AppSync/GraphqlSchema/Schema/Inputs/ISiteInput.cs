using AllOverIt.Aws.Cdk.AppSync;
using AllOverIt.Aws.Cdk.AppSync.Attributes;
using System;

namespace GraphqlSchema.Schema.Inputs
{
    [SchemaType(GraphqlSchemaType.Input, "SiteInput")]
    internal interface ISiteInput
    {
        [SchemaTypeRequired]
        [SchemaDateType]
        public DateTime StartDate { get; }

        [SchemaTypeRequired]
        public string ApiKey { get; }

        [SchemaTypeRequired]
        public string ContactName { get; }

        [SchemaTypeRequired]
        public string ContactEmail { get; }

        [SchemaTypeRequired]
        public string TimeZoneId { get; }
    }
}