using AllOverIt.Assertion;

namespace AllOverIt.Patterns.Pipeline.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="IPipelineStep{TIn, TOut}"/> and
    /// <see cref="IPipelineStepAsync{TIn, TOut}"/> types.</summary>
    public static class PipelineStepExtensions
    {
        /// <summary>Gets the underlying Func for a provided <see cref="IPipelineStep{TIn, TOut}"/>.</summary>
        /// <typeparam name="TIn">The input type for the provided pipeline step.</typeparam>
        /// <typeparam name="TOut">The output type for the provided pipeline step.</typeparam>
        /// <param name="step">The pipeline step to get the underlying Func for.</param>
        /// <returns>The underlying Func for a provided <see cref="IPipelineStep{TIn, TOut}"/>.</returns>
        public static Func<TIn, TOut> AsFunc<TIn, TOut>(this IPipelineStep<TIn, TOut> step)
        {
            _ = step.WhenNotNull();

            return step.Execute;
        }

        /// <summary>Gets the underlying asynchronous Func for a provided <see cref="IPipelineStepAsync{TIn, TOut}"/>.</summary>
        /// <typeparam name="TIn">The input type for the provided pipeline step.</typeparam>
        /// <typeparam name="TOut">The output type for the provided pipeline step.</typeparam>
        /// <param name="step">The pipeline step to get the underlying asynchronous Func for.</param>
        /// <returns>The underlying asynchronous Func for a provided <see cref="IPipelineStepAsync{TIn, TOut}"/>.</returns>
        public static Func<TIn, CancellationToken, Task<TOut>> AsFunc<TIn, TOut>(this IPipelineStepAsync<TIn, TOut> step)
        {
            _ = step.WhenNotNull();

            return step.ExecuteAsync;
        }
    }
}
