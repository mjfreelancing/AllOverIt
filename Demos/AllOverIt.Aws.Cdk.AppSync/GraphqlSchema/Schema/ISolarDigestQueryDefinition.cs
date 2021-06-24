using AllOverIt.Aws.Cdk.AppSync.Attributes;
using AllOverIt.Aws.Cdk.AppSync.Schema;
using GraphqlSchema.Schema.Inputs;
using GraphqlSchema.Schema.Types;

namespace GraphqlSchema.Schema
{
    internal interface ISolarDigestQueryDefinition : IQueryDefinition
    {
        [SchemaTypeRequired]
        [LambdaDataSource(Constants.AppName, Constants.Function.GetSite)]
        ISite Site([SchemaTypeRequired] string id);

        [SchemaTypeRequired]
        [LambdaDataSource(Constants.AppName, Constants.Function.GetUploadUrl)]
        string UploadUrl([SchemaTypeRequired] IUploadUrlInput input);

        [SchemaTypeRequired]
        [LambdaDataSource(Constants.AppName, Constants.Function.GetUploadMultiPart)]
        IUploadMultiParts UploadMultiPartUrls([SchemaTypeRequired] IUploadMultiPartInput input);

        [SchemaTypeRequired]
        [LambdaDataSource(Constants.AppName, Constants.Function.GetUploadMultiPartAbort)]
        string UploadMultiPartAbort([SchemaTypeRequired] IUploadMultiPartAbortInput input);

        [SchemaTypeRequired]
        [LambdaDataSource(Constants.AppName, Constants.Function.GetUploadMultiPartComplete)]
        string UploadMultiPartComplete([SchemaTypeRequired] IUploadMultiPartCompleteInput input);

        [SchemaTypeRequired]
        [LambdaDataSource(Constants.AppName, Constants.Function.GetDownloadUrl)]
        string DownloadUrl([SchemaTypeRequired] string filename);
    }
}