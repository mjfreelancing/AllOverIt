using AllOverIt.Assertion;

namespace AllOverIt.Helpers
{
    /// <summary>Collects exceptions that can be thrown as an AggregateException.</summary>
    public sealed class ExceptionAggregator
    {
        private readonly List<Exception> _exceptions = [];

        /// <summary>Provides all exceptions currently added to the aggregator.</summary>
        public Exception[] Exceptions => [.. _exceptions];

        /// <summary>Adds a new exception to the aggregator. If it is an AggregateException it will be flattened into
        /// the resulting AggregateException thrown at the time of calling <see cref="ThrowIfAnyExceptions"/>.</summary>
        /// <param name="exception">The exception to add to the aggregator.</param>
        public void AddException(Exception exception)
        {
            _ = exception.WhenNotNull();

            _exceptions.Add(exception);
        }

        /// <summary>Throws an AggregateException if any exceptions were added to the aggregator.</summary>
        /// <param name="flatten">When <see langword="True" />, if any of the exceptions are an AggregateException they will be flattened
        /// into the resulting AggregateException thrown. Default is <see langword="True" />.</param>
        public void ThrowIfAnyExceptions(bool flatten = true)
        {
            if (_exceptions.Count != 0)
            {
                var aggregate = new AggregateException(_exceptions);

                if (flatten)
                {
                    throw aggregate.Flatten();
                }

                throw aggregate;
            }
        }
    }
}