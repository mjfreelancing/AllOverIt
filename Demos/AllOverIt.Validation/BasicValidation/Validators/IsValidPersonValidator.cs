using AllOverIt.Validation;
using AllOverIt.Validation.Extensions;
using BasicValidation.Models;

namespace BasicValidation.Validators
{
    // Validates using explicit rules (no context)
    public sealed class IsValidPersonValidator : ValidatorBase<Person>
    {
        public IsValidPersonValidator()
        {
            // Only adding multiple rules to keep the number of validators to a minimum

            // methods starting with IsXXX are extension methods provided by AllOverIt.Validation
            RuleFor(person => person.Age).IsGreaterThan(21);
            RuleFor(person => person.Age).IsNotEmpty();       // same as checking for a default (zero) value
            RuleFor(person => person.FirstName).IsNotEmpty();
            RuleFor(person => person.FirstName).IsRequired();
            RuleFor(person => person.LastName).IsNotEmpty();
            RuleFor(person => person.LastName).IsRequired();
        }
    }
}