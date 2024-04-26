namespace AllOverIt.GenericHost
{
    /// <summary>Abstract base class for <see cref="IGenericApp"/> concrete types.</summary>
    public abstract class GenericAppBase : IGenericApp
    {
        /// <inheritdoc />
        public abstract Task StartAsync(CancellationToken cancellationToken);

        /// <inheritdoc />
        /// <remarks>The base class provides a default implementation for this method.</remarks>
        public virtual void OnStopping()
        {
        }

        /// <inheritdoc />
        /// <remarks>The base class provides a default implementation for this method.</remarks>
        public virtual void OnStopped()
        {
        }
    }
}