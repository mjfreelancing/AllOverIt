using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace AllOverIt.Async
{
    // Original source: https://devblogs.microsoft.com/pfxteam/asynclazyt/

    /// <summary>
    /// Provides support for lazy initialization using a factory that returns a Task{TType}.
    /// </summary>
    /// <typeparam name="TType">The type that is lazily initialized.</typeparam>
    public class AsyncLazy<TType> : Lazy<Task<TType>>
    {
        /// <summary>
        /// Initializes a new instance of the AsyncLazy{TType} class. When lazy initialization occurs, the specified initialization
        /// factory is executed asynchronously.
        /// </summary>
        /// <param name="factory">The factory used for lazy initialization of the stored value.</param>
        public AsyncLazy(Func<TType> factory)
            : base(() => Task.Factory.StartNew(factory))
        {
        }

        /// <summary>
        /// Initializes a new instance of the AsyncLazy{TType} class. When lazy initialization occurs, the specified initialization
        /// factory is executed asynchronously.
        /// </summary>
        /// <param name="factory">The factory used for lazy initialization of the stored value.</param>
        public AsyncLazy(Func<Task<TType>> factory)
            : base(() => Task.Factory.StartNew(factory).Unwrap())
        {
        }

        /// <summary>
        /// Gets an awaiter that allows for 'await lazyProp' instead of 'await lazyProp.Value'
        /// </summary>
        /// <returns>An awaiter for the value being initialized.</returns>
        public TaskAwaiter<TType> GetAwaiter()
        {
            return Value.GetAwaiter();
        }
    }
}