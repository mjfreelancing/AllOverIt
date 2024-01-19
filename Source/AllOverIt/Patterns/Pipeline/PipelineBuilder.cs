using AllOverIt.Assertion;
using AllOverIt.Patterns.Pipeline.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Patterns.Pipeline
{
    /// <summary>Provides a number of <c>Pipe()</c> methods that can be chained to build synchronous or asynchronous pipelines.</summary>
    public static class PipelineBuilder
    {
        /// <summary>Creates a new pipeline with an initial step that requires no input.</summary>
        /// <typeparam name="TOut">The output type for the pipeline step.</typeparam>
        /// <param name="step">The pipeline step to be appended.</param>
        /// <returns>A new pipeline builder instance that can have additional pipeline steps appended.</returns>
        public static IPipelineBuilder<TOut> Pipe<TOut>(Func<TOut> step)
        {
            return new PipelineNoInputBuilder<TOut>(step);
        }

        /// <summary>Creates a new pipeline with an initial step.</summary>
        /// <typeparam name="TIn">The input type for the pipeline step.</typeparam>
        /// <typeparam name="TOut">The output type for the pipeline step.</typeparam>
        /// <param name="step">The pipeline step to be appended.</param>
        /// <returns>A new pipeline builder instance that can have additional pipeline steps appended.</returns>
        public static IPipelineBuilder<TIn, TOut> Pipe<TIn, TOut>(Func<TIn, TOut> step)
        {
            return new PipelineBuilder<TIn, TOut>(step);
        }

        /// <summary>Creates a new pipeline with an initial step.</summary>
        /// <typeparam name="TIn">The input type for the pipeline step.</typeparam>
        /// <typeparam name="TOut">The output type for the pipeline step.</typeparam>
        /// <param name="step">The pipeline step to be appended.</param>
        /// <returns>A new pipeline builder instance that can have additional pipeline steps appended.</returns>
        public static IPipelineBuilder<TIn, TOut> Pipe<TIn, TOut>(IPipelineStep<TIn, TOut> step)
        {
            // AsFunc() performs a null check
            var stepFunc = step.AsFunc();

            return new PipelineBuilder<TIn, TOut>(stepFunc);
        }

        /// <summary>Creates a new pipeline with an initial step, of type <typeparamref name="TPipelineStep"/>.</summary>
        /// <typeparam name="TPipelineStep">The type of the initial pipeline step.</typeparam>
        /// <typeparam name="TIn">The input type for the pipeline step.</typeparam>
        /// <typeparam name="TOut">The output type for the pipeline step.</typeparam>
        /// <returns>A new pipeline builder instance that can have additional pipeline steps appended.</returns>
        public static IPipelineBuilder<TIn, TOut> Pipe<TPipelineStep, TIn, TOut>() where TPipelineStep : IPipelineStep<TIn, TOut>, new()
        {
            var step = new TPipelineStep();
            return Pipe(step);
        }

        /// <summary>Creates a new asynchronous pipeline with an initial step.</summary>
        /// <typeparam name="TOut">The output type for the pipeline step.</typeparam>
        /// <param name="step">The pipeline step to be appended.</param>
        /// <returns>A new pipeline builder instance that can have additional pipeline steps appended.</returns>
        public static IPipelineBuilderAsync<TOut> PipeAsync<TOut>(Func<CancellationToken, Task<TOut>> step)
        {
            return new PipelineNoInputBuilderAsync<TOut>(step);
        }

        /// <summary>Creates a new asynchronous pipeline with an initial step.</summary>
        /// <typeparam name="TIn">The input type for the pipeline step.</typeparam>
        /// <typeparam name="TOut">The output type for the pipeline step.</typeparam>
        /// <param name="step">The pipeline step to be appended.</param>
        /// <returns>A new pipeline builder instance that can have additional pipeline steps appended.</returns>
        public static IPipelineBuilderAsync<TIn, TOut> PipeAsync<TIn, TOut>(Func<TIn, CancellationToken, Task<TOut>> step)
        {
            return new PipelineBuilderAsync<TIn, TOut>(step);
        }

        /// <summary>Creates a new asynchronous pipeline with an initial step.</summary>
        /// <typeparam name="TIn">The input type for the pipeline step.</typeparam>
        /// <typeparam name="TOut">The output type for the pipeline step.</typeparam>
        /// <param name="step">The pipeline step to be appended.</param>
        /// <returns>A new pipeline builder instance that can have additional pipeline steps appended.</returns>
        public static IPipelineBuilderAsync<TIn, TOut> PipeAsync<TIn, TOut>(IPipelineStepAsync<TIn, TOut> step)
        {
            // AsFunc() performs a null check
            var stepFunc = step.AsFunc();

            return new PipelineBuilderAsync<TIn, TOut>(stepFunc);
        }

        /// <summary>Creates a new asynchronous pipeline with an initial step, of type <typeparamref name="TPipelineStep"/>.</summary>
        /// <typeparam name="TPipelineStep">The type of the initial pipeline step.</typeparam>
        /// <typeparam name="TIn">The input type for the pipeline step.</typeparam>
        /// <typeparam name="TOut">The output type for the pipeline step.</typeparam>
        /// <returns>A new pipeline builder instance that can have additional pipeline steps appended.</returns>
        public static IPipelineBuilderAsync<TIn, TOut> PipeAsync<TPipelineStep, TIn, TOut>() where TPipelineStep : IPipelineStepAsync<TIn, TOut>, new()
        {
            var step = new TPipelineStep();

            return PipeAsync(step);
        }
    }

    // Begins a new pipline that takes an initial input
    internal sealed class PipelineBuilder<TIn, TOut> : IPipelineBuilder<TIn, TOut>
    {
        private readonly Func<TIn, TOut> _step;

        public PipelineBuilder(Func<TIn, TOut> step)
        {
            _step = step.WhenNotNull(nameof(step));
        }

        public Func<TIn, TOut> Build()
        {
            return _step;
        }
    }

    // Appends to a pipeline that takes an initial input
    internal sealed class PipelineBuilder<TIn, TPrevOut, TNextOut> : IPipelineBuilder<TIn, TNextOut>
    {
        private readonly IPipelineBuilder<TIn, TPrevOut> _prevStep;
        private readonly Func<TPrevOut, TNextOut> _step;

        public PipelineBuilder(IPipelineBuilder<TIn, TPrevOut> prevStep, Func<TPrevOut, TNextOut> step)
        {
            _prevStep = prevStep.WhenNotNull(nameof(prevStep));
            _step = step.WhenNotNull(nameof(step));
        }

        // Create a func that invokes the previous func and uses its result as the input to the next func (step)
        public Func<TIn, TNextOut> Build()
        {
            TNextOut func(TIn input)
            {
                var prevOutput = _prevStep
                    .Build()
                    .Invoke(input);

                return _step.Invoke(prevOutput);
            }

            return func;
        }
    }
}
