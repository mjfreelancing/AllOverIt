﻿using AllOverIt.Assertion;
using AllOverIt.GenericHost;
using AllOverIt.Validation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using ValidationInvokerDemo.Models;

namespace ValidationInvokerDemo
{
    public sealed class App : ConsoleAppBase
    {
        private readonly IValidationInvoker _validationInvoker;
        private readonly ILifetimeValidationInvoker _serviceValidationInvoker;
        private readonly ILogger<App> _logger;
        public App(IValidationInvoker validationInvoker, ILifetimeValidationInvoker serviceValidationInvoker, ILogger<App> logger)
        {
            _validationInvoker = validationInvoker.WhenNotNull();
            _serviceValidationInvoker = serviceValidationInvoker.WhenNotNull();
            _logger = logger.WhenNotNull(nameof(logger));

            //_logger.LogInformation($"The {personValidator.GetType().GetFriendlyName()} validator has been injected");

            Console.WriteLine();
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("StartAsync");

            var errors = UseValidationInvoker();

            _logger.LogError(string.Join($"  {Environment.NewLine}", errors));

            Console.WriteLine();

            errors = UseServiceValidationInvoker();

            _logger.LogError(string.Join($"  {Environment.NewLine}", errors));

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

        private List<ValidationFailure> UseValidationInvoker()
        {
            var person = new Person();

            // Or use AssertValidation() to force an exception to be thrown
            var validationResult = _validationInvoker.Validate(person);

            _logger.LogInformation($"A default constructed '{nameof(Person)}' has the following validation errors:");

            return validationResult.Errors;
        }

        private List<ValidationFailure> UseServiceValidationInvoker()
        {
            var address = new Address();

            // Or use AssertValidation() to force an exception to be thrown
            var validationResult = _serviceValidationInvoker.Validate(address);

            _logger.LogInformation($"A default constructed '{nameof(Address)}' has the following validation errors:");

            return validationResult.Errors;
        }
    }
}