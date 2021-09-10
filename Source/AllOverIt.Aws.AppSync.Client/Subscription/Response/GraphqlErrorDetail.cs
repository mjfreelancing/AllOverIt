using System.Collections.Generic;

namespace AllOverIt.Aws.AppSync.Client.Subscription
{
    public class GraphqlErrorDetail
    {
        public int? ErrorCode { get; set; }
        public string ErrorType { get; set; }
        public string Message { get; set; }
        public IEnumerable<GraphqlLocation> Locations { get; set; }
        public IEnumerable<object> Path { get; set; }
    }
}