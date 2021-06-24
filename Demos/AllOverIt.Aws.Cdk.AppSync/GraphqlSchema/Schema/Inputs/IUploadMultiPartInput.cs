using AllOverIt.Aws.Cdk.AppSync;
using AllOverIt.Aws.Cdk.AppSync.Attributes;

namespace GraphqlSchema.Schema.Inputs
{
    [SchemaType(GraphqlSchemaType.Input, "UploadMultiPartInput")]
    internal interface IUploadMultiPartInput
    {
        [SchemaTypeRequired]
        public string Filename { get; set; }

        [SchemaTypeRequired]
        int PartCount { get; set; }
    }
}