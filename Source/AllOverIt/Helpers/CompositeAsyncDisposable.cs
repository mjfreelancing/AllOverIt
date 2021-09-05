#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER || NET5_0_OR_GREATER

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Helpers
{
    /// <summary>A composite that caters for asynchronous disposal of multiple IAsyncDisposable's.</summary>
    public sealed class CompositeAsyncDisposable : IDisposable
    {
        private readonly CancellationTokenSource _beginDisposalCancellationTokenSource = new();
        private readonly CancellationTokenSource _doneDisposalCancellation = new();
        private readonly List<IAsyncDisposable> _disposables = new();
        private Task _completionTask;

        /// <summary>Constructor.</summary>
        public CompositeAsyncDisposable()
        {
        }

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

        /// <summary>Gets a task that completes when all disposables have been asynchronously disposed of.</summary>
        /// <returns>A task that completes when all disposables have been asynchronously disposed of.</returns>
        /// <remarks>The disposables are processed when this CompositeAsyncDisposable is disposed.</remarks>
        public Task GetDisposalCompletion()
        {
            _completionTask ??= Task.Run(async () =>
            {
                _beginDisposalCancellationTokenSource.Token.WaitHandle.WaitOne();

                foreach (var disposable in _disposables)
                {
                    await disposable.DisposeAsync().ConfigureAwait(false);
                }

                _doneDisposalCancellation.Cancel();
            });

            return _completionTask;
        }

        /// <summary>Disposes each of the registered disposables. This method does not return until they are all processed.</summary>
        public void Dispose()
        {
            if (_completionTask != null)
            {
                // Trigger the start of all async disposals
                _beginDisposalCancellationTokenSource.Cancel();

                // Wait for them to all complete
                _doneDisposalCancellation.Token.WaitHandle.WaitOne();
            }

            _beginDisposalCancellationTokenSource?.Dispose();
            _doneDisposalCancellation?.Dispose();
        }
    }
}
#endif