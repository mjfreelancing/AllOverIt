using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Patterns.Pipeline
{
    /// <summary>Represents a synchronous step within a pipeline sequence.</summary>
    /// <typeparam name="TIn">The input type for the provided pipeline step.</typeparam>
    /// <typeparam name="TOut">The output type for the provided pipeline step.</typeparam>
    public interface IPipelineStep<TIn, TOut>
    {
        /// <summary>Executes this step within a pipeline sequence.</summary>
        /// <param name="input">The input provided to the pipeline step.</param>
        /// <returns>The output of the pipeline step.</returns>
        TOut Execute(TIn input);
    }

    /// <summary>Represents an asynchronous step within a pipeline sequence.</summary>
    /// <typeparam name="TIn">The input type for the provided pipeline step.</typeparam>
    /// <typeparam name="TOut">The output type for the provided pipeline step.</typeparam>
    public interface IPipelineStepAsync<TIn, TOut>
    {
        /// <summary>Asynchronously executes this step within a pipeline sequence.</summary>
        /// <param name="input">The input provided to the pipeline step.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The output of the pipeline step.</returns>
        Task<TOut> ExecuteAsync(TIn input, CancellationToken cancellationToken);
    }
}
