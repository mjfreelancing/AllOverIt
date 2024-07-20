using AllOverIt.Assertion;
using AllOverIt.Patterns.Pipeline;
using ReactiveUI;

namespace AllOverIt.ReactiveUI.CommandPipeline
{
    /// <summary>Provides command based <c>Pipe()</c> methods that can be chained to build asynchronous pipelines.</summary>
    public static class ReactiveCommandPipelineBuilder
    {
        /// <summary>Creates a new pipeline with an initial step implemented as a command.</summary>
        /// <typeparam name="TIn">The input type (command argument) for the pipeline step.</typeparam>
        /// <typeparam name="TOut">The output type (command result) for the pipeline step.</typeparam>
        /// <param name="command">The pipeline step to be appended.</param>
        /// <returns>A new pipeline builder instance that can have additional pipeline steps appended.</returns>
        public static IPipelineBuilderAsync<TIn, TOut> Pipe<TIn, TOut>(ReactiveCommand<TIn, TOut> command)
        {
            _ = command.WhenNotNull();

            var step = new ReactiveCommandPipelineStep<TIn, TOut>(command);

            return PipelineBuilder.PipeAsync(step);
        }
    }
}