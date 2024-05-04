using AllOverIt.Validation.Extensions;
using FluentAssertions;
using FluentValidation;

namespace AllOverIt.Validation.Tests.Validators
{
    public partial class ValidatorsFixture
    {
        public class IsGreaterThan : ValidatorsFixture
        {
            private class ComparisonContext
            {
                public int CompareTo { get; set; }
            }

            private class DummyComparisonGreaterThanExplicitValidator : ValidatorBase<DummyComparisonModel>
            {
                public DummyComparisonGreaterThanExplicitValidator()
                {
                    // nullable and non-nullable, explicit comparison value
                    RuleFor(model => model.Value3).IsGreaterThan(0);
                    RuleFor(model => model.Value4).IsGreaterThan(0);
                }
            }

            private class DummyComparisonGreaterThanContextValidator : ValidatorBase<DummyComparisonModel>
            {
                public DummyComparisonGreaterThanContextValidator()
                {
                    // nullable and non-nullable, context provided comparison value
                    RuleFor(model => model.Value5).IsGreaterThan<DummyComparisonModel, int, ComparisonContext>(ctx => ctx.CompareTo);
                    RuleFor(model => model.Value6).IsGreaterThan<DummyComparisonModel, int, ComparisonContext>(ctx => ctx.CompareTo);
                }
            }

            [Fact]
            public void Should_Succeed_Validate_Explicit()
            {
                var model = new DummyComparisonModel
                {
                    Value3 = CreateExcluding(0),
                    Value4 = CreateExcluding(0)
                };

                var validator = new DummyComparisonGreaterThanExplicitValidator();

                var result = validator.Validate(model);

                result.IsValid.Should().BeTrue();
            }

            [Fact]
            public void Should_Fail_Validate_Explicit()
            {
                var model = new DummyComparisonModel { Value3 = 0 };

                var validator = new DummyComparisonGreaterThanExplicitValidator();

                var result = validator.Validate(model);

                result.IsValid.Should().BeFalse();

                var expected = new[]
                {
                    new
                    {
                        PropertyName = nameof(DummyComparisonModel.Value3),
                        ErrorCode = nameof(ValidationErrorCode.OutOfRange),
                        AttemptedValue = (object) model.Value3,
                        ErrorMessage = $"'{nameof(DummyComparisonModel.Value3)}' must be greater than 0."
                    },
                    new
                    {
                        PropertyName = nameof(DummyComparisonModel.Value4),
                        ErrorCode = nameof(ValidationErrorCode.OutOfRange),
                        AttemptedValue = (object) model.Value4,
                        ErrorMessage = $"'{nameof(DummyComparisonModel.Value4)}' must be greater than 0."
                    }
                };

                expected.Should().BeEquivalentTo(result.Errors, option => option.ExcludingMissingMembers());
            }

            [Fact]
            public void Should_Succeed_Validate_Context()
            {
                var model = new DummyComparisonModel
                {
                    Value5 = Create<int>(),
                    Value6 = Create<int>()
                };

                var comparison = new ComparisonContext
                {
                    CompareTo = Math.Min(model.Value5.Value, model.Value6) - 1
                };

                var validationContext = new ValidationContext<DummyComparisonModel>(model);
                validationContext.SetContextData(comparison);

                var validator = new DummyComparisonGreaterThanContextValidator();

                var result = validator.Validate(validationContext);

                result.IsValid.Should().BeTrue();
            }

            [Fact]
            public void Should_Fail_Validate_Context()
            {
                var model = new DummyComparisonModel
                {
                    Value5 = Create<int>(),
                    Value6 = Create<int>()
                };

                var comparison = new ComparisonContext
                {
                    CompareTo = Math.Max(model.Value5.Value, model.Value6)
                };

                var validationContext = new ValidationContext<DummyComparisonModel>(model);
                validationContext.SetContextData(comparison);

                var validator = new DummyComparisonGreaterThanContextValidator();

                var result = validator.Validate(validationContext);

                result.IsValid.Should().BeFalse();

                var expected = new[]
                {
                    new
                    {
                        PropertyName = nameof(DummyComparisonModel.Value5),
                        ErrorCode = nameof(ValidationErrorCode.OutOfRange),
                        AttemptedValue = (object) model.Value5,
                        ErrorMessage = $"'{nameof(DummyComparisonModel.Value5)}' must be greater than {comparison.CompareTo}."
                    },
                    new
                    {
                        PropertyName = nameof(DummyComparisonModel.Value6),
                        ErrorCode = nameof(ValidationErrorCode.OutOfRange),
                        AttemptedValue = (object) model.Value6,
                        ErrorMessage = $"'{nameof(DummyComparisonModel.Value6)}' must be greater than {comparison.CompareTo}."
                    }
                };

                expected.Should().BeEquivalentTo(result.Errors, option => option.ExcludingMissingMembers());
            }
        }
    }
}
