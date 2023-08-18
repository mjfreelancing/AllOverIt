using AllOverIt.Patterns.Pipeline;
using AllOverIt.Patterns.Pipeline.Extensions;
using Microsoft.Extensions.DependencyInjection;
using PipelineAsyncDemo.Steps;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PipelineAsyncDemo
{
    public delegate Task<string> IntegerPipelineProcessorAsync(int value, CancellationToken cancellationToken);

    internal class Program
    {
        static async Task Main()
        {
            var p1 = PipelineBuilder
                .Pipe(() => 1)
                .Pipe(v => v * 2.0d)
                .Pipe(v => (int) (v * 3))
                .Pipe(v => v * 3)
                .Pipe(new DoubleStep())
                .Pipe(v => v * 3)
                .Pipe(v => v * 2.0d)
                .Pipe(v => v.ToString())
                .Build();

            var r1 = p1.Invoke();

            var p2 = PipelineBuilder
                .PipeAsync(cancellationToken => Task.FromResult(1))
                .PipeAsync((v, cancellationToken) => Task.FromResult(v * 2.0d))
                .Pipe(v => (int)(v * 3))
                .Pipe(v => v * 3)
                .PipeAsync(new DoubleStepAsync())
                .Pipe(v => v * 3)
                .PipeAsync((v, cancellationToken) => Task.FromResult(v * 2.0d))
                .PipeAsync((v, cancellationToken) => Task.FromResult(v.ToString()))
                .Build();

            var r2 = await p2.Invoke(CancellationToken.None);

            var services = new ServiceCollection();

            services.AddSingleton<AddFractionStep>();

            // Showing how to register using a IntegerPipelineProcessorAsync delegate
            services.AddSingleton(provider =>
            {
                var addFractionStep = provider.GetRequiredService<AddFractionStep>();

                var pipeline = PipelineBuilder

                   // Using a class type - implemented as async
                   // Only <TPipelineStep, TIn, TOut> because there is no previous input
                   .PipeAsync<DoubleStepAsync, int, int>()

                   // Using a class instance - implemented as async
                   .PipeAsync(new DoubleStepAsync())

                   // Using a class type (TPipelineStep) - implemented as non-async
                   // TPipelineStep, TIn, TPrevOut, TNextOut
                   //   TIn is the original input type
                   //   TPrevOut is the result type from the previous call
                   //   TNextOut is the result type for this next step
                   .Pipe<DoubleStep, int, int, int>()

                   // Using a non-async Func
                   .Pipe(value => value * 2)

                   .PipeAsync(addFractionStep)           // << Much easier this way if using DI

                   .Pipe(value => $"{value}")

                   // Get the resulting Func that has everything merged together
                   .Build();

                return new IntegerPipelineProcessorAsync(pipeline);
            });

            var serviceProvider = services.BuildServiceProvider();

            var pipeline = serviceProvider.GetRequiredService<IntegerPipelineProcessorAsync>();

            var cts = new CancellationTokenSource();
            cts.Cancel();

            var result = await pipeline.Invoke(5, cts.Token /*CancellationToken.None*/);      // Will return 5 * 2 * 2 * 2 * 2 + 0.2 = 80.2

            Console.WriteLine($"{result}");

            Console.WriteLine();
            Console.WriteLine("All Over It.");
            Console.ReadKey();
        }
    }
}