using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Validation.Extensions;
using FluentValidation.Results;

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
                // A rule for a single property
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
                // A rule for a single property
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
                        if (model.ValueOne < 10)
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
                        if (model.ValueOne < 10)
                        {
                            context.AddFailure(new ValidationFailure($"{nameof(DummyModel.ValueOne)} / {nameof(DummyModel.ValueTwo)}", "Bad Value"));
                        }

                        return Task.CompletedTask;
                    });
            }
        }

        private readonly DummyModel _model = new();

        public class CustomRuleFor : AbstractValidatorExtensionsFixture
        {
            private readonly CustomRuleForValidator _validator = new();

            [Fact]
            public void Should_Throw_When_Validator_Null()
            {
                var exception = Should.Throw<ArgumentNullException>(() =>
                {
                    AbstractValidatorExtensions.CustomRuleFor<DummyModel, DummyModel>(null, model => model, (_, _) => { });
                });
                exception.WithNamedMessageWhenNull("validator");
            }

            [Fact]
            public void Should_Throw_When_PropertyExpression_Null()
            {
                var exception = Should.Throw<ArgumentNullException>(() =>
                {
                    AbstractValidatorExtensions.CustomRuleFor<DummyModel, DummyModel>(_validator, null, (_, _) => { });
                });
                exception.WithNamedMessageWhenNull("propertyExpression");
            }

            [Fact]
            public void Should_Throw_When_Action_Null()
            {
                var exception = Should.Throw<ArgumentNullException>(() =>
                {
                    AbstractValidatorExtensions.CustomRuleFor<DummyModel, DummyModel>(_validator, model => model, null);
                });
                exception.WithNamedMessageWhenNull("action");
            }

            [Fact]
            public void Should_Not_Have_Error()
            {
                _model.ValueOne = Create<int>();

                var errors = _validator.Validate(_model).Errors;

                errors.ShouldBeEmpty();
            }

            [Fact]
            public void Should_Validate_Single_Property()
            {
                var errors = _validator.Validate(_model).Errors;

                errors.Single().ErrorMessage.ShouldBe("Cannot be null");
            }

            [Fact]
            public void Should_Validate_Two_Properties()
            {
                _model.ValueTwo = Guid.NewGuid();

                var errors = _validator.Validate(_model).Errors;

                errors.Single().ErrorMessage.ShouldBe("Cannot be null");

                _model.ValueOne = GetWithinRange(11, 100);

                errors = _validator.Validate(_model).Errors;

                errors.Single().ErrorMessage.ShouldBe("Bad Combination");
            }
        }

        public class CustomRuleForAsync : AbstractValidatorExtensionsFixture
        {
            private readonly CustomRuleForAsyncValidator _validator = new();

            [Fact]
            public void Should_Throw_When_Validator_Null()
            {
                var exception = Should.Throw<ArgumentNullException>(() =>
                {
                    AbstractValidatorExtensions.CustomRuleForAsync<DummyModel, DummyModel>(null, model => model, (_, _, _) => Task.CompletedTask);
                });
                exception.WithNamedMessageWhenNull("validator");
            }

            [Fact]
            public void Should_Throw_When_PropertyExpression_Null()
            {
                var exception = Should.Throw<ArgumentNullException>(() =>
                {
                    AbstractValidatorExtensions.CustomRuleForAsync<DummyModel, DummyModel>(_validator, null, (_, _, _) => Task.CompletedTask);
                });
                exception.WithNamedMessageWhenNull("propertyExpression");
            }

            [Fact]
            public void Should_Throw_When_Action_Null()
            {
                var exception = Should.Throw<ArgumentNullException>(() =>
                {
                    AbstractValidatorExtensions.CustomRuleForAsync<DummyModel, DummyModel>(_validator, model => model, null);
                });
                exception.WithNamedMessageWhenNull("action");
            }

            [Fact]
            public async Task Should_Not_Have_Error()
            {
                _model.ValueOne = Create<int>();

                var errors = (await _validator.ValidateAsync(_model)).Errors;

                errors.ShouldBeEmpty();
            }

            [Fact]
            public async Task Should_Validate_Single_Property()
            {
                var errors = (await _validator.ValidateAsync(_model)).Errors;

                errors.Single().ErrorMessage.ShouldBe("Cannot be null");
            }

            [Fact]
            public async Task Should_Validate_Two_Properties()
            {
                _model.ValueTwo = Guid.NewGuid();

                var errors = (await _validator.ValidateAsync(_model)).Errors;

                errors.Single().ErrorMessage.ShouldBe("Cannot be null");

                _model.ValueOne = GetWithinRange(11, 100);

                errors = (await _validator.ValidateAsync(_model)).Errors;

                errors.Single().ErrorMessage.ShouldBe("Bad Combination");
            }
        }

        public class ConditionalCustomRuleFor : AbstractValidatorExtensionsFixture
        {
            private readonly ConditionalCustomRuleForValidator _validator = new();

            [Fact]
            public void Should_Throw_When_Validator_Null()
            {
                var exception = Should.Throw<ArgumentNullException>(() =>
                {
                    AbstractValidatorExtensions.ConditionalCustomRuleFor<DummyModel, DummyModel>(null, model => model, _ => Create<bool>(), (_, _) => { });
                });
                exception.WithNamedMessageWhenNull("validator");
            }

            [Fact]
            public void Should_Throw_When_PropertyExpression_Null()
            {
                var exception = Should.Throw<ArgumentNullException>(() =>
                {
                    AbstractValidatorExtensions.ConditionalCustomRuleFor<DummyModel, DummyModel>(_validator, null, _ => Create<bool>(), (_, _) => { });
                });
                exception.WithNamedMessageWhenNull("propertyExpression");
            }

            [Fact]
            public void Should_Throw_When_Predicate_Null()
            {
                var exception = Should.Throw<ArgumentNullException>(() =>
                {
                    AbstractValidatorExtensions.ConditionalCustomRuleFor<DummyModel, DummyModel>(_validator, model => model, null, (_, _) => { });
                });
                exception.WithNamedMessageWhenNull("predicate");
            }

            [Fact]
            public void Should_Throw_When_Action_Null()
            {
                var exception = Should.Throw<ArgumentNullException>(() =>
                {
                    AbstractValidatorExtensions.ConditionalCustomRuleFor<DummyModel, DummyModel>(_validator, model => model, _ => Create<bool>(), null);
                });
                exception.WithNamedMessageWhenNull("action");
            }

            [Fact]
            public void Should_Not_Have_Error()
            {
                _model.ValueOne = GetWithinRange(1, 9);

                var errors = _validator.Validate(_model).Errors;

                errors.ShouldBeEmpty();
            }

            [Fact]
            public void Should_Validate_Single_Property()
            {
                _model.ValueOne = GetWithinRange(11, 100);

                var errors = _validator.Validate(_model).Errors;

                errors.Single().ErrorMessage.ShouldBe("Bad Value");
            }

            [Fact]
            public void Should_Validate_Two_Properties()
            {
                _model.ValueOne = GetWithinRange(1, 5);
                _model.ValueTwo = Guid.NewGuid();

                var errors = _validator.Validate(_model).Errors;

                errors.Single().ErrorMessage.ShouldBe("Bad Value");
            }
        }

        public class ConditionalCustomRuleForAsync : AbstractValidatorExtensionsFixture
        {
            private readonly ConditionalCustomRuleForAsyncValidator _validator = new();

            [Fact]
            public void Should_Throw_When_Validator_Null()
            {
                var exception = Should.Throw<ArgumentNullException>(() =>
                {
                    AbstractValidatorExtensions.ConditionalCustomRuleForAsync<DummyModel, DummyModel>(null, model => model, _ => Create<bool>(), (_, _, _) => Task.CompletedTask);
                });
                exception.WithNamedMessageWhenNull("validator");
            }

            [Fact]
            public void Should_Throw_When_PropertyExpression_Null()
            {
                var exception = Should.Throw<ArgumentNullException>(() =>
                {
                    AbstractValidatorExtensions.ConditionalCustomRuleForAsync<DummyModel, DummyModel>(_validator, null, _ => Create<bool>(), (_, _, _) => Task.CompletedTask);
                });
                exception.WithNamedMessageWhenNull("propertyExpression");
            }

            [Fact]
            public void Should_Throw_When_Predicate_Null()
            {
                var exception = Should.Throw<ArgumentNullException>(() =>
                {
                    AbstractValidatorExtensions.ConditionalCustomRuleForAsync<DummyModel, DummyModel>(_validator, model => model, null, (_, _, _) => Task.CompletedTask);
                });
                exception.WithNamedMessageWhenNull("predicate");
            }

            [Fact]
            public void Should_Throw_When_Action_Null()
            {
                var exception = Should.Throw<ArgumentNullException>(() =>
                {
                    AbstractValidatorExtensions.ConditionalCustomRuleForAsync<DummyModel, DummyModel>(_validator, model => model, _ => Create<bool>(), null);
                });
                exception.WithNamedMessageWhenNull("action");
            }

            [Fact]
            public async Task Should_Not_Have_Error()
            {
                _model.ValueOne = GetWithinRange(1, 9);

                var errors = (await _validator.ValidateAsync(_model)).Errors;

                errors.ShouldBeEmpty();
            }

            [Fact]
            public async Task Should_Validate_Single_Property()
            {
                _model.ValueOne = GetWithinRange(11, 100);

                var errors = (await _validator.ValidateAsync(_model)).Errors;

                errors.Single().ErrorMessage.ShouldBe("Bad Value");
            }

            [Fact]
            public async Task Should_Validate_Two_Properties()
            {
                _model.ValueOne = GetWithinRange(1, 5);
                _model.ValueTwo = Guid.NewGuid();

                var errors = (await _validator.ValidateAsync(_model)).Errors;

                errors.Single().ErrorMessage.ShouldBe("Bad Value");
            }
        }
    }
}





