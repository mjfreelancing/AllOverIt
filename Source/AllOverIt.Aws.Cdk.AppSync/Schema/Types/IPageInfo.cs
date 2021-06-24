using AllOverIt.Aws.Cdk.AppSync.Attributes;

namespace AllOverIt.Aws.Cdk.AppSync.Schema.Types
{
    [SchemaType(GraphqlSchemaType.Type, "PageInfo")]
    public interface IPageInfo
    {
        public string PreviousPageCursor { get; }
        public string NextPageCursor { get; }
    }
}