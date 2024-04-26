using AllOverIt.Assertion;

namespace AllOverIt.Patterns.Pipeline.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="IPipelineBuilder{TIn, TPrevOut}"/> and
    /// <see cref="IPipelineBuilderAsync{TIn, TPrevOut}"/> types.</summary>
    public static class PipelineBuilderExtensions
    {
        #region Pipe - with input, synchronous, return IPipelineBuilder<TIn, TNextOut>

        /// <summary>Adds a new pipeline step to an existing pipeline builder.</summary>
        /// <typeparam name="TIn">The input type for the first step in the pipeline.</typeparam>
        /// <typeparam name="TPrevOut">The output type from the previous step in the pipeline.</typeparam>
        /// <typeparam name="TNextOut">The output type for the step being added to the pipeline.</typeparam>
        /// <param name="prevStep">The builder instance representing the previous step in the pipeline.</param>
        /// <param name="step">The step to be added to the pipeline builder.</param>
        /// <returns>A new pipeline builder representing all steps currently added to the pipeline.</returns>
        public static IPipelineBuilder<TIn, TNextOut> Pipe<TIn, TPrevOut, TNextOut>(
            this IPipelineBuilder<TIn, TPrevOut> prevStep, Func<TPrevOut, TNextOut> step)
        {
            return new PipelineBuilder<TIn, TPrevOut, TNextOut>(prevStep, step);
        }

        /// <summary>Adds a new pipeline step to an existing pipeline builder.</summary>
        /// <typeparam name="TIn">The input type for the first step in the pipeline.</typeparam>
        /// <typeparam name="TPrevOut">The output type from the previous step in the pipeline.</typeparam>
        /// <typeparam name="TNextOut">The output type for the step being added to the pipeline.</typeparam>
        /// <param name="prevStep">The builder instance representing the previous step in the pipeline.</param>
        /// <param name="step">The step to be added to the pipeline builder.</param>
        /// <returns>A new pipeline builder representing all steps currently added to the pipeline.</returns>
        public static IPipelineBuilder<TIn, TNextOut> Pipe<TIn, TPrevOut, TNextOut>(
            this IPipelineBuilder<TIn, TPrevOut> prevStep, IPipelineStep<TPrevOut, TNextOut> step)
        {
            var stepFunc = step.AsFunc();

            return Pipe(prevStep, stepFunc);
        }

        /// <summary>Adds a new pipeline step to an existing pipeline builder.</summary>
        /// <typeparam name="TPipelineStep">The pipeline step type.</typeparam>
        /// <typeparam name="TIn">The input type for the first step in the pipeline.</typeparam>
        /// <typeparam name="TPrevOut">The output type from the previous step in the pipeline.</typeparam>
        /// <typeparam name="TNextOut">The output type for the step being added to the pipeline.</typeparam>
        /// <param name="prevStep">The builder instance representing the previous step in the pipeline.</param>
        /// <returns>A new pipeline builder representing all steps currently added to the pipeline.</returns>
        public static IPipelineBuilder<TIn, TNextOut> Pipe<TPipelineStep, TIn, TPrevOut, TNextOut>(
            this IPipelineBuilder<TIn, TPrevOut> prevStep) where TPipelineStep : IPipelineStep<TPrevOut, TNextOut>, new()
        {
            var step = new TPipelineStep();

            return Pipe(prevStep, step);
        }

        #endregion

        #region Pipe - with input, asynchronous, return IPipelineBuilderAsync<TIn, TNextOut>

        /// <summary>Adds a new asynchronous pipeline step to an existing asynchronous pipeline builder.</summary>
        /// <typeparam name="TIn">The input type for the first step in the pipeline.</typeparam>
        /// <typeparam name="TPrevOut">The output type from the previous step in the pipeline.</typeparam>
        /// <typeparam name="TNextOut">The output type for the step being added to the pipeline.</typeparam>
        /// <param name="prevStep">The builder instance representing the previous step in the pipeline.</param>
        /// <param name="step">The step to be added to the pipeline builder.</param>
        /// <returns>A new pipeline builder representing the asynchronous step just added to the pipeline.</returns>
        public static IPipelineBuilderAsync<TIn, TNextOut> PipeAsync<TIn, TPrevOut, TNextOut>(
           this IPipelineBuilderAsync<TIn, TPrevOut> prevStep, Func<TPrevOut, CancellationToken, Task<TNextOut>> step)
        {
            return new PipelineBuilderAsync<TIn, TPrevOut, TNextOut>(prevStep, step);
        }

        /// <summary>Adds a new asynchronous pipeline step to an existing asynchronous pipeline builder.</summary>
        /// <typeparam name="TIn">The input type for the first step in the pipeline.</typeparam>
        /// <typeparam name="TPrevOut">The output type from the previous step in the pipeline.</typeparam>
        /// <typeparam name="TNextOut">The output type for the step being added to the pipeline.</typeparam>
        /// <param name="prevStep">The builder instance representing the previous step in the pipeline.</param>
        /// <param name="step">The step to be added to the pipeline builder.</param>
        /// <returns>A new pipeline builder representing the asynchronous step just added to the pipeline.</returns>
        public static IPipelineBuilderAsync<TIn, TNextOut> PipeAsync<TIn, TPrevOut, TNextOut>(
            this IPipelineBuilderAsync<TIn, TPrevOut> prevStep, IPipelineStepAsync<TPrevOut, TNextOut> step)
        {
            var stepFunc = step.AsFunc();

            return PipeAsync(prevStep, stepFunc);
        }

        /// <summary>Adds a new asynchronous pipeline step to an existing asynchronous pipeline builder.</summary>
        /// <typeparam name="TPipelineStep">The asynchronous pipeline step type.</typeparam>
        /// <typeparam name="TIn">The input type for the first step in the pipeline.</typeparam>
        /// <typeparam name="TPrevOut">The output type from the previous step in the pipeline.</typeparam>
        /// <typeparam name="TNextOut">The output type for the step being added to the pipeline.</typeparam>
        /// <param name="prevStep">The builder instance representing the previous step in the pipeline.</param>
        /// <returns>A new pipeline builder representing all steps currently added to the pipeline.</returns>
        public static IPipelineBuilderAsync<TIn, TNextOut> PipeAsync<TPipelineStep, TIn, TPrevOut, TNextOut>(
            this IPipelineBuilderAsync<TIn, TPrevOut> prevStep) where TPipelineStep : IPipelineStepAsync<TPrevOut, TNextOut>, new()
        {
            var step = new TPipelineStep();

            return PipeAsync(prevStep, step);
        }

        #endregion

        #region Pipe - with input, synchronous, return IPipelineBuilderAsync<TIn, TNextOut>

        /// <summary>Adds a new non-asynchronous pipeline step to an existing asynchronous pipeline builder.</summary>
        /// <typeparam name="TIn">The input type for the first step in the pipeline.</typeparam>
        /// <typeparam name="TPrevOut">The output type from the previous step in the pipeline.</typeparam>
        /// <typeparam name="TNextOut">The output type for the step being added to the pipeline.</typeparam>
        /// <param name="prevStep">The builder instance representing the previous step in the pipeline.</param>
        /// <param name="step">The step to be added to the pipeline builder.</param>
        /// <returns>A new pipeline builder representing all steps currently added to the pipeline.</returns>
        public static IPipelineBuilderAsync<TIn, TNextOut> Pipe<TIn, TPrevOut, TNextOut>(
           this IPipelineBuilderAsync<TIn, TPrevOut> prevStep, Func<TPrevOut, TNextOut> step)
        {
            _ = step.WhenNotNull(nameof(step));

            Task<TNextOut> stepAsync(TPrevOut result, CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested();

                return Task.FromResult(step.Invoke(result));
            };

            return new PipelineBuilderAsync<TIn, TPrevOut, TNextOut>(prevStep, stepAsync);
        }

        /// <summary>Adds a new non-asynchronous pipeline step to an existing asynchronous pipeline builder.</summary>
        /// <typeparam name="TIn">The input type for the first step in the pipeline.</typeparam>
        /// <typeparam name="TPrevOut">The output type from the previous step in the pipeline.</typeparam>
        /// <typeparam name="TNextOut">The output type for the step being added to the pipeline.</typeparam>
        /// <param name="prevStep">The builder instance representing the previous step in the pipeline.</param>
        /// <param name="step">The step to be added to the pipeline builder.</param>
        /// <returns>A new pipeline builder representing all steps currently added to the pipeline.</returns>
        public static IPipelineBuilderAsync<TIn, TNextOut> Pipe<TIn, TPrevOut, TNextOut>(
            this IPipelineBuilderAsync<TIn, TPrevOut> prevStep, IPipelineStep<TPrevOut, TNextOut> step)
        {
            var stepFunc = step.AsFunc();

            return Pipe(prevStep, stepFunc);
        }

        /// <summary>Adds a new non-asynchronous pipeline step to an existing asynchronous pipeline builder.</summary>
        /// <typeparam name="TPipelineStep">The pipeline step type.</typeparam>
        /// <typeparam name="TIn">The input type for the first step in the pipeline.</typeparam>
        /// <typeparam name="TPrevOut">The output type from the previous step in the pipeline.</typeparam>
        /// <typeparam name="TNextOut">The output type for the step being added to the pipeline.</typeparam>
        /// <param name="prevStep">The builder instance representing the previous step in the pipeline.</param>
        /// <returns>A new pipeline builder representing all steps currently added to the pipeline.</returns>
        public static IPipelineBuilderAsync<TIn, TNextOut> Pipe<TPipelineStep, TIn, TPrevOut, TNextOut>(
            this IPipelineBuilderAsync<TIn, TPrevOut> prevStep) where TPipelineStep : IPipelineStep<TPrevOut, TNextOut>, new()
        {
            var step = new TPipelineStep();

            return Pipe(prevStep, step);
        }

        #endregion

        #region Pipe - with no input, synchronous, return IPipelineBuilder<TIn, TNextOut>

        /// <summary>Adds a new pipeline step to an existing pipeline builder.</summary>
        /// <typeparam name="TPrevOut">The output type from the previous step in the pipeline.</typeparam>
        /// <typeparam name="TNextOut">The output type for the step being added to the pipeline.</typeparam>
        /// <param name="prevStep">The builder instance representing the previous step in the pipeline.</param>
        /// <param name="step">The step to be added to the pipeline builder.</param>
        /// <returns>A new pipeline builder representing all steps currently added to the pipeline.</returns>
        public static IPipelineBuilder<TNextOut> Pipe<TPrevOut, TNextOut>(
            this IPipelineBuilder<TPrevOut> prevStep, Func<TPrevOut, TNextOut> step)
        {
            return new PipelineNoInputBuilder<TPrevOut, TNextOut>(prevStep, step);
        }

        /// <summary>Adds a new pipeline step to an existing pipeline builder.</summary>
        /// <typeparam name="TPrevOut">The output type from the previous step in the pipeline.</typeparam>
        /// <typeparam name="TNextOut">The output type for the step being added to the pipeline.</typeparam>
        /// <param name="prevStep">The builder instance representing the previous step in the pipeline.</param>
        /// <param name="step">The step to be added to the pipeline builder.</param>
        /// <returns>A new pipeline builder representing all steps currently added to the pipeline.</returns>
        public static IPipelineBuilder<TNextOut> Pipe<TPrevOut, TNextOut>(
            this IPipelineBuilder<TPrevOut> prevStep, IPipelineStep<TPrevOut, TNextOut> step)
        {
            var stepFunc = step.AsFunc();

            return Pipe(prevStep, stepFunc);
        }

        /// <summary>Adds a new pipeline step to an existing pipeline builder.</summary>
        /// <typeparam name="TPipelineStep">The pipeline step type.</typeparam>
        /// <typeparam name="TPrevOut">The output type from the previous step in the pipeline.</typeparam>
        /// <typeparam name="TNextOut">The output type for the step being added to the pipeline.</typeparam>
        /// <param name="prevStep">The builder instance representing the previous step in the pipeline.</param>
        /// <returns>A new pipeline builder representing all steps currently added to the pipeline.</returns>
        public static IPipelineBuilder<TNextOut> Pipe<TPipelineStep, TPrevOut, TNextOut>(
            this IPipelineBuilder<TPrevOut> prevStep) where TPipelineStep : IPipelineStep<TPrevOut, TNextOut>, new()
        {
            var step = new TPipelineStep();

            return Pipe(prevStep, step);
        }

        #endregion

        #region Pipe - with no input, asynchronous, return IPipelineBuilderAsync<TIn, TNextOut>

        /// <summary>Adds a new asynchronous pipeline step to an existing asynchronous pipeline builder.</summary>
        /// <typeparam name="TPrevOut">The output type from the previous step in the pipeline.</typeparam>
        /// <typeparam name="TNextOut">The output type for the step being added to the pipeline.</typeparam>
        /// <param name="prevStep">The builder instance representing the previous step in the pipeline.</param>
        /// <param name="step">The step to be added to the pipeline builder.</param>
        /// <returns>A new pipeline builder representing the asynchronous step just added to the pipeline.</returns>
        public static IPipelineBuilderAsync<TNextOut> PipeAsync<TPrevOut, TNextOut>(
           this IPipelineBuilderAsync<TPrevOut> prevStep, Func<TPrevOut, CancellationToken, Task<TNextOut>> step)
        {
            return new PipelineNoInputBuilderAsync<TPrevOut, TNextOut>(prevStep, step);
        }

        /// <summary>Adds a new asynchronous pipeline step to an existing asynchronous pipeline builder.</summary>
        /// <typeparam name="TPrevOut">The output type from the previous step in the pipeline.</typeparam>
        /// <typeparam name="TNextOut">The output type for the step being added to the pipeline.</typeparam>
        /// <param name="prevStep">The builder instance representing the previous step in the pipeline.</param>
        /// <param name="step">The step to be added to the pipeline builder.</param>
        /// <returns>A new pipeline builder representing the asynchronous step just added to the pipeline.</returns>
        public static IPipelineBuilderAsync<TNextOut> PipeAsync<TPrevOut, TNextOut>(
            this IPipelineBuilderAsync<TPrevOut> prevStep, IPipelineStepAsync<TPrevOut, TNextOut> step)
        {
            var stepFunc = step.AsFunc();

            return PipeAsync(prevStep, stepFunc);
        }

        /// <summary>Adds a new asynchronous pipeline step to an existing asynchronous pipeline builder.</summary>
        /// <typeparam name="TPipelineStep">The asynchronous pipeline step type.</typeparam>
        /// <typeparam name="TPrevOut">The output type from the previous step in the pipeline.</typeparam>
        /// <typeparam name="TNextOut">The output type for the step being added to the pipeline.</typeparam>
        /// <param name="prevStep">The builder instance representing the previous step in the pipeline.</param>
        /// <returns>A new pipeline builder representing all steps currently added to the pipeline.</returns>
        public static IPipelineBuilderAsync<TNextOut> PipeAsync<TPipelineStep, TPrevOut, TNextOut>(this IPipelineBuilderAsync<TPrevOut> prevStep)
            where TPipelineStep : IPipelineStepAsync<TPrevOut, TNextOut>, new()
        {
            var step = new TPipelineStep();

            return PipeAsync(prevStep, step);
        }

        #endregion

        #region Pipe - with no input, synchronous, return IPipelineBuilderAsync<TIn, TNextOut>

        /// <summary>Adds a new non-asynchronous pipeline step to an existing asynchronous pipeline builder.</summary>
        /// <typeparam name="TPrevOut">The output type from the previous step in the pipeline.</typeparam>
        /// <typeparam name="TNextOut">The output type for the step being added to the pipeline.</typeparam>
        /// <param name="prevStep">The builder instance representing the previous step in the pipeline.</param>
        /// <param name="step">The step to be added to the pipeline builder.</param>
        /// <returns>A new pipeline builder representing all steps currently added to the pipeline.</returns>
        public static IPipelineBuilderAsync<TNextOut> Pipe<TPrevOut, TNextOut>(
           this IPipelineBuilderAsync<TPrevOut> prevStep, Func<TPrevOut, TNextOut> step)
        {
            _ = step.WhenNotNull(nameof(step));

            Task<TNextOut> stepAsync(TPrevOut result, CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested();

                return Task.FromResult(step.Invoke(result));
            };

            return new PipelineNoInputBuilderAsync<TPrevOut, TNextOut>(prevStep, stepAsync);
        }

        /// <summary>Adds a new non-asynchronous pipeline step to an existing asynchronous pipeline builder.</summary>
        /// <typeparam name="TPrevOut">The output type from the previous step in the pipeline.</typeparam>
        /// <typeparam name="TNextOut">The output type for the step being added to the pipeline.</typeparam>
        /// <param name="prevStep">The builder instance representing the previous step in the pipeline.</param>
        /// <param name="step">The step to be added to the pipeline builder.</param>
        /// <returns>A new pipeline builder representing all steps currently added to the pipeline.</returns>
        public static IPipelineBuilderAsync<TNextOut> Pipe<TPrevOut, TNextOut>(
            this IPipelineBuilderAsync<TPrevOut> prevStep, IPipelineStep<TPrevOut, TNextOut> step)
        {
            var stepFunc = step.AsFunc();

            return Pipe(prevStep, stepFunc);
        }

        /// <summary>Adds a new non-asynchronous pipeline step to an existing asynchronous pipeline builder.</summary>
        /// <typeparam name="TPipelineStep">The pipeline step type.</typeparam>
        /// <typeparam name="TPrevOut">The output type from the previous step in the pipeline.</typeparam>
        /// <typeparam name="TNextOut">The output type for the step being added to the pipeline.</typeparam>
        /// <param name="prevStep">The builder instance representing the previous step in the pipeline.</param>
        /// <returns>A new pipeline builder representing all steps currently added to the pipeline.</returns>
        public static IPipelineBuilderAsync<TNextOut> Pipe<TPipelineStep, TPrevOut, TNextOut>(
            this IPipelineBuilderAsync<TPrevOut> prevStep) where TPipelineStep : IPipelineStep<TPrevOut, TNextOut>, new()
        {
            var step = new TPipelineStep();

            return Pipe(prevStep, step);
        }

        #endregion
    }
}