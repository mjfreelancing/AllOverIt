using AllOverIt.Validation.Extensions;
using FluentAssertions;

namespace AllOverIt.Validation.Tests.Validators
{
    public partial class ValidatorsFixture
    {
        public class IsUnique : ValidatorsFixture
        {
            private class DummyIsUniqueValidator : ValidatorBase<DummyDtos>
            {
                public DummyIsUniqueValidator()
                {
                    RuleFor(model => model.Dtos).IsUnique(model => model.Value1);
                    RuleFor(model => model.Dtos).IsUnique(model => model.Value1, model => model.Value2);
                    RuleFor(model => model.Dtos).IsUnique(model => model.Value1, model => model.Value2, model => model.Value3);
                    RuleFor(model => model.Dtos).IsUnique(model => model.Value1, model => model.Value2, model => model.Value3, model => model.Value4);
                }
            }

            [Fact]
            public void Should_Succeed_Validate()
            {
                var model = new DummyDtos
                {
                    Dtos = Enumerable.Range(1, 4).Select(i => new DummyDto
                    {
                        Value1 = i.ToString(),
                        Value2 = Guid.NewGuid(),
                        Value3 = i + 1,
                        Value4 = i + 2
                    }).ToList()
                };

                var validator = new DummyIsUniqueValidator();

                var result = validator.Validate(model);

                result.IsValid.Should().BeTrue();
            }

            [Fact]
            public void Should_Fail_Validate_For_Value1()
            {
                var model = new DummyDtos
                {
                    Dtos = Enumerable.Range(1, 8).Select(i => new DummyDto
                    {
                        Value1 = i < 3 ? "-1" : (i > 5 ? "-2" : i.ToString()),
                        Value2 = Guid.NewGuid(),
                        Value3 = i + 1,
                        Value4 = i + 2
                    }).ToList()
                };

                var validator = new DummyIsUniqueValidator();

                var result = validator.Validate(model);

                result.IsValid.Should().BeFalse();

                var expected = new[]
                {
                    new
                    {
                        PropertyName = nameof(DummyDto.Value1),
                        ErrorCode = nameof(ValidationErrorCode.Duplicate),
                        AttemptedValue = (string[])["-1", "-2"],
                        ErrorMessage = $"'{nameof(DummyDto.Value1)}' must have unique values."
                    }
                };

                expected.Should().BeEquivalentTo(result.Errors, option => option.ExcludingMissingMembers());
            }

            [Fact]
            public void Should_Fail_Validate_For_Value1_Value2()
            {
                var guid1 = Guid.NewGuid();
                var guid2 = Guid.NewGuid();

                var model = new DummyDtos
                {
                    Dtos = Enumerable.Range(1, 8).Select(i => new DummyDto
                    {
                        Value1 = i < 3 ? "-1" : (i > 5 ? "-2" : i.ToString()),
                        Value2 = i < 3 ? guid1 : (i > 5 ? guid2 : Guid.NewGuid()),
                        Value3 = i + 1,
                        Value4 = i + 2
                    }).ToList()
                };

                var validator = new DummyIsUniqueValidator();

                var result = validator.Validate(model);

                result.IsValid.Should().BeFalse();

                var expected = new[]
                {
                     new
                    {
                        PropertyName = nameof(DummyDto.Value1),
                        ErrorCode = nameof(ValidationErrorCode.Duplicate),
                        AttemptedValue = (string[])["-1", "-2"],
                        ErrorMessage = $"'{nameof(DummyDto.Value1)}' must have unique values."
                    },
                    new
                    {
                        PropertyName = $"{nameof(DummyDto.Value1)}, {nameof(DummyDto.Value2)}",
                        ErrorCode = nameof(ValidationErrorCode.Duplicate),
                        AttemptedValue = (string[])[$"-1, {guid1.ToString()}", $"-2, {guid2.ToString()}"],
                        ErrorMessage = $"The combination of '{nameof(DummyDto.Value1)}' and '{nameof(DummyDto.Value2)}' must be unique."
                    }
                };

                expected.Should().BeEquivalentTo(result.Errors, option => option.ExcludingMissingMembers());
            }

