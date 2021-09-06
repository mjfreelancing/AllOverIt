#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER || NET5_0_OR_GREATER

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Helpers
{
    /// <summary>A composite that caters for asynchronous disposal of multiple IAsyncDisposable's using a synchronous Dispose().</summary>
    public sealed class CompositeAsyncDisposable : IDisposable, IAsyncDisposable
    {
        private List<IAsyncDisposable> _disposables;
        private CancellationTokenSource _beginDisposalCancellationTokenSource;
        private CancellationTokenSource _doneDisposalCancellation;
        private Task _completionTask;

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
            _disposables ??= new List<IAsyncDisposable>();
            _disposables.AddRange(disposables);
        }

        /// <summary>Gets a task that completes when all disposables have been asynchronously disposed of via the Dispose() method.</summary>
        /// <returns>A task that completes when all disposables have been asynchronously disposed of.</returns>
        /// <remarks>This task is only required when disposing via Dispose().</remarks>
        public Task GetDisposalCompletion()
        {
            _beginDisposalCancellationTokenSource ??= new CancellationTokenSource();
            _doneDisposalCancellation ??= new CancellationTokenSource();

            _completionTask ??= Task.Run(async () =>
            {
                _beginDisposalCancellationTokenSource.Token.WaitHandle.WaitOne();

                await DisposeResources().ConfigureAwait(false);

                _doneDisposalCancellation.Cancel();
            });

            return _completionTask;
        }

        /// <summary>Disposes each of the registered disposables. This method does not return until they are all processed.</summary>
        /// <remarks>Dispose() will dispose of all registered IAsyncDisposable's in a background thread, whereas DisposeAsync() will
        /// perform the disposal on the calling thread.</remarks>
        public void Dispose()
        {
            if (_disposables.Any())
            {
                // make sure the background thread has been created, otherwise nothing will be disposed of.
                _ = GetDisposalCompletion();

                // Trigger the start of all async disposals
                _beginDisposalCancellationTokenSource.Cancel();

                // Wait for them to all complete
                _doneDisposalCancellation.Token.WaitHandle.WaitOne();
            }

            _beginDisposalCancellationTokenSource?.Dispose();
            _beginDisposalCancellationTokenSource = null;

            _doneDisposalCancellation?.Dispose();
            _doneDisposalCancellation = null;

            _completionTask = null;
        }

        /// <summary>Disposes each of the registered disposables. This method does not return until they are all processed.</summary>
        /// <remarks>Dispose() will dispose of all registered IAsyncDisposable's in a background thread, whereas DisposeAsync() will
        /// perform the disposal on the calling thread.</remarks>
        public async ValueTask DisposeAsync()
        {
            await DisposeResources().ConfigureAwait(false);
        }

        private async Task DisposeResources()
        {
            foreach (var disposable in _disposables)
            {
                await disposable.DisposeAsync().ConfigureAwait(false);
            }

            _disposables.Clear();
        }
    }
}
#endif