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
        private readonly List<IAsyncDisposable> _disposables = new();

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
            if (_disposables.Any())
            {
                DisposeResources();
            }
        }

        /// <summary>Disposes each of the registered disposables. This method does not return until they are all processed.</summary>
        /// <remarks>Dispose() will dispose of all registered IAsyncDisposable's in a background thread, whereas DisposeAsync() will
        /// perform the disposal on the calling thread.</remarks>
        public async ValueTask DisposeAsync()
        {
            if (_disposables.Any())
            {
                await DisposeResourcesAsync().ConfigureAwait(false);
            }
        }

        private void DisposeResources()
        {
            AggregateException aggregateException = null;

            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                // capture for the closure below (keep the analyzer happy)
                var cts = cancellationTokenSource;

                Task.Run(async () =>
                {
                    try
                    {
                        await DisposeResourcesAsync().ConfigureAwait(false);
                    }
                    catch (AggregateException aggregate)
                    {
                        aggregateException = aggregate;
                    }
                    catch (Exception exception)
                    {
                        aggregateException = new AggregateException(exception);
                    }
                    finally
                    {
                        cts.Cancel();
                    }
                }, CancellationToken.None);

                cancellationTokenSource.Token.WaitHandle.WaitOne();
            }

            if (aggregateException != null)
            {
                throw aggregateException;
            }
        }

        private async Task DisposeResourcesAsync()
        {
             IList<Exception> innerExceptions = null;

            foreach (var disposable in _disposables)
            {
                try
                {
                    await disposable.DisposeAsync().ConfigureAwait(false);
                }
                catch(Exception exception)
                {
                    innerExceptions ??= new List<Exception>();
                    innerExceptions.Add(exception);
                }
            }

            _disposables.Clear();

            if (innerExceptions != null)
            {
                throw new AggregateException(innerExceptions);
            }
        }
    }
}
#endif