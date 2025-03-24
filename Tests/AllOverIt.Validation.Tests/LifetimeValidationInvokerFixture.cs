using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Validation.Exceptions;
using AllOverIt.Validation.Extensions;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;

namespace AllOverIt.Validation.Tests
{
    public class LifetimeValidationInvokerFixture : FixtureBase
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
                                    context.PropertyPath,
                                    $"'{context.PropertyPath}' has a value of {value} when expecting {comparison}.",
                                    comparison
                                );

                                context.AddFailure(failure);
                            }
                        }
                    });
            }
        }

        private sealed class DisposableDependency : IDisposable
        {
            private bool _disposed;

            internal Action Action { get; set; }

            public bool RuleWasInvoked { get; set; }

            public DisposableDependency()
            {
            }

            public async Task DoSomethingAsync()
            {
                if (_disposed)
                {
                    throw new InvalidOperationException("The async validation has not awaited because this dependency has already been disposed");
                }

                await Task.Delay(10);
            }

            public void Dispose()
            {
                Action?.Invoke();

                _disposed = true;
            }
        }

        private class DummyModelValidatorWithDependency : ValidatorBase<DummyModel>
        {
            public DummyModelValidatorWithDependency(DisposableDependency dependency)
            {
                // This is a specific test after fixing a bug where IDisposable dependencies would be disposed before the validation is invoked.
                var disposed = false;
                dependency.Action = () => { disposed = true; };

                this.CustomRuleFor(
                    model => model.ValueOne,
                    (value, context) =>
                    {
                        disposed.Should().BeFalse();
                        dependency.RuleWasInvoked = true;
                    });
            }
        }

        private class DummyModeAsyncValidatorWithDependency : ValidatorBase<DummyModel>
        {
            public DummyModeAsyncValidatorWithDependency(DisposableDependency dependency)
            {
                // This is a specific test after fixing a bug where IDisposable dependencies would be disposed before the validation is invoked.
                var disposed = false;
                dependency.Action = () => { disposed = true; };

                this.CustomRuleForAsync(
                    model => model.ValueOne,
                    async (value, context, token) =>
                    {
                        disposed.Should().BeFalse();

                        await dependency.DoSomethingAsync();

                        disposed.Should().BeFalse();
                        dependency.RuleWasInvoked = true;
                    });
            }
        }

        private readonly IServiceCollection _services;
        private readonly LifetimeValidationInvoker _validationInvoker;
        private readonly ILifetimeValidationRegistry _validationRegistry;

        public LifetimeValidationInvokerFixture()
        {
            _services = new ServiceCollection();
            _validationInvoker = new LifetimeValidationInvoker(_services);
            _validationRegistry = _validationInvoker;
        }

        public class ServiceProvider : LifetimeValidationInvokerFixture
        {
            [Fact]
            public void Should_Throw_When_ServiceProvider_Not_Set()
            {
                RegisterDummyModelValidator(Create<bool>(), Create<ServiceLifetime>(), false);

                var model = Create<DummyModel>();

                Invoking(() =>
                {
                    _validationInvoker.AssertValidation(model);
                })
                .Should()
                .Throw<InvalidOperationException>()
                .WithMessage("The scope factory has not been set.");
            }
        }

        public class ContainsModelRegistration_Strongly_Type : LifetimeValidationInvokerFixture
        {
            [Fact]
            public void Should_Return_True_For_Registered_Model()
            {
                RegisterDummyModelValidator(true, Create<ServiceLifetime>());

                _validationRegistry.ContainsModelRegistration<DummyModel>()
                    .Should()
                    .BeTrue();
            }

            [Fact]
            public void Should_Return_False_For_Non_Registered_Model()
            {
                _validationRegistry.ContainsModelRegistration<DummyModel>()
                    .Should()
                    .BeFalse();
            }
        }

        public class ContainsModelRegistration_Type : LifetimeValidationInvokerFixture
        {
            [Fact]
            public void Should_Return_True_For_Registered_Model()
            {
                RegisterDummyModelValidator(false, Create<ServiceLifetime>());

                _validationRegistry.ContainsModelRegistration(typeof(DummyModel))
                    .Should()
                    .BeTrue();
            }

            [Fact]
            public void Should_Return_False_For_Non_Registered_Model()
            {
                _validationRegistry.ContainsModelRegistration(typeof(DummyModel))
                    .Should()
                    .BeFalse();
            }
        }

        public class RegisterTransient_Strongly_Typed : LifetimeValidationInvokerFixture
        {
            [Fact]
            public void Should_Throw_When_Model_Already_Registered()
            {
                _validationRegistry.RegisterTransient<DummyModel, DummyModelValidator>();

                Invoking(() =>
                {
                    _validationRegistry.RegisterTransient<DummyModel, DummyModelValidator>();
                })
                 .Should()
                 .Throw<ValidationRegistryException>()
                 .WithMessage($"The type '{typeof(DummyModel).GetFriendlyName()}' already has a registered validator.");
            }

            [Fact]
            public void Should_Register_Validator()
            {
                _validationRegistry.RegisterTransient<DummyModel, DummyModelValidator>();

                BuildServiceProvider();

                AssertValidatorRegistration<DummyModel, DummyModelValidator>(ServiceLifetime.Transient);
            }
        }

        public class RegisterScoped_Strongly_Typed : LifetimeValidationInvokerFixture
        {
            [Fact]
            public void Should_Throw_When_Model_Already_Registered()
            {
                _validationRegistry.RegisterScoped<DummyModel, DummyModelValidator>();

                Invoking(() =>
                {
                    _validationRegistry.RegisterScoped<DummyModel, DummyModelValidator>();
                })
                 .Should()
                 .Throw<ValidationRegistryException>()
                 .WithMessage($"The type '{typeof(DummyModel).GetFriendlyName()}' already has a registered validator.");
            }

            [Fact]
            public void Should_Register_Validator()
            {
                _validationRegistry.RegisterScoped<DummyModel, DummyModelValidator>();

                BuildServiceProvider();

                AssertValidatorRegistration<DummyModel, DummyModelValidator>(ServiceLifetime.Scoped);
            }
        }

        public class RegisterSingleton_Strongly_Typed : LifetimeValidationInvokerFixture
        {
            [Fact]
            public void Should_Throw_When_Model_Already_Registered()
            {
                _validationRegistry.RegisterSingleton<DummyModel, DummyModelValidator>();

                Invoking(() =>
                {
                    _validationRegistry.RegisterSingleton<DummyModel, DummyModelValidator>();
                })
                 .Should()
                 .Throw<ValidationRegistryException>()
                 .WithMessage($"The type '{typeof(DummyModel).GetFriendlyName()}' already has a registered validator.");
            }

            [Fact]
            public void Should_Register_Validator()
            {
                _validationRegistry.RegisterSingleton<DummyModel, DummyModelValidator>();

                BuildServiceProvider();

                AssertValidatorRegistration<DummyModel, DummyModelValidator>(ServiceLifetime.Singleton);
            }
        }

        public class Register_Strongly_Typed : LifetimeValidationInvokerFixture
        {
            [Fact]
            public void Should_Throw_When_Model_Already_Registered()
            {
                _validationRegistry.Register<DummyModel, DummyModelValidator>(Create<ServiceLifetime>());

                Invoking(() =>
                {
                    _validationRegistry.Register<DummyModel, DummyModelValidator>(Create<ServiceLifetime>());
                })
                 .Should()
                 .Throw<ValidationRegistryException>()
                 .WithMessage($"The type '{typeof(DummyModel).GetFriendlyName()}' already has a registered validator.");
            }

            [Fact]
            public void Should_Register_Validator()
            {
                var lifetime = Create<ServiceLifetime>();

                _validationRegistry.Register<DummyModel, DummyModelValidator>(lifetime);

                BuildServiceProvider();

                AssertValidatorRegistration<DummyModel, DummyModelValidator>(lifetime);
            }
        }

        public class RegisterTransient_Type : LifetimeValidationInvokerFixture
        {
            [Fact]
            public void Should_Throw_When_Not_A_Validator()
            {
                Invoking(() =>
                {
                    _validationRegistry.RegisterTransient(typeof(DummyModel), typeof(DummyModel));
                })
                   .Should()
                   .Throw<ValidationRegistryException>()
                   .WithMessage($"The type '{nameof(DummyModel)}' is not a validator.");
            }

            [Fact]
            public void Should_Throw_When_Cannot_Validate_Model_Type()
            {
                Invoking(() =>
                {
                    _validationRegistry.RegisterTransient(typeof(string), typeof(DummyModelValidator));
                })
                   .Should()
                   .Throw<ValidationRegistryException>()
                   .WithMessage($"The type '{nameof(DummyModelValidator)}' cannot validate a String type.");
            }

            [Fact]
            public void Should_Throw_When_Model_Already_Registered()
            {
                _validationRegistry.RegisterTransient(typeof(DummyModel), typeof(DummyModelValidator));

                Invoking(() =>
                {
                    _validationRegistry.RegisterTransient(typeof(DummyModel), typeof(DummyModelValidator));
                })
                 .Should()
                 .Throw<ValidationRegistryException>()
                 .WithMessage($"The type '{typeof(DummyModel).GetFriendlyName()}' already has a registered validator.");
            }

            [Fact]
            public void Should_Register_Validator()
            {
                _validationRegistry.RegisterTransient(typeof(DummyModel), typeof(DummyModelValidator));

                BuildServiceProvider();

                AssertValidatorRegistration<DummyModel, DummyModelValidator>(ServiceLifetime.Transient);
            }
        }

        public class RegisterScoped_Type : LifetimeValidationInvokerFixture
        {
            [Fact]
            public void Should_Throw_When_Not_A_Validator()
            {
                Invoking(() =>
                {
                    _validationRegistry.RegisterScoped(typeof(DummyModel), typeof(DummyModel));
                })
                   .Should()
                   .Throw<ValidationRegistryException>()
                   .WithMessage($"The type '{nameof(DummyModel)}' is not a validator.");
            }

            [Fact]
            public void Should_Throw_When_Cannot_Validate_Model_Type()
            {
                Invoking(() =>
                {
                    _validationRegistry.RegisterScoped(typeof(string), typeof(DummyModelValidator));
                })
                   .Should()
                   .Throw<ValidationRegistryException>()
                   .WithMessage($"The type '{nameof(DummyModelValidator)}' cannot validate a String type.");
            }

            [Fact]
            public void Should_Throw_When_Model_Already_Registered()
            {
                _validationRegistry.RegisterScoped(typeof(DummyModel), typeof(DummyModelValidator));

                Invoking(() =>
                {
                    _validationRegistry.RegisterScoped(typeof(DummyModel), typeof(DummyModelValidator));
                })
                 .Should()
                 .Throw<ValidationRegistryException>()
                 .WithMessage($"The type '{typeof(DummyModel).GetFriendlyName()}' already has a registered validator.");
            }

            [Fact]
            public void Should_Register_Validator()
            {
                _validationRegistry.RegisterScoped(typeof(DummyModel), typeof(DummyModelValidator));

                BuildServiceProvider();

                AssertValidatorRegistration<DummyModel, DummyModelValidator>(ServiceLifetime.Scoped);
            }
        }

        public class RegisterSingleton_Type : LifetimeValidationInvokerFixture
        {
            [Fact]
            public void Should_Throw_When_Not_A_Validator()
            {
                Invoking(() =>
                {
                    _validationRegistry.RegisterSingleton(typeof(DummyModel), typeof(DummyModel));
                })
                   .Should()
                   .Throw<ValidationRegistryException>()
                   .WithMessage($"The type '{nameof(DummyModel)}' is not a validator.");
            }

            [Fact]
            public void Should_Throw_When_Cannot_Validate_Model_Type()
            {
                Invoking(() =>
                {
                    _validationRegistry.RegisterSingleton(typeof(string), typeof(DummyModelValidator));
                })
                   .Should()
                   .Throw<ValidationRegistryException>()
                   .WithMessage($"The type '{nameof(DummyModelValidator)}' cannot validate a String type.");
            }

            [Fact]
            public void Should_Throw_When_Model_Already_Registered()
            {
                _validationRegistry.RegisterSingleton(typeof(DummyModel), typeof(DummyModelValidator));

                Invoking(() =>
                {
                    _validationRegistry.RegisterSingleton(typeof(DummyModel), typeof(DummyModelValidator));
                })
                 .Should()
                 .Throw<ValidationRegistryException>()
                 .WithMessage($"The type '{typeof(DummyModel).GetFriendlyName()}' already has a registered validator.");
            }

            [Fact]
            public void Should_Register_Validator()
            {
                _validationRegistry.RegisterSingleton(typeof(DummyModel), typeof(DummyModelValidator));

                BuildServiceProvider();

                AssertValidatorRegistration<DummyModel, DummyModelValidator>(ServiceLifetime.Singleton);
            }
        }

        public class Register_Type : LifetimeValidationInvokerFixture
        {
            [Fact]
            public void Should_Throw_When_Not_A_Validator()
            {
                Invoking(() =>
                {
                    _validationRegistry.Register(typeof(DummyModel), typeof(DummyModel), Create<ServiceLifetime>());
                })
                   .Should()
                   .Throw<ValidationRegistryException>()
                   .WithMessage($"The type '{nameof(DummyModel)}' is not a validator.");
            }

            [Fact]
            public void Should_Throw_When_Cannot_Validate_Model_Type()
            {
                Invoking(() =>
                {
                    _validationRegistry.Register(typeof(string), typeof(DummyModelValidator), Create<ServiceLifetime>());
                })
                   .Should()
                   .Throw<ValidationRegistryException>()
                   .WithMessage($"The type '{nameof(DummyModelValidator)}' cannot validate a String type.");
            }

            [Fact]
            public void Should_Throw_When_Model_Already_Registered()
            {
                _validationRegistry.Register(typeof(DummyModel), typeof(DummyModelValidator), Create<ServiceLifetime>());

                Invoking(() =>
                {
                    _validationRegistry.Register(typeof(DummyModel), typeof(DummyModelValidator), Create<ServiceLifetime>());
                })
                 .Should()
                 .Throw<ValidationRegistryException>()
                 .WithMessage($"The type '{typeof(DummyModel).GetFriendlyName()}' already has a registered validator.");
            }

            [Fact]
            public void Should_Register_Validator()
            {
                var lifetime = Create<ServiceLifetime>();

                _validationRegistry.Register(typeof(DummyModel), typeof(DummyModelValidator), lifetime);

                BuildServiceProvider();

                AssertValidatorRegistration<DummyModel, DummyModelValidator>(lifetime);
            }
        }

        public class Validate_Type : LifetimeValidationInvokerFixture
        {
            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Throw_When_Invoke_Validator(bool useStrongTyping)
            {
                RegisterDummyModelValidator(useStrongTyping, Create<ServiceLifetime>());

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

                expected.Should().BeEquivalentTo(result.Errors, option => option.ExcludingMissingMembers());
            }

            [Fact]
            public void Should_Dispose_Dependencies_After_Validation()
            {
                var dependency = new DisposableDependency();
                _services.AddScoped(provider => dependency);
                _validationRegistry.RegisterScoped(typeof(DummyModel), typeof(DummyModelValidatorWithDependency));
                BuildServiceProvider();

                var model = new DummyModel();

                dependency.RuleWasInvoked.Should().BeFalse();

                // The validator contains the actual assertion
                _validationInvoker.Validate(model);

                dependency.RuleWasInvoked.Should().BeTrue();
            }
        }

        public class ValidateAsync_Type : LifetimeValidationInvokerFixture
        {
            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public async Task Should_Throw_When_Invoke_Validator(bool useStrongTyping)
            {
                RegisterDummyModelValidator(useStrongTyping, Create<ServiceLifetime>());

                var model = new DummyModel();

                var result = await _validationInvoker.ValidateAsync(model);

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

                expected.Should().BeEquivalentTo(result.Errors, option => option.ExcludingMissingMembers());
            }

            [Fact]
            public async Task Should_Dispose_Dependencies_After_Validation()
            {
                var dependency = new DisposableDependency();
                _services.AddScoped(provider => dependency);
                _validationRegistry.RegisterScoped(typeof(DummyModel), typeof(DummyModeAsyncValidatorWithDependency));
                BuildServiceProvider();

                var model = new DummyModel();

                dependency.RuleWasInvoked.Should().BeFalse();

                // The validator contains the actual assertion
                await _validationInvoker.ValidateAsync(model);

                dependency.RuleWasInvoked.Should().BeTrue();
            }
        }

        public class Validate_Type_Context : LifetimeValidationInvokerFixture
        {
            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Throw_When_Invoke_Validator(bool useStrongTyping)
            {
                RegisterDummyModelValidator(useStrongTyping, Create<ServiceLifetime>());

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

                expected.Should().BeEquivalentTo(result.Errors, option => option.ExcludingMissingMembers());
            }

            [Fact]
            public void Should_Dispose_Dependencies_After_Validation()
            {
                var dependency = new DisposableDependency();
                _services.AddScoped(provider => dependency);
                _validationRegistry.RegisterScoped(typeof(DummyModel), typeof(DummyModelValidatorWithDependency));
                BuildServiceProvider();

                var model = new DummyModel();
                var comparisonContext = !model.ValueFour;

                dependency.RuleWasInvoked.Should().BeFalse();

                // The validator contains the actual assertion
                _validationInvoker.Validate(model, comparisonContext);

                dependency.RuleWasInvoked.Should().BeTrue();
            }
        }

        public class ValidateAsync_Type_Context : LifetimeValidationInvokerFixture
        {
            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public async Task Should_Throw_When_Invoke_Validator(bool useStrongTyping)
            {
                RegisterDummyModelValidator(useStrongTyping, Create<ServiceLifetime>());

                var model = Create<DummyModel>();
                var comparisonContext = !model.ValueFour;

                var result = await _validationInvoker.ValidateAsync(model, comparisonContext);

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

                expected.Should().BeEquivalentTo(result.Errors, option => option.ExcludingMissingMembers());
            }

            [Fact]
            public async Task Should_Dispose_Dependencies_After_Validation()
            {
                var dependency = new DisposableDependency();
                _services.AddScoped(provider => dependency);
                _validationRegistry.RegisterScoped(typeof(DummyModel), typeof(DummyModeAsyncValidatorWithDependency));
                BuildServiceProvider();

                var model = new DummyModel();
                var comparisonContext = !model.ValueFour;

                dependency.RuleWasInvoked.Should().BeFalse();

                // The validator contains the actual assertion
                await _validationInvoker.ValidateAsync(model, comparisonContext);

                dependency.RuleWasInvoked.Should().BeTrue();
            }
        }

        public class AssertValidation_Type : LifetimeValidationInvokerFixture
        {
            [Fact]
            public void Should_Throw_When_Validator_Not_Registered()
            {
                BuildServiceProvider();

                Invoking(() =>
                {
                    _validationInvoker.AssertValidation(Create<DummyModel>());
                })
                    .Should()
                    .Throw<InvalidOperationException>()
                    .WithMessage("The type 'DummyModel' does not have a registered validator.");
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Throw_When_Invoke_Validator(bool useStrongTyping)
            {
                RegisterDummyModelValidator(useStrongTyping, Create<ServiceLifetime>());

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
                RegisterDummyModelValidator(useStrongTyping, Create<ServiceLifetime>());

                var model = Create<DummyModel>();

                Invoking(() =>
                {
                    _validationInvoker.AssertValidation(model);
                })
                .Should()
                .NotThrow();
            }

            [Fact]
            public void Should_Dispose_Dependencies_After_Validation()
            {
                var dependency = new DisposableDependency();
                _services.AddScoped(provider => dependency);
                _validationRegistry.RegisterScoped(typeof(DummyModel), typeof(DummyModelValidatorWithDependency));
                BuildServiceProvider();

                var model = new DummyModel();
                var comparisonContext = !model.ValueFour;

                dependency.RuleWasInvoked.Should().BeFalse();

                // The validator contains the actual assertion
                _validationInvoker.AssertValidation(model);

                dependency.RuleWasInvoked.Should().BeTrue();
            }
        }

        public class AssertValidationAsync_Type : LifetimeValidationInvokerFixture
        {
            [Fact]
            public async Task Should_Throw_When_Validator_Not_Registered()
            {
                BuildServiceProvider();

                await Invoking(async () =>
                {
                    await _validationInvoker.AssertValidationAsync(Create<DummyModel>());
                })
                    .Should()
                    .ThrowAsync<InvalidOperationException>()
                    .WithMessage("The type 'DummyModel' does not have a registered validator.");
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public async Task Should_Throw_When_Invoke_Validator(bool useStrongTyping)
            {
                RegisterDummyModelValidator(useStrongTyping, Create<ServiceLifetime>());

                var model = new DummyModel();

                await Invoking(async () =>
                {
                    await _validationInvoker.AssertValidationAsync(model);
                })
                    .Should()
                    .ThrowAsync<ValidationException>()
                    .WithMessage($"Validation failed: {Environment.NewLine}" +
                                 $" -- ValueOne: 'ValueOne' requires a valid value. Severity: Error{Environment.NewLine}" +
                                 $" -- ValueTwo: 'ValueTwo' should not be empty. Severity: Error{Environment.NewLine}" +
                                 " -- ValueThree: 'ValueThree' should not be empty. Severity: Error");
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public async Task Should_Not_Throw_When_Invoke_Validator(bool useStrongTyping)
            {
                RegisterDummyModelValidator(useStrongTyping, Create<ServiceLifetime>());

                var model = Create<DummyModel>();

                await Invoking(async () =>
                {
                    await _validationInvoker.AssertValidationAsync(model);
                })
                    .Should()
                    .NotThrowAsync();
            }

            [Fact]
            public async Task Should_Dispose_Dependencies_After_Validation()
            {
                var dependency = new DisposableDependency();
                _services.AddScoped(provider => dependency);
                _validationRegistry.RegisterScoped(typeof(DummyModel), typeof(DummyModeAsyncValidatorWithDependency));
                BuildServiceProvider();

                var model = new DummyModel();
                var comparisonContext = !model.ValueFour;

                dependency.RuleWasInvoked.Should().BeFalse();

                // The validator contains the actual assertion
                await _validationInvoker.AssertValidationAsync(model);

                dependency.RuleWasInvoked.Should().BeTrue();
            }
        }

        public class AssertValidation_Type_Context : LifetimeValidationInvokerFixture
        {
            [Fact]
            public void Should_Throw_When_Validator_Not_Registered()
            {
                BuildServiceProvider();

                Invoking(() =>
                {
                    _validationInvoker.AssertValidation(Create<DummyModel>(), Create<bool>());
                })
                    .Should()
                    .Throw<InvalidOperationException>()
                    .WithMessage("The type 'DummyModel' does not have a registered validator.");
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Throw_When_Invoke_Validator(bool useStrongTyping)
            {
                RegisterDummyModelValidator(useStrongTyping, Create<ServiceLifetime>());

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
                RegisterDummyModelValidator(useStrongTyping, Create<ServiceLifetime>());

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

            [Fact]
            public void Should_Dispose_Dependencies_After_Validation()
            {
                var dependency = new DisposableDependency();
                _services.AddScoped(provider => dependency);
                _validationRegistry.RegisterScoped(typeof(DummyModel), typeof(DummyModelValidatorWithDependency));
                BuildServiceProvider();

                var model = new DummyModel();
                var comparisonContext = !model.ValueFour;

                dependency.RuleWasInvoked.Should().BeFalse();

                // The validator contains the actual assertion
                _validationInvoker.AssertValidation(model, comparisonContext);

                dependency.RuleWasInvoked.Should().BeTrue();
            }
        }

        public class AssertValidationAsync_Type_Context : LifetimeValidationInvokerFixture
        {
            [Fact]
            public async Task Should_Throw_When_Validator_Not_Registered()
            {
                BuildServiceProvider();

                await Invoking(async () =>
                {
                    await _validationInvoker.AssertValidationAsync(Create<DummyModel>(), Create<bool>());
                })
                    .Should()
                    .ThrowAsync<InvalidOperationException>()
                    .WithMessage("The type 'DummyModel' does not have a registered validator.");
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public async Task Should_Throw_When_Invoke_Validator(bool useStrongTyping)
            {
                RegisterDummyModelValidator(useStrongTyping, Create<ServiceLifetime>());

                var model = new DummyModel();
                var context = Create<bool>();
                model.ValueFour = !context;

                await Invoking(async () =>
                {
                    await _validationInvoker.AssertValidationAsync(model, context);
                })
                    .Should()
                    .ThrowAsync<ValidationException>()
                    .WithMessage($"Validation failed: {Environment.NewLine}" +
                                 $" -- ValueOne: 'ValueOne' requires a valid value. Severity: Error{Environment.NewLine}" +
                                 $" -- ValueTwo: 'ValueTwo' should not be empty. Severity: Error{Environment.NewLine}" +
                                 $" -- ValueThree: 'ValueThree' should not be empty. Severity: Error{Environment.NewLine}" +
                                 $" -- ValueFour: 'ValueFour' has a value of {model.ValueFour} when expecting {context}. Severity: Error");
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public async Task Should_Not_Throw_When_Invoke_Validator(bool useStrongTyping)
            {
                RegisterDummyModelValidator(useStrongTyping, Create<ServiceLifetime>());

                var model = Create<DummyModel>();
                var context = Create<bool>();
                model.ValueFour = context;

                await Invoking(async () =>
                {
                    await _validationInvoker.AssertValidationAsync(model, context);
                })
                    .Should()
                    .NotThrowAsync();
            }

            [Fact]
            public async Task Should_Dispose_Dependencies_After_Validation()
            {
                var dependency = new DisposableDependency();
                _services.AddScoped(provider => dependency);
                _validationRegistry.RegisterScoped(typeof(DummyModel), typeof(DummyModeAsyncValidatorWithDependency));
                BuildServiceProvider();

                var model = new DummyModel();
                var comparisonContext = !model.ValueFour;

                dependency.RuleWasInvoked.Should().BeFalse();

                // The validator contains the actual assertion
                await _validationInvoker.AssertValidationAsync(model, comparisonContext);

                dependency.RuleWasInvoked.Should().BeTrue();
            }
        }

        private void RegisterDummyModelValidator(bool useStrongTyping, ServiceLifetime lifetime, bool setProvider = true)
        {
            if (useStrongTyping)
            {
                _validationRegistry.Register<DummyModel, DummyModelValidator>(lifetime);
            }
            else
            {
                _validationRegistry.Register(typeof(DummyModel), typeof(DummyModelValidator), lifetime);
            }

            if (setProvider)
            {
                BuildServiceProvider();
            }
        }

        private void BuildServiceProvider()
        {
            var serviceProvider = _services.BuildServiceProvider();
            var scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

            _validationInvoker.SetScopeFactory(scopeFactory);
        }

        private void AssertValidatorRegistration<TModel, TValidator>(ServiceLifetime lifetime)
        {
            var descriptor = _services
                .Where(descriptor => descriptor.ServiceType == LifetimeValidationInvoker.CreateModelValidatorKey(typeof(TModel)))
                .SingleOrDefault();

            descriptor.Lifetime.Should().Be(lifetime);

            var provider = _services.BuildServiceProvider();

            descriptor.ImplementationFactory
                .Invoke(provider)
                .Should()
                .BeOfType(typeof(TValidator));
        }
    }
}
