using AllOverIt.Validation.Extensions;
using FluentAssertions;
using System;

namespace AllOverIt.Validation.Tests.Validators
{
    public partial class ValidatorsFixture
    {
        public class IsRequired : ValidatorsFixture
        {
            private class DummyIsRequiredValidator : ValidatorBase<DummyComparisonModel>
            {
                public DummyIsRequiredValidator()
                {
                    RuleFor(model => model.Value1).IsRequired();    // string
                    RuleFor(model => model.Value2).IsRequired();    // Guid
                    RuleFor(model => model.Value3).IsRequired();    // nullable
                    RuleFor(model => model.Value7).IsRequired();    // IReadOnlyList<int>
                }
            }

            [Fact]
            public void Should_Succeed_Validate()
            {
                var model = new DummyComparisonModel
                {
                    Value1 = Create<string>(),
                    Value2 = Guid.NewGuid(),
                    Value3 = Create<int>(),
                    Value7 = CreateMany<int>()
                };

                var validator = new DummyIsRequiredValidator();

                var result = validator.Validate(model);

                result.IsValid.Should().BeTrue();
            }

            [Fact]
            public void Should_Fail_Validate()
            {
                var model = new DummyComparisonModel();

                var validator = new DummyIsRequiredValidator();

                var result = validator.Validate(model);

                result.IsValid.Should().BeFalse();

                var expected = new[]
                {
                    new
                    {
                        PropertyName = nameof(DummyComparisonModel.Value1),
                        ErrorCode = nameof(ValidationErrorCode.Required),
                        AttemptedValue = (object) model.Value1,
                        ErrorMessage = $"'{nameof(DummyComparisonModel.Value1)}' requires a valid value."
                    },
                    new
                    {
                        PropertyName = nameof(DummyComparisonModel.Value2),
                        ErrorCode = nameof(ValidationErrorCode.Required),
                        AttemptedValue = (object) model.Value2,
                        ErrorMessage = $"'{nameof(DummyComparisonModel.Value2)}' requires a valid value."
                    },
                    new
                    {
                        PropertyName = nameof(DummyComparisonModel.Value3),
                        ErrorCode = nameof(ValidationErrorCode.Required),
                        AttemptedValue = (object) model.Value3,
                        ErrorMessage = $"'{nameof(DummyComparisonModel.Value3)}' requires a valid value."
                    },
                    new
                    {
                        PropertyName = nameof(DummyComparisonModel.Value7),
                        ErrorCode = nameof(ValidationErrorCode.Required),
                        AttemptedValue = (object) model.Value7,
                        ErrorMessage = $"'{nameof(DummyComparisonModel.Value7)}' requires a valid value."
                    }
                };

                expected.Should().BeEquivalentTo(result.Errors, option => option.ExcludingMissingMembers());
            }
        }
    }
}
