//using AllOverIt.Validation;
//using FluentValidation;
//using System;
//using ValidationViaDependencyInjection.Models;

//namespace ValidationViaDependencyInjection.Validators
//{
//    public sealed class PersonWithIdValidator : ValidatorBase<(Person Person, Guid Id)>
//    {
//        public PersonWithIdValidator()
//        {
//            RuleFor(model => model.Person).SetValidator(new IsValidPersonValidator());
//            RuleFor(model => model.Id).NotEmpty().WithName("Id");
//        }
//    }
//}