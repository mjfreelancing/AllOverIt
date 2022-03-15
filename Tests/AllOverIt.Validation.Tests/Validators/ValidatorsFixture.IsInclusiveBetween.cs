﻿using AllOverIt.Validation.Extensions;
using FluentAssertions;
using FluentValidation;
using Xunit;

namespace AllOverIt.Validation.Tests.Validators
{
    public partial class ValidatorsFixture
    {
        public class IsInclusiveBetween : ValidatorsFixture
        {
            private class ComparisonContext
            {
                public int CompareFrom { get; set; }
                public int CompareTo { get; set; }
            }

            private class DummyComparisonInclusiveBetweenExplicitValidator : ValidatorBase<DummyComparisonModel>
            {
                public DummyComparisonInclusiveBetweenExplicitValidator()
                {
                    // nullable and non-nullable, explicit comparison value
                    RuleFor(model => model.Value3).IsInclusiveBetween(0, 10);
                    RuleFor(model => model.Value4).IsInclusiveBetween(0, 10);
                    RuleFor(model => model.Value5).IsInclusiveBetween(0, 10);
                }
            }

            private class DummyComparisonInclusiveBetweenContextValidator : ValidatorBase<DummyComparisonModel>
            {
                public DummyComparisonInclusiveBetweenContextValidator()
                {
                    // nullable and non-nullable, context provided comparison value
                    RuleFor(model => model.Value4).IsInclusiveBetween<DummyComparisonModel, int, ComparisonContext>(ctx => ctx.CompareFrom, ctx => ctx.CompareTo);
                    RuleFor(model => model.Value5).IsInclusiveBetween<DummyComparisonModel, int, ComparisonContext>(ctx => ctx.CompareFrom, ctx => ctx.CompareTo);
                    RuleFor(model => model.Value6).IsInclusiveBetween<DummyComparisonModel, int, ComparisonContext>(ctx => ctx.CompareFrom, ctx => ctx.CompareTo);
                }
            }

            [Fact]
            public void Should_Succeed_Validate_Explicit()
            {
                var model = new DummyComparisonModel
                {
                    Value3 = 0,
                    Value4 = GetWithinRange(0, 10),
                    Value5 = 10
                };

                var validator = new DummyComparisonInclusiveBetweenExplicitValidator();

                var result = validator.Validate(model);

                result.IsValid.Should().BeTrue();
            }

            [Fact]
            public void Should_Fail_Validate_Explicit()
            {
                var model = new DummyComparisonModel
                {
                    Value3 = -1,
                    Value4 = 11,
                    Value5 = 0      // not out of range
                };

                var validator = new DummyComparisonInclusiveBetweenExplicitValidator();

                var result = validator.Validate(model);

                result.IsValid.Should().BeFalse();

                var expected = new[]
                {
                    new
                    {
                        PropertyName = nameof(DummyComparisonModel.Value3),
                        ErrorCode = nameof(ValidationErrorCode.OutOfRange),
                        AttemptedValue = (object) model.Value3,
                        ErrorMessage = $"'{nameof(DummyComparisonModel.Value3)}' must be between 0 and 10 (inclusive)."
                    },
                    new
                    {
                        PropertyName = nameof(DummyComparisonModel.Value4),
                        ErrorCode = nameof(ValidationErrorCode.OutOfRange),
                        AttemptedValue = (object) model.Value4,
                        ErrorMessage = $"'{nameof(DummyComparisonModel.Value4)}' must be between 0 and 10 (inclusive)."
                    }
                };

                expected.Should().BeEquivalentTo(result.Errors, options => options.ExcludingMissingMembers());
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
                    CompareFrom = 1,
                    CompareTo = 1000
                };

                var validationContext = new ValidationContext<DummyComparisonModel>(model);
                validationContext.SetContextData(comparison);

                var validator = new DummyComparisonInclusiveBetweenContextValidator();

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
                    Value4 = comparison.CompareFrom,        // not out of range
                    Value5 = comparison.CompareFrom - 1,
                    Value6 = comparison.CompareTo + 1
                };

                var validationContext = new ValidationContext<DummyComparisonModel>(model);
                validationContext.SetContextData(comparison);

                var validator = new DummyComparisonInclusiveBetweenContextValidator();

                var result = validator.Validate(validationContext);

                result.IsValid.Should().BeFalse();

                var expected = new[]
                {
                    new
                    {
                        PropertyName = nameof(DummyComparisonModel.Value5),
                        ErrorCode = nameof(ValidationErrorCode.OutOfRange),
                        AttemptedValue = (object) model.Value5,
                        ErrorMessage =
                            $"'{nameof(DummyComparisonModel.Value5)}' must be between {comparison.CompareFrom} and {comparison.CompareTo} (inclusive)."
                    },
                    new
                    {
                        PropertyName = nameof(DummyComparisonModel.Value6),
                        ErrorCode = nameof(ValidationErrorCode.OutOfRange),
                        AttemptedValue = (object) model.Value6,
                        ErrorMessage =
                            $"'{nameof(DummyComparisonModel.Value6)}' must be between {comparison.CompareFrom} and {comparison.CompareTo} (inclusive)."
                    }
                };

                expected.Should().BeEquivalentTo(result.Errors, options => options.ExcludingMissingMembers());
            }
        }
    }
}
