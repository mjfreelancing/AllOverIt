using AllOverIt.Assertion;
using System;

namespace AllOverIt.Patterns.ResourceInitialization
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
            _ = initialize.WhenNotNull(nameof(initialize));
            _cleanUp = cleanUp.WhenNotNull(nameof(cleanUp));

            initialize.Invoke();
        }

        /// <summary>This is called when the instance is being disposed.</summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);      // just in case an inherited class has a finalizer
        }

        /// <summary>Disposes of the internal resources.</summary>
        /// <param name="disposing">Indicates if the internal resources are to be disposed.</param>
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

    /// <summary>A strongly-type disposable object implementing the Resource Acquisition Is Initialization idiom.</summary>
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
            _ = initialize.WhenNotNull(nameof(initialize));
            _cleanUp = cleanUp.WhenNotNull(nameof(cleanUp));

            Context = initialize.Invoke();
        }

        /// <summary>Called when the instance is being disposed, resulting in the cleanup action provided at the time of
        /// construction being invoked.</summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);      // just in case an inherited class has a finalizer
        }

        /// <summary>Disposes of the internal resources.</summary>
        /// <param name="disposing">Indicates if the internal resources are to be disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _cleanUp.Invoke(Context);
                }

                _disposed = true;
            }
        }
    }
}