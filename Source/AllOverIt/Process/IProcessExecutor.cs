namespace AllOverIt.Process
{
    /// <summary>Defines a process executor.</summary>
    public interface IProcessExecutor : IDisposable
    {
        /// <summary>Asynchronously executes a configured process.</summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the </param>
        /// <returns>The result of executing the process.</returns>
        /// <remarks>The <paramref name="cancellationToken"/> is ignored if targeting NET STANDARD 2.1.</remarks>
        Task<ProcessExecutorResult> ExecuteAsync(CancellationToken cancellationToken = default);

        /// <summary>Asynchronously executes a configured process.</summary>
        /// <returns>The result of executing the process, including a buffered copy of the standard and error output.</returns>
        /// <remarks>The <paramref name="cancellationToken"/> is ignored if targeting NET STANDARD 2.1.</remarks>
        Task<ProcessExecutorBufferedResult> ExecuteBufferedAsync(CancellationToken cancellationToken = default);
    }
}
