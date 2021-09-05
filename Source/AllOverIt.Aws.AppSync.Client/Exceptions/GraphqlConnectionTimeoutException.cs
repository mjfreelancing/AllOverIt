using System;

namespace AllOverIt.Aws.AppSync.Client.Exceptions
{
    public sealed class GraphqlConnectionTimeoutException : Exception
    {
        public GraphqlConnectionTimeoutException()
        {
        }

        public GraphqlConnectionTimeoutException(string message)
            : base(message)
        {
        }
    }
}