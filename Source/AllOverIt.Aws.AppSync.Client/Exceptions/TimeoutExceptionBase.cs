using System;

namespace AllOverIt.Aws.AppSync.Client.Exceptions
{
    /// <summary>A base class for all graphql related timeout based exceptions.</summary>
    public abstract class TimeoutExceptionBase : Exception
    {
        /// <summary>The timeout period that expired.</summary>
        public TimeSpan Timeout { get; }

        /// <summary>Constructor.</summary>
        /// <param name="timeout">The timeout period that expired.</param>
        protected TimeoutExceptionBase(TimeSpan timeout)
        {
            Timeout = timeout;
        }
    }
}