using AllOverIt.Validation.Extensions;
using FluentAssertions;

namespace AllOverIt.Validation.Tests.Validators
{
    public partial class ValidatorsFixture
    {
        public class IsRequired : ValidatorsFixture
        {
            private class DummyIsRequiredValidator : ValidatorBase<DummyDto>
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
                var model = new DummyDto
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
                var model = new DummyDto();

                var validator = new DummyIsRequiredValidator();

                var result = validator.Validate(model);

                result.IsValid.Should().BeFalse();

                var expected = new[]
                {
                    new
                    {
                        PropertyName = nameof(DummyDto.Value1),
                        ErrorCode = nameof(ValidationErrorCode.Required),
                        AttemptedValue = (object) model.Value1,
                        ErrorMessage = $"'{nameof(DummyDto.Value1)}' requires a valid value."
                    },
                    new
                    {
                        PropertyName = nameof(DummyDto.Value2),
                        ErrorCode = nameof(ValidationErrorCode.Required),
                        AttemptedValue = (object) model.Value2,
                        ErrorMessage = $"'{nameof(DummyDto.Value2)}' requires a valid value."
                    },
                    new
                    {
                        PropertyName = nameof(DummyDto.Value3),
                        ErrorCode = nameof(ValidationErrorCode.Required),
                        AttemptedValue = (object) model.Value3,
                        ErrorMessage = $"'{nameof(DummyDto.Value3)}' requires a valid value."
                    },
                    new
                    {
                        PropertyName = nameof(DummyDto.Value7),
                        ErrorCode = nameof(ValidationErrorCode.Required),
                        AttemptedValue = (object) model.Value7,
                        ErrorMessage = $"'{nameof(DummyDto.Value7)}' requires a valid value."
                    }
                };

                expected.Should().BeEquivalentTo(result.Errors, option => option.ExcludingMissingMembers());
            }
        }
    }
}
