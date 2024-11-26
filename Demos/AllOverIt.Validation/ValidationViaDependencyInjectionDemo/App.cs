using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.GenericHost;
using FluentValidation;
using Microsoft.Extensions.Logging;
using ValidationViaDependencyInjectionDemo.Models;

namespace ValidationViaDependencyInjectionDemo
{
    public sealed class App : ConsoleAppBase
    {
        private readonly IValidator<Person> _personValidator;
        private readonly ILogger<App> _logger;
        public App(IValidator<Person> personValidator, ILogger<App> logger)
        {
            _personValidator = personValidator.WhenNotNull();
            _logger = logger.WhenNotNull();

            _logger.LogInformation("The {ValidatorName} validator has been injected", personValidator.GetType().GetFriendlyName());

            Console.WriteLine();
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("StartAsync");

            var person = new Person();
            var validationResult = _personValidator.Validate(person);

            _logger.LogInformation($"A default constructed {nameof(Person)} has the following validation errors:");

            var errors = validationResult.Errors.Select(item => item.ErrorMessage);

            _logger.LogError("{Errors}", string.Join($"  {Environment.NewLine}", errors));

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