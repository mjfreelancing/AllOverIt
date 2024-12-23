﻿using AllOverIt.Assertion;

namespace AllOverIt.Patterns.ResourceInitialization
{
    /// <summary>An async disposable object implementing the Resource Acquisition Is Initialization idiom.</summary>
    public class RaiiAsync : IAsyncDisposable
    {
        private bool _disposed;
        private readonly Func<Task> _cleanUp;

        /// <summary>Constructor used to provide the initialization and cleanup actions to be invoked.</summary>
        /// <param name="initialize">The initialization action to invoke at the time of initialization.</param>
        /// <param name="cleanUp">The cleanup action to perform when the object is disposed.</param>
        public RaiiAsync(Action initialize, Func<Task> cleanUp)
        {
            _ = initialize.WhenNotNull();
            _cleanUp = cleanUp.WhenNotNull();

            initialize.Invoke();
        }

        /// <summary>Called asynchronously when the instance is being disposed, resulting in the cleanup action
        /// provided at the time of construction being invoked.</summary>
        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore();

            GC.SuppressFinalize(this);
        }

        /// <summary>Performs the asynchronous disposal of resources.</summary>
        protected virtual async ValueTask DisposeAsyncCore()
        {
            if (!_disposed)
            {
                await _cleanUp.Invoke().ConfigureAwait(false);
                _disposed = true;
            }
        }
    }

    /// <summary>A strongly-type async disposable object implementing the Resource Acquisition Is Initialization idiom.</summary>
    /// <typeparam name="TType">The type being initialized.</typeparam>
    public class RaiiAsync<TType> : IAsyncDisposable
    {
        private bool _disposed;
        private readonly Func<TType, Task> _cleanUp;

        /// <summary>The context provided at the time of initialization.</summary>
        public TType Context { get; }

        /// <summary>Constructor used to provide the initialization and cleanup actions to be invoked.</summary>
        /// <param name="initialize">The initialization action to invoke at the time of initialization.</param>
        /// <param name="cleanUp">The cleanup action to perform when the object is disposed.</param>
        public RaiiAsync(Func<TType> initialize, Func<TType, Task> cleanUp)
        {
            _ = initialize.WhenNotNull();
            _cleanUp = cleanUp.WhenNotNull();

            Context = initialize.Invoke();
        }

        /// <summary>Called asynchronously when the instance is being disposed, resulting in the cleanup action
        /// provided at the time of construction being invoked.</summary>
        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore();

            GC.SuppressFinalize(this);
        }

        /// <summary>Performs the asynchronous disposal of resources.</summary>
        protected virtual async ValueTask DisposeAsyncCore()
        {
            if (!_disposed)
            {
                await _cleanUp.Invoke(Context).ConfigureAwait(false);
                _disposed = true;
            }
        }
    }
}
