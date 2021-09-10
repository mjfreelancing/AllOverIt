using System;

namespace AllOverIt.Aws.AppSync.Client.Exceptions
{
    [Serializable]
    public sealed class ConnectionTimeoutException : TimeoutExceptionBase
    {
        public ConnectionTimeoutException(TimeSpan timeout)
            : base(timeout)
        {
        }
    }
}