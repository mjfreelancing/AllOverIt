using AllOverIt.Aws.Cdk.AppSync;
using AllOverIt.Aws.Cdk.AppSync.Attributes;

namespace GraphqlSchema.Schema.Inputs
{
    [SchemaType(GraphqlSchemaType.Input, "UploadMultiPartETagInput")]
    internal interface IUploadMultiPartETagInput
    {
        [SchemaTypeRequired]
        int PartNumber { get; set; }

        [SchemaTypeRequired]
        public string ETag { get; set; }
    }
}