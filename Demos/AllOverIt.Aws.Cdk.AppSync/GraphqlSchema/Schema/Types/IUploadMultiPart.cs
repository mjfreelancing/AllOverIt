using AllOverIt.Aws.Cdk.AppSync;
using AllOverIt.Aws.Cdk.AppSync.Attributes;

namespace GraphqlSchema.Schema.Types
{
    [SchemaType(GraphqlSchemaType.Type, "UploadMultiPart")]
    internal interface IUploadMultiPart
    {
        [SchemaTypeRequired]
        public int PartNumber { get; }

        [SchemaTypeRequired]
        public string Url { get; }
    }
}