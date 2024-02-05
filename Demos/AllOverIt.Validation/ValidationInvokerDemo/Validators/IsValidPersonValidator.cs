using AllOverIt.Validation;
using AllOverIt.Validation.Extensions;

public sealed class IsValidPersonValidator : ValidatorBase<Person>
{
    // When used with IValidationInvoker, a default constructor is required
    public IsValidPersonValidator()
    {
        // methods starting with IsXXX are extension methods provided by AllOverIt.Validation
        RuleFor(person => person.Age).IsGreaterThan(21);
        RuleFor(person => person.Age).IsNotEmpty();       // same as checking for a default (zero) value
        RuleFor(person => person.FirstName).IsNotEmpty();
        RuleFor(person => person.FirstName).IsRequired();
        RuleFor(person => person.LastName).IsNotEmpty();
        RuleFor(person => person.LastName).IsRequired();
    }
}
