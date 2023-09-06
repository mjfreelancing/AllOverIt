using AllOverIt.Extensions;
using AllOverIt.Validation;
using AllOverIt.Validation.Extensions;
using BasicValidationDemo.Models;
using FluentValidation;

namespace BasicValidationDemo.Validators
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
                    var personContext = context.GetContextData<Person, PersonContext>();

                    if (!personContext.LastNameIsOptional && value.IsNullOrEmpty())
                    {
                        context.AddFailure($"'{context.PropertyPath}' requires a value (not optional).");
                    }
                });

            // can still use regular rules
            RuleFor(person => person.FirstName).IsRequired();
        }
    }
}