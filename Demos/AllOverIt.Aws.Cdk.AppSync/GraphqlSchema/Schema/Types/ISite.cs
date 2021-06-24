using AllOverIt.Aws.Cdk.AppSync;
using AllOverIt.Aws.Cdk.AppSync.Attributes;
using GraphqlSchema.Schema.Inputs;
using System;

namespace GraphqlSchema.Schema.Types
{
    [SchemaType(GraphqlSchemaType.Type, "Site")]
    internal interface ISite
    {
        [SchemaTypeRequired]
        public string Id { get; }

        [SchemaTypeRequired]
        [SchemaDateType]
        public DateTime StartDate { get; }

        [SchemaTypeRequired]
        public string ContactName { get; }

        [SchemaTypeRequired]
        public string ContactEmail { get; }

        [SchemaTypeRequired]
        public string TimeZoneId { get; }

        public string LastAggregationDate { get; }

        public string LastSummaryDate { get; }

        public string LastRefreshDateTime { get; }

        [SchemaTypeRequired]
        [LambdaDataSource(Constants.AppName, Constants.Function.GetSitePowerSummary)]
        IPowerConnection Power(int limit, string startCursor, [SchemaTypeRequired] IPowerFilterInput filter);
    }
}