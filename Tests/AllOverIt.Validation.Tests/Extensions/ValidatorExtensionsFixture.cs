using AllOverIt.Fixture;
using AllOverIt.Validation.Extensions;
using FluentValidation;

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

                var exception = Should.Throw<ValidationException>(() => {
                        var model = new DummyModel();
                        var validator = new DummyModelValidator();

                        validator.ValidateAndThrow(model);});
                exception.Message.ShouldBe("Validation failed: " +
                                 " -- ValueOne: 'ValueOne' requires a valid value. Severity: Error");
            }

            [Fact]
            public void Should_Not_Throw_When_Invoke_Validator()
            {
                Should.NotThrow(() =>
                {
                    var context = Create<int>();

                    var model = new DummyModel
                    {
                        ValueOne = context
                    };

                    var validator = new DummyModelValidator();

                    validator.ValidateAndThrow(model);
                });
            }
        }

        public class ValidateAndThrowAsync : ValidationInvokerFixture
        {
            [Fact]
            public async Task Should_Throw_When_Invoke_Validator()
            {
                var exception = await Should.ThrowAsync<ValidationException>(async () => {
                        var model = new DummyModel();
                        var validator = new DummyModelValidator();

                        await validator.ValidateAndThrowAsync(model);});
                exception.Message.ShouldBe("Validation failed: " +
                                 " -- ValueOne: 'ValueOne' requires a valid value. Severity: Error");
            }

            [Fact]
            public async Task Should_Not_Throw_When_Invoke_Validator()
            {
                await Should.NotThrowAsync(async () =>
                {
                    var context = Create<int>();

                    var model = new DummyModel
                    {
                        ValueOne = context
                    };

                    var validator = new DummyModelValidator();

                    await validator.ValidateAndThrowAsync(model);
                });
            }
        }
    }
}





