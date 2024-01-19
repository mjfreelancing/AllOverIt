using AllOverIt.Assertion;
using AllOverIt.Patterns.Pipeline;
using AllOverIt.Patterns.Pipeline.Extensions;
using AllOverIt.ReactiveUI.CommandPipeline;
using ReactiveUI;

namespace AllOverIt.ReactiveUI.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="IPipelineBuilderAsync{TIn, TPrevOut}"/>.</summary>
    public static class PipelineBuilderAsyncExtensions
    {
        /// <summary>Adds a new pipeline step to an existing pipeline builder.</summary>
        /// <typeparam name="TIn">The input type for the first step in the pipeline.</typeparam>
        /// <typeparam name="TPrevOut">The output type from the previous step in the pipeline.</typeparam>
        /// <typeparam name="TNextOut">The output type for the step being added to the pipeline.</typeparam>
        /// <param name="prevStep">The builder instance representing the previous step in the pipeline.</param>
        /// <param name="step">The step, as a command, to be added to the pipeline builder.</param>
        /// <returns>A new pipeline builder representing all steps currently added to the pipeline.</returns>
        public static IPipelineBuilderAsync<TIn, TNextOut> Pipe<TIn, TPrevOut, TNextOut>(this IPipelineBuilderAsync<TIn, TPrevOut> prevStep,
            ReactiveCommand<TPrevOut, TNextOut> step)
        {
            _ = prevStep.WhenNotNull(nameof(prevStep));
            _ = step.WhenNotNull(nameof(step));

            var commandStep = new ReactiveCommandPipelineStep<TPrevOut, TNextOut>(step);

            return prevStep.PipeAsync(commandStep);
        }
    }
}