using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Validation.Extensions;
using FluentAssertions;
using FluentValidation;
using System;
using Xunit;
using ValidationException = AllOverIt.Validation.Exceptions.ValidationException;

namespace AllOverIt.Validation.Tests.Exceptions
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
                RuleFor(model => model.ValueTwo).IsNotNullOrEmpty();
                RuleFor(model => model.ValueThree).IsNotNullOrEmpty();

                RuleFor(model => model.ValueFour)
                    .Custom((value, context) =>
                    {
                        if (context.RootContextData.ContainsKey("data"))
                        {
                            var comparison = context.GetContextData<DummyModel, bool>();

                            if (value != comparison)
                            {
                                context.AddFailure($"'{context.PropertyName}' has a value of {value} when expecting {comparison}");
                            }
                        }
                    });
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

        public class AssertValidation_Type : ValidationInvokerFixture
        {
            public AssertValidation_Type()
            {
                _validationInvoker.Register<DummyModel, DummyModelValidator>();
            }

            [Fact]
            public void Should_Throw_When_Instance_Null()
            {
                Invoking(() =>
                {
                    _validationInvoker.AssertValidation<DummyModel>(null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("instance");
            }

            [Fact]
            public void Should_Throw_When_Invoke_Validator()
            {
                var model = new DummyModel();

                Invoking(() =>
                {
                    _validationInvoker.AssertValidation(model);
                })
                .Should()
                .Throw<ValidationException>()
                .WithMessage("'ValueOne' requires a valid value., " +
                             "'ValueTwo' should not be empty., " +
                             "'ValueThree' should not be empty.");
            }

            [Fact]
            public void Should_Not_Throw_When_Invoke_Validator()
            {
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
            public AssertValidation_Type_Context()
            {
                _validationInvoker.Register<DummyModel, DummyModelValidator>();
            }

            [Fact]
            public void Should_Throw_When_Instance_Null()
            {
                Invoking(() =>
                {
                    _validationInvoker.AssertValidation<DummyModel, int>(null, Create<int>());
                })
               .Should()
               .Throw<ArgumentNullException>()
               .WithNamedMessageWhenNull("instance");
            }

            [Fact]
            public void Should_Throw_When_Invoke_Validator()
            {
                var model = new DummyModel();
                var context = Create<bool>();
                model.ValueFour = !context;

                Invoking(() =>
                {
                    _validationInvoker.AssertValidation(model, context);
                })
               .Should()
               .Throw<ValidationException>()
               .WithMessage("'ValueOne' requires a valid value., " +
                            "'ValueTwo' should not be empty., " +
                            "'ValueThree' should not be empty., " +
                           $"'ValueFour' has a value of {model.ValueFour} when expecting {context}");
            }

            [Fact]
            public void Should_Not_Throw_When_Invoke_Validator()
            {
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
    }
}
