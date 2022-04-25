﻿using AllOverIt.Fixture;
using AllOverIt.Validation.Exceptions;
using AllOverIt.Validation.Extensions;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using System;
using Xunit;

namespace AllOverIt.Validation.Tests
{
    public class ValidationInvokerFixture : FixtureBase
    {
        private class DummyModel
        {
            public int? ValueOne { get; set; }
            public Guid ValueTwo { get; set; }
            public int ValueThree { get; set; }
            public bool ValueFour { get; set; }
        }

        private class DummyModelValidator : ValidatorBase<DummyModel>
        {
            static DummyModelValidator()
            {
                DisablePropertyNameSplitting();
            }

            public DummyModelValidator()
            {
                RuleFor(model => model.ValueOne).IsRequired();
                RuleFor(model => model.ValueTwo).IsNotEmpty();
                RuleFor(model => model.ValueThree).IsNotEmpty();

                RuleFor(model => model.ValueFour)
                    .Custom((value, context) =>
                    {
                        if (context.RootContextData.ContainsKey("data"))
                        {
                            var comparison = context.GetContextData<DummyModel, bool>();

                            if (value != comparison)
                            {
                                var failure = new ValidationFailure(
                                    context.PropertyName,
                                    $"'{context.PropertyName}' has a value of {value} when expecting {comparison}.",
                                    comparison
                                );

                                context.AddFailure(failure);
                            }
                        }
                    });
            }
        }


        private class DummyModelValidator2 : ValidatorBase<DummyModel>
        {
            public DummyModelValidator2(int dummy)
            {
            }
        }
        
        private readonly ValidationInvoker _validationInvoker;

        public ValidationInvokerFixture()
        {
            _validationInvoker = new ValidationInvoker();
        }

        public class Register : ValidationInvokerFixture
        {
            [Fact]
            public void Should_Throw_When_Validator_Not_Registered()
            {
                Invoking(() =>
                {
                    _validationInvoker.AssertValidation(Create<DummyModel>());
                })
                    .Should()
                    .Throw<InvalidOperationException>()
                    .WithMessage("The type 'DummyModel' does not have a registered validator.");
            }
        }

        public class Register_Strongly_Typed : ValidationInvokerFixture
        {
            [Fact]
            public void Should_Register_Validator()
            {
                _validationInvoker.Register<DummyModel, DummyModelValidator>();

                // registering a second time will fail
                Invoking(() =>
                    {
                        _validationInvoker.Register<DummyModel, DummyModelValidator>();
                    })
                   .Should()
                   .Throw<ArgumentException>()
                   .WithMessage($"An item with the same key has already been added. Key: {typeof(DummyModel).FullName}");
            }
        }

        public class Register_By_Type : ValidationInvokerFixture
        {
            [Fact]
            public void Should_Throw_When_Not_A_Validator()
            {
                Invoking(() =>
                    {
                        _validationInvoker.Register(typeof(DummyModel), typeof(DummyModel));
                    })
                   .Should()
                   .Throw<ValidationRegistryException>()
                   .WithMessage($"The {nameof(DummyModel)} type is not a validator.");
            }

            [Fact]
            public void Should_Throw_When_Cannot_Validate_Model_Type()
            {
                Invoking(() =>
                    {
                        _validationInvoker.Register(typeof(string), typeof(DummyModelValidator));
                    })
                   .Should()
                   .Throw<ValidationRegistryException>()
                   .WithMessage($"The {nameof(DummyModelValidator)} type cannot validate a System.String type.");
            }

            [Fact]
            public void Should_Throw_When_No_Default_Constructor()
            {
                Invoking(() =>
                {
                    _validationInvoker.Register(typeof(DummyModel), typeof(DummyModelValidator2));
                })
                   .Should()
                   .Throw<ValidationRegistryException>()
                   .WithMessage($"The {nameof(DummyModelValidator2)} type must have a default constructor.");
            }

            [Fact]
            public void Should_Register_Validator()
            {
                _validationInvoker.Register(typeof(DummyModel), typeof(DummyModelValidator));

                // registering a second time will fail
                Invoking(() =>
                    {
                        _validationInvoker.Register(typeof(DummyModel), typeof(DummyModelValidator));
                    })
                   .Should()
                   .Throw<ArgumentException>()
                   .WithMessage($"An item with the same key has already been added. Key: {typeof(DummyModel).FullName}");
            }
        }

