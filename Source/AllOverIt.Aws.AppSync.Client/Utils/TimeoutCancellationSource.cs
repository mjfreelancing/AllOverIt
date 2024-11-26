namespace AllOverIt.Aws.AppSync.Client.Utils
{
    // A CancellationTokenSource that can timeout and be linked with another CancellationTokenSource.
    internal sealed class TimeoutCancellationSource : IDisposable
    {
        private bool _disposed;

        private readonly CancellationTokenSource _cts;

        public CancellationToken Token => _cts.Token;
        public TimeSpan Timeout { get; }

        public TimeoutCancellationSource(TimeSpan timeout)
        {
            Timeout = timeout;
            _cts = new CancellationTokenSource(timeout);
        }

        public CancellationTokenSource GetLinkedTokenSource(CancellationToken token)
        {
            return CancellationTokenSource.CreateLinkedTokenSource(Token, token);
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;

            }

            _cts.Dispose();
            _disposed = true;
        }
    }
}