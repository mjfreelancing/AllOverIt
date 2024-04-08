using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Async
{
    /// <summary>A composite that caters for asynchronous disposal of multiple IAsyncDisposable's using a synchronous Dispose().</summary>
    public sealed class CompositeAsyncDisposable : IDisposable, IAsyncDisposable
    {
        private List<IAsyncDisposable> _disposables = [];

        /// <summary>Returns the collection of disposables.</summary>
        public IEnumerable<IAsyncDisposable> Disposables => _disposables;

        /// <summary>Constructor.</summary>
        /// <param name="disposables">Async disposables to add to the composite disposable.</param>
        public CompositeAsyncDisposable(params IAsyncDisposable[] disposables)
        {
            Add(disposables);
        }

        /// <summary>Adds async disposables to the composite disposable.</summary>
        /// <param name="disposables">Async disposables to add to the composite disposable.</param>
        public void Add(params IAsyncDisposable[] disposables)
        {
            _disposables.AddRange(disposables);
        }

        /// <summary>Disposes each of the registered disposables. This method does not return until they are all processed.</summary>
        /// <remarks>Dispose() will dispose of all registered IAsyncDisposable's in a background thread, whereas DisposeAsync() will
        /// perform the disposal on the calling thread.</remarks>
        public void Dispose()
        {
            if (_disposables is not null && _disposables.Count != 0)
            {
                // Dispose should not throw, so it is assumed this will not throw
                DisposeResources();
            }
        }

        /// <summary>Disposes each of the registered disposables. This method does not return until they are all processed.</summary>
        /// <remarks>Dispose() will dispose of all registered IAsyncDisposable's in a background thread, whereas DisposeAsync() will
        /// perform the disposal on the calling thread.</remarks>
        public async ValueTask DisposeAsync()
        {
            if (_disposables is not null && _disposables.Count != 0)
            {
                // Dispose should not throw, so it is assumed this will not throw
                await DisposeResourcesAsync().ConfigureAwait(false);
            }
        }

        private void DisposeResources()
        {
            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                Task.Factory.StartNew(async state =>
                {
                    try
                    {
                        // Dispose should not throw, so it is assumed this will not throw
                        await DisposeResourcesAsync().ConfigureAwait(false);
                    }
                    finally
                    {
                        ((CancellationTokenSource) state).Cancel();
                    }
                }, cancellationTokenSource, CancellationToken.None);

                cancellationTokenSource.Token.WaitHandle.WaitOne();
            }
        }

        private async Task DisposeResourcesAsync()
        {
            try
            {
                foreach (var disposable in _disposables)
                {
                    // Dispose should not throw, so it is assumed this will not throw
                    await disposable.DisposeAsync().ConfigureAwait(false);
                }
            }
            finally
            {
                _disposables.Clear();
                _disposables = null;
            }
        }
    }
}