            [Fact]
            public void Should_Fail_Validate_For_Value1_Value2_Value3()
            {
                var guid1 = Guid.NewGuid();
                var guid2 = Guid.NewGuid();

                var model = new DummyDtos
                {
                    Dtos = Enumerable.Range(1, 8).Select(i => new DummyDto
                    {
                        Value1 = i < 3 ? "-1" : (i > 5 ? "-2" : i.ToString()),
                        Value2 = i < 3 ? guid1 : (i > 5 ? guid2 : Guid.NewGuid()),
                        Value3 = i < 3 ? 1 : (i > 5 ? 2 : -i),
                        Value4 = i + 2
                    }).ToList()
                };

                var validator = new DummyIsUniqueValidator();

                var result = validator.Validate(model);

                result.IsValid.Should().BeFalse();

                var expected = new[]
                {
                     new
                    {
                        PropertyName = nameof(DummyDto.Value1),
                        ErrorCode = nameof(ValidationErrorCode.Duplicate),
                        AttemptedValue = (string[])["-1", "-2"],
                        ErrorMessage = $"'{nameof(DummyDto.Value1)}' must have unique values."
                    },
                    new
                    {
                        PropertyName = $"{nameof(DummyDto.Value1)}, {nameof(DummyDto.Value2)}",
                        ErrorCode = nameof(ValidationErrorCode.Duplicate),
                        AttemptedValue = (string[])[$"-1, {guid1.ToString()}", $"-2, {guid2.ToString()}"],
                        ErrorMessage = $"The combination of '{nameof(DummyDto.Value1)}' and '{nameof(DummyDto.Value2)}' must be unique."
                    },
                    new
                    {
                        PropertyName = $"{nameof(DummyDto.Value1)}, {nameof(DummyDto.Value2)}, {nameof(DummyDto.Value3)}",
                        ErrorCode = nameof(ValidationErrorCode.Duplicate),
                        AttemptedValue = (string[])[$"-1, {guid1.ToString()}, 1", $"-2, {guid2.ToString()}, 2"],
                        ErrorMessage = $"The combination of '{nameof(DummyDto.Value1)}', '{nameof(DummyDto.Value2)}' and '{nameof(DummyDto.Value3)}' must be unique."
                    }
                };

                expected.Should().BeEquivalentTo(result.Errors, option => option.ExcludingMissingMembers());
            }

            [Fact]
            public void Should_Fail_Validate_For_Value1_Value2_Value3_Value4()
            {
                var guid1 = Guid.NewGuid();
                var guid2 = Guid.NewGuid();

                var model = new DummyDtos
                {
                    Dtos = Enumerable.Range(1, 8).Select(i => new DummyDto
                    {
                        Value1 = i < 3 ? "-1" : (i > 5 ? "-2" : i.ToString()),
                        Value2 = i < 3 ? guid1 : (i > 5 ? guid2 : Guid.NewGuid()),
                        Value3 = i < 3 ? 1 : (i > 5 ? 2 : -i),
                        Value4 = i < 3 ? -1 : (i > 5 ? -2 : i)
                    }).ToList()
                };

                var validator = new DummyIsUniqueValidator();

                var result = validator.Validate(model);

                result.IsValid.Should().BeFalse();

                var expected = new[]
                {
                     new
                    {
                        PropertyName = nameof(DummyDto.Value1),
                        ErrorCode = nameof(ValidationErrorCode.Duplicate),
                        AttemptedValue = (string[])["-1", "-2"],
                        ErrorMessage = $"'{nameof(DummyDto.Value1)}' must have unique values."
                    },
                    new
                    {
                        PropertyName = $"{nameof(DummyDto.Value1)}, {nameof(DummyDto.Value2)}",
                        ErrorCode = nameof(ValidationErrorCode.Duplicate),
                        AttemptedValue = (string[])[$"-1, {guid1.ToString()}", $"-2, {guid2.ToString()}"],
                        ErrorMessage = $"The combination of '{nameof(DummyDto.Value1)}' and '{nameof(DummyDto.Value2)}' must be unique."
                    },
                    new
                    {
                        PropertyName = $"{nameof(DummyDto.Value1)}, {nameof(DummyDto.Value2)}, {nameof(DummyDto.Value3)}",
                        ErrorCode = nameof(ValidationErrorCode.Duplicate),
                        AttemptedValue = (string[])[$"-1, {guid1.ToString()}, 1", $"-2, {guid2.ToString()}, 2"],
                        ErrorMessage = $"The combination of '{nameof(DummyDto.Value1)}', '{nameof(DummyDto.Value2)}' and '{nameof(DummyDto.Value3)}' must be unique."
                    },
                    new
                    {
                        PropertyName = $"{nameof(DummyDto.Value1)}, {nameof(DummyDto.Value2)}, {nameof(DummyDto.Value3)}, {nameof(DummyDto.Value4)}",
                        ErrorCode = nameof(ValidationErrorCode.Duplicate),
                        AttemptedValue = (string[])[$"-1, {guid1.ToString()}, 1, -1", $"-2, {guid2.ToString()}, 2, -2"],
                        ErrorMessage = $"The combination of '{nameof(DummyDto.Value1)}', '{nameof(DummyDto.Value2)}', '{nameof(DummyDto.Value3)}' and '{nameof(DummyDto.Value4)}' must be unique."
                    }
                };

                expected.Should().BeEquivalentTo(result.Errors, option => option.ExcludingMissingMembers());
            }
        }
    }
}
