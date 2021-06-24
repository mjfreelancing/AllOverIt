using AllOverIt.Aws.Cdk.AppSync;
using AllOverIt.Aws.Cdk.AppSync.Attributes;

namespace GraphqlSchema.Schema.Inputs
{
    [SchemaType(GraphqlSchemaType.Input, "UploadUrlInput")]
    internal interface IUploadUrlInput
    {
        [SchemaTypeRequired]
        public string Filename { get; set; }
    }
}