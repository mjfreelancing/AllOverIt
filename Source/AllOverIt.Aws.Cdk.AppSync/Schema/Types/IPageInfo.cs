using AllOverIt.Aws.Cdk.AppSync.Attributes.Types;

namespace AllOverIt.Aws.Cdk.AppSync.Schema.Types
{
    /// <summary>Represents pagination details on a Connection.</summary>
    [SchemaType("PageInfo")]
    public interface IPageInfo
    {
        /// <summary>The cursor for the start of the current page.</summary>
        public string CurrentPageCursor();

        /// <summary>The cursor for the start of the previous page.</summary>
        public string PreviousPageCursor();

        /// <summary>The cursor for the start of the next page.</summary>
        public string NextPageCursor();
    }
}