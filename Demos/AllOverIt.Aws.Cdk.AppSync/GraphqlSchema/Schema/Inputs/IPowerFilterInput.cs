using AllOverIt.Aws.Cdk.AppSync;
using AllOverIt.Aws.Cdk.AppSync.Attributes;
using GraphqlSchema.Schema.Types;
using System;

namespace GraphqlSchema.Schema.Inputs
{
    [SchemaType(GraphqlSchemaType.Input, "PowerFilterInput")]
    internal interface IPowerFilterInput
    {
        [SchemaTypeRequired]
        [SchemaDateType]
        public DateTime StartDate { get; }

        [SchemaTypeRequired]
        [SchemaDateType]
        public DateTime EndDate { get; }

        [SchemaTypeRequired]
        public MeterType MeterType { get; }

        [SchemaTypeRequired]
        public SummaryType SummaryType { get; }
    }
}