using AllOverIt.Assertion;

namespace AllOverIt.Aws.AppSync.Client.Utils
{
    // Used to capture exceptions. Cannot use the reactive ToList() because the sequence does not complete.
    internal sealed class ObservableExceptionCollector : IDisposable
    {
        private bool _disposed;

        private readonly List<Exception> _exceptions = [];
        private readonly IDisposable _subscription;

        public IEnumerable<Exception> Exceptions => _exceptions;

        public ObservableExceptionCollector(IObservable<Exception> observable)
        {
            _subscription = observable
                .WhenNotNull()
                .Subscribe(_exceptions.Add);
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _subscription.Dispose();
            _disposed = true;
        }
    }
}