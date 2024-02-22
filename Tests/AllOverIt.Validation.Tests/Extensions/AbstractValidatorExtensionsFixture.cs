using AllOverIt.Fixture;
using AllOverIt.Validation.Extensions;
using FluentValidation.Results;
using System;
using System.Threading.Tasks;

namespace AllOverIt.Validation.Tests.Extensions
{
    public class AbstractValidatorExtensionsFixture : FixtureBase
    {
        private class DummyModel
        {
            public int? ValueOne { get; set; }
            public Guid ValueTwo { get; set; }
        }

        private class CustomRuleForValidator : ValidatorBase<DummyModel>
        {
            public CustomRuleForValidator()
            {
                this.CustomRuleFor(model => model.ValueOne, (value, context) =>
                {
                    if (value == default)
                    {
                        context.AddFailure(new ValidationFailure(nameof(DummyModel.ValueOne), "Cannot be null"));
                    }
                });

                // A rule for more than one property
                this.CustomRuleFor(model => model, (model, context) =>
                {
                    if (model?.ValueOne > 10 && model.ValueTwo != default)
                    {
                        context.AddFailure(new ValidationFailure($"{nameof(DummyModel.ValueOne)} / {nameof(DummyModel.ValueTwo)}", "Bad Combination"));
                    }
                });
            }
        }

        private class CustomRuleForAsyncValidator : ValidatorBase<DummyModel>
        {
            public CustomRuleForAsyncValidator()
            {
                // A rule for a single property
                this.CustomRuleForAsync(model => model.ValueOne, (value, context, cancellationToken) =>
                {
                    if (value == default)
                    {
                        context.AddFailure(new ValidationFailure(nameof(DummyModel.ValueOne), "Cannot be null"));
                    }

                    return Task.CompletedTask;
                });

                // A rule for more than one property
                this.CustomRuleForAsync(model => model, (model, context, cancellationToken) =>
                {
                    if (model?.ValueOne > 10 && model.ValueTwo != default)
                    {
                        context.AddFailure(new ValidationFailure($"{nameof(DummyModel.ValueOne)} / {nameof(DummyModel.ValueTwo)}", "Bad Combination"));
                    }

                    return Task.CompletedTask;
                });
            }
        }

        private class ConditionalCustomRuleForValidator : ValidatorBase<DummyModel>
        {
            public ConditionalCustomRuleForValidator()
            {
                this.ConditionalCustomRuleFor(
                    model => model.ValueOne,
                    value => value != default,      // Condition to execute the custom rule
                    (value, context) =>
                    {
                        if (value > 10)
                        {
                            context.AddFailure(new ValidationFailure(nameof(DummyModel.ValueOne), "Bad Value"));
                        }
                    });

                // A rule for more than one property
                this.ConditionalCustomRuleFor(
                    model => model,
                    model => model.ValueOne != default && model.ValueTwo != default,    // Condition to execute the custom rule
                    (model, context) =>
                    {
                        if (model.ValueOne > 10)
                        {
                            context.AddFailure(new ValidationFailure($"{nameof(DummyModel.ValueOne)} / {nameof(DummyModel.ValueTwo)}", "Bad Value"));
                        }
                    });
            }
        }

        private class ConditionalCustomRuleForAsyncValidator : ValidatorBase<DummyModel>
        {
            public ConditionalCustomRuleForAsyncValidator()
            {
                this.ConditionalCustomRuleForAsync(
                    model => model.ValueOne,
                    value => value != default,      // Condition to execute the custom rule
                    (value, context, cancellationToken) =>
                    {
                        if (value > 10)
                        {
                            context.AddFailure(new ValidationFailure(nameof(DummyModel.ValueOne), "Bad Value"));
                        }

                        return Task.CompletedTask;
                    });

                // A rule for more than one property
                this.ConditionalCustomRuleForAsync(
                    model => model,
                    model => model.ValueOne != default && model.ValueTwo != default,    // Condition to execute the custom rule
                    (model, context, cancellationToken) =>
                    {
                        if (model.ValueOne > 10)
                        {
                            context.AddFailure(new ValidationFailure($"{nameof(DummyModel.ValueOne)} / {nameof(DummyModel.ValueTwo)}", "Bad Value"));
                        }

                        return Task.CompletedTask;
                    });
            }
        }



        public class CustomRuleFor : AbstractValidatorExtensionsFixture
        {

        }
        public class CustomRuleForAsync : AbstractValidatorExtensionsFixture
        {

        }
        public class ConditionalCustomRuleFor : AbstractValidatorExtensionsFixture
        {

        }
        public class ConditionalCustomRuleForAsync : AbstractValidatorExtensionsFixture
        {

        }
    }
}