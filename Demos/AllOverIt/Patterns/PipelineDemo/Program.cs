using AllOverIt.Patterns.Pipeline;
using AllOverIt.Patterns.Pipeline.Extensions;
using Microsoft.Extensions.DependencyInjection;
using PipelineDemo.Steps;
using System;

namespace PipelineDemo
{
    public delegate string IntegerPipelineProcessor(int value);

    internal class Program
    {
        static void Main()
        {
            var services = new ServiceCollection();

            services.AddSingleton<AddFractionStep>();

            // Showing how to register using a IntegerPipelineProcessor delegate
            services.AddSingleton(provider =>
            {
                var addFractionStep = provider.GetRequiredService<AddFractionStep>();

                var pipeline = PipelineBuilder

                   // Using a class type
                   // Only <TPipelineStep, TIn, TOut> because there is no previous input
                   .Pipe<DoubleStep, int, int>()

                   // Using a class instance
                   .Pipe(new DoubleStep())

                   // Using a class type (TPipelineStep)
                   // TPipelineStep, TIn, TPrevOut, TNextOut
                   //   TIn is the original input type
                   //   TPrevOut is the result type from the previous call
                   //   TNextOut is the result type for this next step
                   .Pipe<DoubleStep, int, int, int>()

                   // Using a Func
                   .Pipe(value => value * 2)

                   .Pipe(addFractionStep)           // << Much easier this way if using DI

                   .Pipe(value => $"{value}")

                   // Get the resulting Func that has everything merged together
                   .Build();

                return new IntegerPipelineProcessor(pipeline);
            });

            var serviceProvider = services.BuildServiceProvider();

            var pipeline = serviceProvider.GetRequiredService<IntegerPipelineProcessor>();            

            var result = pipeline.Invoke(5);

            Console.WriteLine($"{result}");

            Console.WriteLine();
            Console.WriteLine("All Over It.");
            Console.ReadKey();
        }
    }
}