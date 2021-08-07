using AllOverIt.Validation.Extensions;
using FluentAssertions;
using FluentValidation;
using Xunit;

namespace AllOverIt.Validation.Tests.Validators
{
    public partial class ValidatorsFixture
    {
        public class IsExclusiveBetween : ValidatorsFixture
        {
            private class ComparisonContext
            {
                public int CompareFrom { get; set; }
                public int CompareTo { get; set; }
            }

            private class DummyComparisonExclusiveBetweenExplicitValidator : ValidatorBase<DummyComparisonModel>
            {
                public DummyComparisonExclusiveBetweenExplicitValidator()
                {
                    // nullable and non-nullable, explicit comparison value
                    RuleFor(model => model.Value3).IsExclusiveBetween(0, 10);
                    RuleFor(model => model.Value4).IsExclusiveBetween(0, 10);
                    RuleFor(model => model.Value5).IsExclusiveBetween(0, 10);
                }
            }

            private class DummyComparisonExclusiveBetweenContextValidator : ValidatorBase<DummyComparisonModel>
            {
                public DummyComparisonExclusiveBetweenContextValidator()
                {
                    // nullable and non-nullable, context provided comparison value
                    RuleFor(model => model.Value4).IsExclusiveBetween<DummyComparisonModel, int, ComparisonContext>(ctx => ctx.CompareFrom, ctx => ctx.CompareTo);
                    RuleFor(model => model.Value5).IsExclusiveBetween<DummyComparisonModel, int, ComparisonContext>(ctx => ctx.CompareFrom, ctx => ctx.CompareTo);
                    RuleFor(model => model.Value6).IsExclusiveBetween<DummyComparisonModel, int, ComparisonContext>(ctx => ctx.CompareFrom, ctx => ctx.CompareTo);
                }
            }

            [Fact]
            public void Should_Succeed_Validate_Explicit()
            {
                var model = new DummyComparisonModel
                {
                    Value3 = GetWithinRange(1, 9),
                    Value4 = GetWithinRange(1, 9),
                    Value5 = GetWithinRange(1, 9)
                };

                var validator = new DummyComparisonExclusiveBetweenExplicitValidator();

                var result = validator.Validate(model);

                result.IsValid.Should().BeTrue();
            }

            [Fact]
            public void Should_Fail_Validate_Explicit()
            {
                var model = new DummyComparisonModel
                {
                    Value3 = Create<bool>() ? 0 : 10,
                    Value4 = Create<bool>() ? 0 : 10,
                    Value5 = GetWithinRange(1, 9)           // not out of range
                };

                var validator = new DummyComparisonExclusiveBetweenExplicitValidator();

                var result = validator.Validate(model);

                result.IsValid.Should().BeFalse();

                result.Errors.Should().BeEquivalentTo(new[]
                {
                    new
                    {
                        PropertyName = nameof(DummyComparisonModel.Value3),
                        ErrorCode = nameof(ValidationErrorCode.OutOfRange),
                        AttemptedValue = (object)model.Value3,
                        ErrorMessage = $"'{nameof(DummyComparisonModel.Value3)}' must be between 0 and 10 (exclusive)."
                    },
                    new
                    {
                        PropertyName = nameof(DummyComparisonModel.Value4),
                        ErrorCode = nameof(ValidationErrorCode.OutOfRange),
                        AttemptedValue = (object)model.Value4,
                        ErrorMessage = $"'{nameof(DummyComparisonModel.Value4)}' must be between 0 and 10 (exclusive)."
                    }
                }, options => options.ExcludingMissingMembers());
            }

            [Fact]
            public void Should_Succeed_Validate_Context()
            {
                var model = new DummyComparisonModel
                {
                    Value4 = 1,
                    Value5 = GetWithinRange(1, 1000),
                    Value6 = 1000
                };

                var comparison = new ComparisonContext
                {
                    CompareFrom = 0,
                    CompareTo = 1001
                };

                var validationContext = new ValidationContext<DummyComparisonModel>(model);
                validationContext.SetContextData(comparison);

                var validator = new DummyComparisonExclusiveBetweenContextValidator();

                var result = validator.Validate(validationContext);

                result.IsValid.Should().BeTrue();
            }

            [Fact]
            public void Should_Fail_Validate_Context()
            {
                var comparison = new ComparisonContext
                {
                    CompareFrom = GetWithinRange(1001, 1500),
                    CompareTo = GetWithinRange(1501, 1999)
                };

                var model = new DummyComparisonModel
                {
                    Value4 = comparison.CompareFrom + 1,        // not out of range
                    Value5 = comparison.CompareFrom,
                    Value6 = comparison.CompareTo
                };

                var validationContext = new ValidationContext<DummyComparisonModel>(model);
                validationContext.SetContextData(comparison);

                var validator = new DummyComparisonExclusiveBetweenContextValidator();

                var result = validator.Validate(validationContext);

                result.IsValid.Should().BeFalse();

                result.Errors.Should().BeEquivalentTo(new[]
                {
                    new
                    {
                        PropertyName = nameof(DummyComparisonModel.Value5),
                        ErrorCode = nameof(ValidationErrorCode.OutOfRange),
                        AttemptedValue = (object)model.Value5,
                        ErrorMessage = $"'{nameof(DummyComparisonModel.Value5)}' must be between {comparison.CompareFrom} and {comparison.CompareTo} (exclusive)."
                    },
                    new
                    {
                        PropertyName = nameof(DummyComparisonModel.Value6),
                        ErrorCode = nameof(ValidationErrorCode.OutOfRange),
                        AttemptedValue = (object)model.Value6,
                        ErrorMessage = $"'{nameof(DummyComparisonModel.Value6)}' must be between {comparison.CompareFrom} and {comparison.CompareTo} (exclusive)."
                    }
                }, options => options.ExcludingMissingMembers());
            }
        }
    }
}
