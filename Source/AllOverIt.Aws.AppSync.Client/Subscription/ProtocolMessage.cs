namespace AllOverIt.Aws.AppSync.Client.Subscription
{
    internal static class ProtocolMessage
    {
        public static class Request
        {
            public const string ConnectionInit = "connection_init";
            public const string Start = "start";
            public const string Stop = "stop";
        }

        public static class Response
        {
            public const string ConnectionAck = "connection_ack";
            public const string StartAck = "start_ack";
            public const string Data = "data";
            public const string KeepAlive = "ka";
            public const string Error = "error";
            public const string ConnectionError = "connection_error";
            public const string Close = "close";
            public const string Complete = "complete";
        }
    }
}