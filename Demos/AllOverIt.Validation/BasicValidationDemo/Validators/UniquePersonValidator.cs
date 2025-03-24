using AllOverIt.Validation;
using AllOverIt.Validation.Extensions;
using BasicValidationDemo.Models;

namespace BasicValidationDemo.Validators
{
    public sealed class UniquePersonValidator : ValidatorBase<People>
    {
        public UniquePersonValidator()
        {
            // There's support for up to 4 properties to be unique
            RuleFor(people => people.Items).IsUnique(person => person.FirstName, person => person.LastName);
        }
    }
}