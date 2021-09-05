using System;

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER || NET5_0_OR_GREATER
using System.Threading.Tasks;
#endif

namespace AllOverIt.Helpers
{
    /// <summary>A disposable object implementing the Resource Acquisition Is Initialization idiom.</summary>
    public class Raii : IDisposable
    {
        private bool _disposed;
        private readonly Action _cleanUp;

        /// <summary>Constructor used to provide the initialization and cleanup actions to be invoked.</summary>
        /// <param name="initialize">The initialization action to invoke at the time of initialization.</param>
        /// <param name="cleanUp">The cleanup action to perform when the object is disposed.</param>
        public Raii(Action initialize, Action cleanUp)
        {
            _ = initialize ?? throw new ArgumentNullException(nameof(initialize));

            _cleanUp = cleanUp ?? throw new ArgumentNullException(nameof(cleanUp));
            initialize.Invoke();
        }

        /// <summary>This is called when the instance is being disposed.</summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// A virtual method that is called at the time of disposal. For this class, the cleanup action provided at the time
        /// of construction is invoked.
        /// </summary>
        /// <param name="disposing">Is true when the object is being disposed, otherwise false.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _cleanUp.Invoke();
                }

                _disposed = true;
            }
        }
    }

    /// <summary>
    /// A strongly-type disposable object implementing the Resource Acquisition Is Initialization idiom.
    /// </summary>
    /// <typeparam name="TType">The type being initialized.</typeparam>
    public class Raii<TType> : IDisposable
    {
        private bool _disposed;
        private readonly Action<TType> _cleanUp;

        /// <summary>The context provided at the time of initialization.</summary>
        public TType Context { get; private set; }

        /// <summary>Constructor used to provide the initialization and cleanup actions to be invoked.</summary>
        /// <param name="initialize">The initialization action to invoke at the time of initialization.</param>
        /// <param name="cleanUp">The cleanup action to perform when the object is disposed.</param>
        public Raii(Func<TType> initialize, Action<TType> cleanUp)
        {
            _ = initialize ?? throw new ArgumentNullException(nameof(initialize));

            Context = initialize.Invoke();
            _cleanUp = cleanUp ?? throw new ArgumentNullException(nameof(cleanUp));
        }

        /// <summary>Called when the instance is being disposed, resulting in the cleanup action provided at the time of
        /// construction being invoked.</summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// A virtual method that is called at the time of disposal. For this class, the cleanup action provided at the time
        /// of construction is invoked.
        /// </summary>
        /// <param name="disposing">Is true when the object is being disposed, otherwise false.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _cleanUp.Invoke(Context);
                    Context = default;
                }

                _disposed = true;
            }
        }
    }

#if NETSTANDARD2_1_OR_GREATER
    /// <summary>
    /// A strongly-type async disposable object implementing the Resource Acquisition Is Initialization idiom.
    /// </summary>
    /// <typeparam name="TType">The type being initialized.</typeparam>
    public class RaiiAsync<TType> : IAsyncDisposable
    {
        private Func<TType, Task> _cleanUp;

        /// <summary>The context provided at the time of initialization.</summary>
        public TType Context { get; }

        /// <summary>Constructor used to provide the initialization and cleanup actions to be invoked.</summary>
        /// <param name="initialize">The initialization action to invoke at the time of initialization.</param>
        /// <param name="cleanUp">The cleanup action to perform when the object is disposed.</param>
        public RaiiAsync(Func<TType> initialize, Func<TType, Task> cleanUp)
        {
            _ = initialize ?? throw new ArgumentNullException(nameof(initialize));

            Context = initialize.Invoke();
            _cleanUp = cleanUp ?? throw new ArgumentNullException(nameof(cleanUp));
        }

        /// <summary>Called asynchronously when the instance is being disposed, resulting in the cleanup action
        /// provided at the time of construction being invoked.</summary>
        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore();

#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
            GC.SuppressFinalize(this);
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
        }

        protected virtual async ValueTask DisposeAsyncCore()
        {
            if (_cleanUp != null)
            {
                await _cleanUp.Invoke(Context).ConfigureAwait(false);
            }

            _cleanUp = null;
        }
    }
#endif
}