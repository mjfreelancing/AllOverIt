﻿using AllOverIt.Validation.Extensions;
using FluentAssertions;
using System;
using Xunit;

namespace AllOverIt.Validation.Tests.Validators
{
    public partial class ValidatorsFixture
    {
        public class IsNotNullOrEmpty : ValidatorsFixture
        {
            private class DummyIsRequiredValidator : ValidatorBase<DummyComparisonModel>
            {
                public DummyIsRequiredValidator()
                {
                    RuleFor(model => model.Value1).IsNotNullOrEmpty();    // string
                    RuleFor(model => model.Value2).IsNotNullOrEmpty();    // Guid
                    RuleFor(model => model.Value3).IsNotNullOrEmpty();    // nullable
                    RuleFor(model => model.Value7).IsNotNullOrEmpty();    // IReadOnlyList<int>
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

                result.Errors.Should().BeEquivalentTo(new[]
                {
                    new
                    {
                        PropertyName = nameof(DummyComparisonModel.Value1),
                        ErrorCode = nameof(ValidationErrorCode.NotEmpty),
                        AttemptedValue = (object)model.Value1,
                        ErrorMessage = $"'{nameof(DummyComparisonModel.Value1)}' should not be empty."
                    },
                    new
                    {
                        PropertyName = nameof(DummyComparisonModel.Value2),
                        ErrorCode = nameof(ValidationErrorCode.NotEmpty),
                        AttemptedValue = (object)model.Value2,
                        ErrorMessage = $"'{nameof(DummyComparisonModel.Value2)}' should not be empty."
                    },
                    new
                    {
                        PropertyName = nameof(DummyComparisonModel.Value3),
                        ErrorCode = nameof(ValidationErrorCode.NotEmpty),
                        AttemptedValue = (object)model.Value3,
                        ErrorMessage = $"'{nameof(DummyComparisonModel.Value3)}' should not be empty."
                    },
                    new
                    {
                        PropertyName = nameof(DummyComparisonModel.Value7),
                        ErrorCode = nameof(ValidationErrorCode.NotEmpty),
                        AttemptedValue = (object)model.Value7,
                        ErrorMessage = $"'{nameof(DummyComparisonModel.Value7)}' should not be empty."
                    }
                }, options => options.ExcludingMissingMembers());
            }
        }
    }
}
