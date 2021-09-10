using System;
using System.Threading;

namespace AllOverIt.Aws.AppSync.Client.Utils
{
    internal sealed class TimeoutCancellationSource : IDisposable
    {
        private CancellationTokenSource _cts;

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
            _cts?.Dispose();
            _cts = null;
        }
    }
}