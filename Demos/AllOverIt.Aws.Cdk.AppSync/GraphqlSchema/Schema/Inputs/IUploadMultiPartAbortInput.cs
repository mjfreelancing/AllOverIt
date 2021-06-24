using AllOverIt.Aws.Cdk.AppSync;
using AllOverIt.Aws.Cdk.AppSync.Attributes;

namespace GraphqlSchema.Schema.Inputs
{
    [SchemaType(GraphqlSchemaType.Input, "UploadMultiPartAbortInput")]
    internal interface IUploadMultiPartAbortInput
    {
        [SchemaTypeRequired]
        string Filename { get; set; }

        [SchemaTypeRequired]
        int UploadId { get; set; }
    }
}