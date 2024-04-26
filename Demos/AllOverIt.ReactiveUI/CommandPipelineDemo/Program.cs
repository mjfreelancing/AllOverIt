using AllOverIt.Patterns.Pipeline.Extensions;
using AllOverIt.ReactiveUI.CommandPipeline;
using AllOverIt.ReactiveUI.Extensions;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace CommandPipelineDemo
{
    internal class Program
    {
        static async Task Main()
        {
            var subscriptions = new CompositeDisposable();

            var produceIntegerCommand = ProduceIntegerCreateCommand(subscriptions);
            var addFractionCommand = AddFractionCreateCommand(subscriptions);
            var convertToStringCommand = ConvertToStringCommand(subscriptions);

            Console.WriteLine();
            Console.WriteLine("Using ReactiveCommandPipelineBuilder");
            Console.WriteLine();

            // Using ReactiveCommandPipelineBuilder
            var pipeline1 = CreatePipelineUsingReactiveCommandPipelineBuilder(produceIntegerCommand, addFractionCommand, convertToStringCommand);
            var result1 = await pipeline1.Invoke(Unit.Default, CancellationToken.None);

            // This may output before all of the subscription based logs do
            Console.WriteLine();
            Console.WriteLine($"Pipeline result = {result1}");

            Console.WriteLine();
            Console.WriteLine("Using extension method");
            Console.WriteLine();

            // Using extension method
            var pipeline2 = CreatePipelineUsingExtensionMethod(produceIntegerCommand, addFractionCommand, convertToStringCommand);
            var result2 = await pipeline2.Invoke(Unit.Default, CancellationToken.None);

            // This may output before all of the subscription based logs do
            Console.WriteLine();
            Console.WriteLine($"Pipeline result = {result2}");

            // Wait a little while for the subscription messages to complete
            await Task.Delay(1000).ConfigureAwait(false);

            // Kill any that have not yet processed
            subscriptions.Dispose();

            Console.WriteLine();
            Console.WriteLine("All Over It.");
            Console.ReadKey();
        }

        private static ReactiveCommand<Unit, int> ProduceIntegerCreateCommand(CompositeDisposable subscriptions)
        {
            var command = ReactiveCommand.Create(() => 1);

            command
                .Subscribe(value =>
                {
                    Console.WriteLine($" > Produce Integer: {value}");
                })
                .DisposeWith(subscriptions);

            return command;
        }

        private static ReactiveCommand<int, double> AddFractionCreateCommand(CompositeDisposable subscriptions)
        {
            var command = ReactiveCommand.Create<int, double>(value =>
            {
                Console.WriteLine($" - Add 3.25");
                return value + 3.25d;
            });

            command
                .Subscribe(value =>
                {
                    Console.WriteLine($" > Add fraction result: {value}");
                })
                .DisposeWith(subscriptions);

            return command;
        }

        private static ReactiveCommand<double, string> ConvertToStringCommand(CompositeDisposable subscriptions)
        {
            var command = ReactiveCommand.Create<double, string>(v => $"{v}");

            command
                .Subscribe(value =>
                {
                    Console.WriteLine($" > As a string: {value}");
                })
                .DisposeWith(subscriptions);

            return command;
        }

        private static Func<double, CancellationToken, Task<double>> MultiplyDoubleByThreeAsync()
        {
            static Task<double> func(double value, CancellationToken cancellationToken)
            {
                Console.WriteLine(" - Multiple by 3");
                return Task.FromResult(value * 3.0d);
            }

            return func;
        }

        private static Func<string, double> ParseStringToDouble()
        {
            static double func(string value)
            {
                Console.WriteLine($" - Parse the value {value} back to a double");
                return double.Parse(value);
            }

            return func;
        }

        private static Func<double, int> FloorDoubleValue()
        {
            static int func(double value)
            {
                Console.WriteLine($" - Floor the current value of {value}");
                return (int) Math.Floor(value);
            }

            return func;
        }

        private static Func<Unit, CancellationToken, Task<double>> CreatePipelineUsingReactiveCommandPipelineBuilder(ReactiveCommand<Unit, int> produceIntegerCommand,
            ReactiveCommand<int, double> addFractionCommand, ReactiveCommand<double, string> convertToStringCommand)
        {
            return ReactiveCommandPipelineBuilder
                .Pipe(produceIntegerCommand)
                .Pipe(addFractionCommand)
                .PipeAsync(MultiplyDoubleByThreeAsync())
                .Pipe(convertToStringCommand)
                .Pipe(ParseStringToDouble())
                .Pipe(FloorDoubleValue())
                .Pipe(addFractionCommand)
                .Build();
        }

        private static Func<Unit, CancellationToken, Task<double>> CreatePipelineUsingExtensionMethod(ReactiveCommand<Unit, int> produceIntegerCommand,
            ReactiveCommand<int, double> addFractionCommand, ReactiveCommand<double, string> convertToStringCommand)
        {
            return produceIntegerCommand
                .Pipe(addFractionCommand)
                .PipeAsync(MultiplyDoubleByThreeAsync())
                .Pipe(convertToStringCommand)
                .Pipe(ParseStringToDouble())
                .Pipe(FloorDoubleValue())
                .Pipe(addFractionCommand)
                .Build();
        }
    }
}