using System;
using System.Threading;
using System.Threading.Tasks;
using AllOverIt.Assertion;
using AllOverIt.GenericHost;
using FluentValidation;
using Microsoft.Extensions.Logging;
using ValidationViaDependencyInjection.Models;

namespace ValidationViaDependencyInjection
{
    public sealed class App : ConsoleAppBase
    {
        private readonly IValidator<Person> _personValidator;
        private readonly ILogger<App> _logger;
        public App(IValidator<Person> personValidator, ILogger<App> logger)
        {
            _personValidator = personValidator.WhenNotNull(nameof(personValidator));
            _logger = logger.WhenNotNull(nameof(logger));

            Console.WriteLine();
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("StartAsync");

            var person = new Person();
            var validationResult = _personValidator.Validate(person);


            ExitCode = 0;

            Console.WriteLine();
            Console.WriteLine("All Over It.");
            Console.WriteLine();
            Console.ReadKey();

            return Task.CompletedTask;
        }

        public override void OnStopping()
        {
            _logger.LogInformation("App is stopping");
        }

        public override void OnStopped()
        {
            _logger.LogInformation("App is stopped");
        }
    }
}