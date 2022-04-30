using System.Threading.Tasks;
using AllOverIt.Fixture;
using AllOverIt.Validation.Extensions;
using FluentAssertions;
using FluentValidation;
using Xunit;

namespace AllOverIt.Validation.Tests.Extensions
{
    public class ValidatorExtensionsFixture : FixtureBase
    {
        private class DummyModel
        {
            public int? ValueOne { get; set; }
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
            }
        }

        public class ValidateAndThrow : ValidationInvokerFixture
        {
            [Fact]
            public void Should_Throw_When_Invoke_Validator()
            {

                Invoking(() =>
                    {
                        var model = new DummyModel();
                        var validator = new DummyModelValidator();

                        validator.ValidateAndThrow(model);
                    })
                    .Should()
                    .Throw<ValidationException>()
                    .WithMessage("Validation failed: " +
                                 " -- ValueOne: 'ValueOne' requires a valid value. Severity: Error");
            }

            [Fact]
            public void Should_Not_Throw_When_Invoke_Validator()
            {
                Invoking(() =>
                {
                    var context = Create<int>();

                    var model = new DummyModel
                    {
                        ValueOne = context
                    };

                    var validator = new DummyModelValidator();

                    validator.ValidateAndThrow(model);
                })
               .Should()
               .NotThrow();
            }
        }

        public class ValidateAndThrowAsync : ValidationInvokerFixture
        {
            [Fact]
            public async Task Should_Throw_When_Invoke_Validator()
            {
                await Invoking(async () =>
                    {
                        var model = new DummyModel();
                        var validator = new DummyModelValidator();

                        await validator.ValidateAndThrowAsync(model);
                    })
                    .Should()
                    .ThrowAsync<ValidationException>()
                    .WithMessage("Validation failed: " +
                                 " -- ValueOne: 'ValueOne' requires a valid value. Severity: Error");
            }

            [Fact]
            public async Task Should_Not_Throw_When_Invoke_Validator()
            {
                await Invoking(async () =>
                    {
                        var context = Create<int>();

                        var model = new DummyModel
                        {
                            ValueOne = context
                        };

                        var validator = new DummyModelValidator();

                        await validator.ValidateAndThrowAsync(model);
                    })
                    .Should()
                    .NotThrowAsync();
            }
        }
    }
}