using AllOverIt.Validation.Extensions;
using FluentAssertions;
using FluentValidation;

namespace AllOverIt.Validation.Tests.Validators
{
    public partial class ValidatorsFixture
    {
        public class IsLessThanOrEqualTo : ValidatorsFixture
        {
            private class ComparisonContext
            {
                public int CompareTo { get; set; }
            }

            private class DummyComparisonLessThanOrEqualToExplicitValidator : ValidatorBase<DummyDto>
            {
                public DummyComparisonLessThanOrEqualToExplicitValidator()
                {
                    // nullable and non-nullable, explicit comparison value
                    RuleFor(model => model.Value3).IsLessThanOrEqualTo(0);
                    RuleFor(model => model.Value4).IsLessThanOrEqualTo(0);
                }
            }

            private class DummyComparisonLessThanOrEqualToContextValidator : ValidatorBase<DummyDto>
            {
                public DummyComparisonLessThanOrEqualToContextValidator()
                {
                    // nullable and non-nullable, context provided comparison value
                    RuleFor(model => model.Value5).IsLessThanOrEqualTo<DummyDto, int, ComparisonContext>(ctx => ctx.CompareTo);
                    RuleFor(model => model.Value6).IsLessThanOrEqualTo<DummyDto, int, ComparisonContext>(ctx => ctx.CompareTo);
                }
            }

            [Fact]
            public void Should_Succeed_Validate_Explicit_Zero()
            {
                var model = new DummyDto
                {
                    Value3 = 0
                };

                var validator = new DummyComparisonLessThanOrEqualToExplicitValidator();

                var result = validator.Validate(model);

                result.IsValid.Should().BeTrue();
            }

            [Fact]
            public void Should_Succeed_Validate_Explicit_Random()
            {
                var model = new DummyDto
                {
                    Value3 = -CreateExcluding(0),
                    Value4 = -CreateExcluding(0)
                };

                var validator = new DummyComparisonLessThanOrEqualToExplicitValidator();

                var result = validator.Validate(model);

                result.IsValid.Should().BeTrue();
            }

            [Fact]
            public void Should_Fail_Validate_Explicit()
            {
                var model = new DummyDto
                {
                    Value3 = CreateExcluding(0),
                    Value4 = CreateExcluding(0)
                };

                var validator = new DummyComparisonLessThanOrEqualToExplicitValidator();

                var result = validator.Validate(model);

                result.IsValid.Should().BeFalse();

                var expected = new[]
                {
                    new
                    {
                        PropertyName = nameof(DummyDto.Value3),
                        ErrorCode = nameof(ValidationErrorCode.OutOfRange),
                        AttemptedValue = (object) model.Value3,
                        ErrorMessage = $"'{nameof(DummyDto.Value3)}' must be less than or equal to 0."
                    },
                    new
                    {
                        PropertyName = nameof(DummyDto.Value4),
                        ErrorCode = nameof(ValidationErrorCode.OutOfRange),
                        AttemptedValue = (object) model.Value4,
                        ErrorMessage = $"'{nameof(DummyDto.Value4)}' must be less than or equal to 0."
                    }
                };

                expected.Should().BeEquivalentTo(result.Errors, option => option.ExcludingMissingMembers());
            }

            [Fact]
            public void Should_Succeed_Validate_Context_On_Boundary()
            {
                var model = new DummyDto
                {
                    Value5 = Create<int>(),
                    Value6 = Create<int>()
                };

                var comparison = new ComparisonContext
                {
                    CompareTo = Math.Max(model.Value5.Value, model.Value6)
                };

                var validationContext = new ValidationContext<DummyDto>(model);
                validationContext.SetContextData(comparison);

                var validator = new DummyComparisonLessThanOrEqualToContextValidator();

                var result = validator.Validate(validationContext);

                result.IsValid.Should().BeTrue();
            }

            [Fact]
            public void Should_Succeed_Validate_Context_Off_Boundary()
            {
                var model = new DummyDto
                {
                    Value5 = Create<int>(),
                    Value6 = Create<int>()
                };

                var comparison = new ComparisonContext
                {
                    CompareTo = Math.Max(model.Value5.Value, model.Value6) + 1
                };

                var validationContext = new ValidationContext<DummyDto>(model);
                validationContext.SetContextData(comparison);

                var validator = new DummyComparisonLessThanOrEqualToContextValidator();

                var result = validator.Validate(validationContext);

                result.IsValid.Should().BeTrue();
            }

            [Fact]
            public void Should_Fail_Validate_Context()
            {
                var model = new DummyDto
                {
                    Value5 = Create<int>(),
                    Value6 = Create<int>()
                };

                var comparison = new ComparisonContext
                {
                    CompareTo = Math.Min(model.Value5.Value, model.Value6) - 1
                };

                var validationContext = new ValidationContext<DummyDto>(model);
                validationContext.SetContextData(comparison);

                var validator = new DummyComparisonLessThanOrEqualToContextValidator();

                var result = validator.Validate(validationContext);

                result.IsValid.Should().BeFalse();

                var expected = new[]
                {
                    new
                    {
                        PropertyName = nameof(DummyDto.Value5),
                        ErrorCode = nameof(ValidationErrorCode.OutOfRange),
                        AttemptedValue = (object) model.Value5,
                        ErrorMessage = $"'{nameof(DummyDto.Value5)}' must be less than or equal to {comparison.CompareTo}."
                    },
                    new
                    {
                        PropertyName = nameof(DummyDto.Value6),
                        ErrorCode = nameof(ValidationErrorCode.OutOfRange),
                        AttemptedValue = (object) model.Value6,
                        ErrorMessage = $"'{nameof(DummyDto.Value6)}' must be less than or equal to {comparison.CompareTo}."
                    }
                };

                expected.Should().BeEquivalentTo(result.Errors, option => option.ExcludingMissingMembers());
            }
        }
    }
}
