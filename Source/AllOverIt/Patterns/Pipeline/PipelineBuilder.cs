﻿using AllOverIt.Assertion;
using AllOverIt.Patterns.Pipeline.Extensions;
using System;
using System.Threading.Tasks;

namespace AllOverIt.Patterns.Pipeline
{
    public static class PipelineBuilder
    {
        public static IPipelineBuilder<TIn, TOut> Pipe<TIn, TOut>(Func<TIn, TOut> step)
        {
            return new PipelineBuilder<TIn, TOut>(step);
        }

        public static IPipelineBuilder<TIn, TOut> Pipe<TIn, TOut>(IPipelineStep<TIn, TOut> step)
        {
            // AsFunc() performs a null check
            return new PipelineBuilder<TIn, TOut>(step.AsFunc());
        }

        public static IPipelineBuilder<TIn, TOut> Pipe<TPipelineStep, TIn, TOut>() where TPipelineStep : IPipelineStep<TIn, TOut>, new()
        {
            var step = new TPipelineStep();
            return Pipe(step);
        }

        public static IPipelineBuilderAsync<TIn, TOut> PipeAsync<TIn, TOut>(Func<TIn, Task<TOut>> step)
        {
            return new PipelineBuilderAsync<TIn, TOut>(step);
        }

        public static IPipelineBuilderAsync<TIn, TOut> PipeAsync<TIn, TOut>(IPipelineStepAsync<TIn, TOut> step)
        {
            // AsFunc() performs a null check
            return new PipelineBuilderAsync<TIn, TOut>(step.AsFunc());
        }

        public static IPipelineBuilderAsync<TIn, TOut> PipeAsync<TPipelineStep, TIn, TOut>() where TPipelineStep : IPipelineStepAsync<TIn, TOut>, new()
        {
            var step = new TPipelineStep();

            return PipeAsync(step);
        }
    }

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

    internal sealed class PipelineBuilder<TIn, TPrevOut, TNextOut> : IPipelineBuilder<TIn, TNextOut>
    {
        private readonly IPipelineBuilder<TIn, TPrevOut> _prevStep;
        private readonly Func<TPrevOut, TNextOut> _step;

        public PipelineBuilder(IPipelineBuilder<TIn, TPrevOut> prevStep, Func<TPrevOut, TNextOut> step)
        {
            _prevStep = prevStep.WhenNotNull(nameof(prevStep));
            _step = step.WhenNotNull(nameof(step));
        }

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