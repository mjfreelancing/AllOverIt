using AllOverIt.Helpers;
using System;
using System.Collections.Generic;

namespace AllOverIt.Aws.AppSync.Client.Utils
{
    internal sealed class ExceptionCollector : IDisposable
    {
        private readonly List<Exception> _exceptions = new();
        private IDisposable _subscription;

        public IReadOnlyList<Exception> Exceptions => _exceptions;

        public ExceptionCollector(IObservable<Exception> observable)
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