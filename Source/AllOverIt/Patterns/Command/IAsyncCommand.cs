namespace AllOverIt.Patterns.Command
{
    /// <summary>Represents an asynchronous command with no input or output.</summary>
    public interface IAsyncCommand
    {
        /// <summary>Asynchronously executes the command.</summary>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task that completes when the command has executed.</returns>
        Task ExecuteAsync(CancellationToken cancellationToken);
    }

    /// <summary>Represents an asynchronous command with a specified input type and no output type.</summary>
    /// <typeparam name="TInput">The input type to be provided to the command.</typeparam>
    public interface IAsyncCommand<in TInput>
    {
        /// <summary>Asynchronously executes the command with a specified input.</summary>
        /// <param name="input">The input instance to be provided to the command.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task that completes when the command has executed.</returns>
        Task ExecuteAsync(TInput input, CancellationToken cancellationToken);
    }

    /// <summary>Represents an asynchronous command with a specified input and output type.</summary>
    /// <typeparam name="TInput">The input type to be provided to the command.</typeparam>
    /// <typeparam name="TOutput">The output type to be returned from the command.</typeparam>
    public interface IAsyncCommand<in TInput, TOutput>
    {
        /// <summary>Asynchronously executes the command with a specified input and returns an output.</summary>
        /// <param name="input">The input instance to be provided to the command.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task, with the command output, that completes when the command has executed.</returns>
        Task<TOutput> ExecuteAsync(TInput input, CancellationToken cancellationToken);
    }
}
