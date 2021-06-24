using AllOverIt.Aws.Cdk.AppSync;
using AllOverIt.Aws.Cdk.AppSync.Attributes;

namespace GraphqlSchema.Schema.Types
{
    [SchemaType(GraphqlSchemaType.Type, "TimeWatts")]
    internal interface ITimeWatts
    {
        [SchemaTypeRequired]
        public string Time { get; }

        [SchemaTypeRequired]
        public double Watts { get; }

        [SchemaTypeRequired]
        public float WattHour { get; }
    }
}