using AllOverIt.Aws.Cdk.AppSync.Attributes.Types;

namespace AllOverIt.Aws.Cdk.AppSync.Schema.Types
{
    /// <summary>Represents pagination details on a Connection.</summary>
    [SchemaType("PageInfo")]
    public interface IPageInfo
    {
        /// <summary>The token for the start of the current page.</summary>
        public string CurrentToken();

        /// <summary>The token for the start of the previous page.</summary>
        public string PreviousToken();

        /// <summary>The token for the start of the next page.</summary>
        public string NextToken();
    }
}