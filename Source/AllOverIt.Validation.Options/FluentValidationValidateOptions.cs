using AllOverIt.Assertion;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace AllOverIt.Validation.Options
{
    internal sealed class FluentValidationValidateOptions<TOptions> : IValidateOptions<TOptions> where TOptions : class
    {
        private readonly IValidator<TOptions> _validator;
        public string? Name { get; }

        public FluentValidationValidateOptions(string? name, IValidator<TOptions> validator)
        {
            _validator = validator.WhenNotNull();

            Name = name;    // Can be null / empty
        }

        public ValidateOptionsResult Validate(string? name, TOptions options)
        {
            _ = options.WhenNotNull();

            // A null name is used to configure all named options
            if (Name is not null && Name != name)
            {
                // Ignored if not validating this instance
                return ValidateOptionsResult.Skip;
            }

            var validationResult = _validator.Validate(options);

            if (validationResult.IsValid)
            {
                return ValidateOptionsResult.Success;
            }

            var errors = validationResult.Errors.Select(failure => failure.ErrorMessage);

            return ValidateOptionsResult.Fail(errors);
        }
    }
}