using AllOverIt.Validation;
using BasicValidation.Models;
using FluentValidation;
using System;

namespace BasicValidation.Validators
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