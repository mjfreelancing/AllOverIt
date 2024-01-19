using AllOverIt.Validation;
using BasicValidationDemo.Models;
using FluentValidation;
using System;

namespace BasicValidationDemo.Validators
{
    public sealed class PersonWithIdValidator : ValidatorBase<(Person Person, Guid Id)>
    {
        public PersonWithIdValidator()
        {
            RuleFor(model => model.Person).SetValidator(new IsValidPersonValidator());
            RuleFor(model => model.Id).NotEmpty().WithName("Id");
        }
    }
}