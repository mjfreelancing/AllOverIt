using AllOverIt.Extensions;
using AllOverIt.Validation;
using AllOverIt.Validation.Extensions;
using BasicValidation.Models;
using FluentValidation;

namespace BasicValidation.Validators
{
    // The rules utilize values provided by a context at the time of invocation
    public sealed class IsValidPersonContextValidator : ValidatorBase<Person>
    {
        public IsValidPersonContextValidator()
        {
            // using IsGreaterThanOrEqualTo from AllOverIt.Validation
            RuleFor(person => person.Age).IsGreaterThanOrEqualTo<Person, int, PersonContext>(ctx => ctx.MinimumAge);

            // using a custom rule (as per FluentValidation)
            RuleFor(person => person.LastName)
                .Custom((value, context) =>
                {
                    // access custom data attached to the context
                    var personContext = ValidationContextExtensions.GetContextData<Person, PersonContext>(context);

                    if (!personContext.LastNameIsOptional && StringExtensions.IsNullOrEmpty(value))
                    {
                        context.AddFailure($"'{context.PropertyName}' requires a value (not optional).");
                    }
                });

            // can still use regular rules
            RuleFor(person => person.FirstName).IsRequired();
        }
    }
}