        public class Validate_Type : ValidationInvokerFixture
        {
            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Throw_When_Invoke_Validator(bool useStrongTyping)
            {
                RegisterValidator(useStrongTyping);

                var model = new DummyModel();

                var result = _validationInvoker.Validate(model);

                result.IsValid.Should().BeFalse();

                var expected = new[]
                {
                    new
                    {
                        PropertyName = nameof(DummyModel.ValueOne),
                        ErrorCode = nameof(ValidationErrorCode.Required),
                        AttemptedValue = (object) model.ValueOne,
                        ErrorMessage = $"'{nameof(DummyModel.ValueOne)}' requires a valid value."
                    },
                    new
                    {
                        PropertyName = nameof(DummyModel.ValueTwo),
                        ErrorCode = nameof(ValidationErrorCode.NotEmpty),
                        AttemptedValue = (object) model.ValueTwo,
                        ErrorMessage = $"'{nameof(DummyModel.ValueTwo)}' should not be empty."
                    },
                    new
                    {
                        PropertyName = nameof(DummyModel.ValueThree),
                        ErrorCode = nameof(ValidationErrorCode.NotEmpty),
                        AttemptedValue = (object) model.ValueThree,
                        ErrorMessage = $"'{nameof(DummyModel.ValueThree)}' should not be empty."
                    }
                };

                expected.Should().BeEquivalentTo(result.Errors, options => options.ExcludingMissingMembers());
            }
        }

        public class Validate_Type_Context : ValidationInvokerFixture
        {
            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Throw_When_Invoke_Validator(bool useStrongTyping)
            {
                RegisterValidator(useStrongTyping);

                var model = Create<DummyModel>();
                var comparisonContext = !model.ValueFour;

                var result = _validationInvoker.Validate(model, comparisonContext);

                result.IsValid.Should().BeFalse();

                var expected = new[]
                {
                    new
                    {
                        PropertyName = nameof(DummyModel.ValueFour),
                        AttemptedValue = (object) comparisonContext,
                        ErrorMessage =
                            $"'ValueFour' has a value of {model.ValueFour} when expecting {comparisonContext}."
                    }
                };

                expected.Should().BeEquivalentTo(result.Errors, options => options.ExcludingMissingMembers());
            }
        }

        public class AssertValidation_Type : ValidationInvokerFixture
        {
            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Throw_When_Invoke_Validator(bool useStrongTyping)
            {
                RegisterValidator(useStrongTyping);

                var model = new DummyModel();

                Invoking(() =>
                {
                    _validationInvoker.AssertValidation(model);
                })
                .Should()
                .Throw<ValidationException>()
                .WithMessage($"Validation failed: {Environment.NewLine}" +
                             $" -- ValueOne: 'ValueOne' requires a valid value. Severity: Error{Environment.NewLine}" +
                             $" -- ValueTwo: 'ValueTwo' should not be empty. Severity: Error{Environment.NewLine}" +
                              " -- ValueThree: 'ValueThree' should not be empty. Severity: Error");
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Not_Throw_When_Invoke_Validator(bool useStrongTyping)
            {
                RegisterValidator(useStrongTyping);

                var model = Create<DummyModel>();

                Invoking(() =>
                {
                    _validationInvoker.AssertValidation(model);
                })
                .Should()
                .NotThrow();
            }
        }

        public class AssertValidation_Type_Context : ValidationInvokerFixture
        {
            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Throw_When_Invoke_Validator(bool useStrongTyping)
            {
                RegisterValidator(useStrongTyping);

                var model = new DummyModel();
                var context = Create<bool>();
                model.ValueFour = !context;

                Invoking(() =>
                {
                    _validationInvoker.AssertValidation(model, context);
                })
               .Should()
               .Throw<ValidationException>()
               .WithMessage($"Validation failed: {Environment.NewLine}" +
                            $" -- ValueOne: 'ValueOne' requires a valid value. Severity: Error{Environment.NewLine}" +
                            $" -- ValueTwo: 'ValueTwo' should not be empty. Severity: Error{Environment.NewLine}" +
                            $" -- ValueThree: 'ValueThree' should not be empty. Severity: Error{Environment.NewLine}" +
                            $" -- ValueFour: 'ValueFour' has a value of {model.ValueFour} when expecting {context}. Severity: Error");
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Not_Throw_When_Invoke_Validator(bool useStrongTyping)
            {
                RegisterValidator(useStrongTyping);

                var model = Create<DummyModel>();
                var context = Create<bool>();
                model.ValueFour = context;

                Invoking(() =>
                {
                    _validationInvoker.AssertValidation(model, context);
                })
               .Should()
               .NotThrow();
            }
        }

        private void RegisterValidator(bool useStrongTyping)
        {
            if (useStrongTyping)
            {
                _validationInvoker.Register<DummyModel, DummyModelValidator>();
            }
            else
            {
                _validationInvoker.Register(typeof(DummyModel), typeof(DummyModelValidator));
            }
        }
    }
}
