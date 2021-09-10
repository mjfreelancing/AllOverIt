using AllOverIt.Helpers;
using System;
using System.Collections.Generic;

namespace AllOverIt.Aws.AppSync.Client.Utils
{
    // Used to capture exceptions. Cannot use the reactive ToList() because the sequence does not complete.
    internal sealed class ObservableExceptionCollector : IDisposable
    {
        private readonly List<Exception> _exceptions = new();
        private IDisposable _subscription;

        public IReadOnlyCollection<Exception> Exceptions => _exceptions;

        public ObservableExceptionCollector(IObservable<Exception> observable)
        {
            _subscription = observable
                .WhenNotNull(nameof(observable))
                .Subscribe(_exceptions.Add);
        }

        public void Dispose()
        {
            _subscription?.Dispose();
            _subscription = null;
        }
    }
}