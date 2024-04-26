using AllOverIt.GenericHost;
using Microsoft.Extensions.Options;

namespace OptionsValidationDemo
{
    public sealed class ConsoleApp : ConsoleAppBase
    {
        private readonly AppOptions _options;

        public ConsoleApp(IOptions<AppOptions> options)
        {
            _options = options.Value;
        }

        public override Task StartAsync(CancellationToken cancellationToken = default)
        {
            Console.WriteLine($"MinLevel = {_options.MinLevel}");

            Console.WriteLine();
            Console.WriteLine("All Over It.");
            Console.WriteLine();
            Console.ReadKey();

            ExitCode = 0;

            return Task.CompletedTask;
        }
    }
}