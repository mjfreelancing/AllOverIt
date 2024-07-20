using AllOverIt.Assertion;
using AllOverIt.Patterns.Pipeline;
using AllOverIt.ReactiveUI.CommandPipeline;
using ReactiveUI;

namespace AllOverIt.ReactiveUI.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="ReactiveCommand{TIn, TPrevOut}"/>.</summary>
    public static class ReactiveCommandExtensions
    {
        /// <summary>Creates a new asynchronous pipeline builder using two commands as the initial steps.</summary>
        /// <typeparam name="TIn">The input type (command argument) for the first step in the pipeline.</typeparam>
        /// <typeparam name="TPrevOut">The output type (command result) from the previous step in the pipeline.</typeparam>
        /// <typeparam name="TNextOut">The output type (command result) for the second step added to the pipeline.</typeparam>
        /// <param name="command">The first step to be added to the pipeline builder.</param>
        /// <param name="step">The second step to be added to the pipeline.</param>
        /// <returns>A new pipeline builder representing the steps just added to the pipeline.</returns>
        public static IPipelineBuilderAsync<TIn, TNextOut> Pipe<TIn, TPrevOut, TNextOut>(this ReactiveCommand<TIn, TPrevOut> command,
            ReactiveCommand<TPrevOut, TNextOut> step)
        {
            _ = command.WhenNotNull();
            _ = step.WhenNotNull();

            return ReactiveCommandPipelineBuilder.Pipe(command).Pipe(step);
        }
    }
}