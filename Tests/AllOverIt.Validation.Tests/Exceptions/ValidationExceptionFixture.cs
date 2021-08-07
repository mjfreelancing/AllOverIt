using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using ValidationException = AllOverIt.Validation.Exceptions.ValidationException;

namespace AllOverIt.Validation.Tests.Exceptions
{
    public class ValidationExceptionFixture : FixtureBase
    {
        public class Constructor_ValidationError : ValidationExceptionFixture
        {
            [Fact]
            public void Should_Throw_When_Errors_Null()
            {
                Invoking(() =>
                {
                    _ = new ValidationException((IEnumerable<ValidationError>)null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("errors");
            }

            [Fact]
            public void Should_Throw_When_Errors_Empty()
            {
                Invoking(() =>
                {
                    _ = new ValidationException(new List<ValidationError>());
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("errors");
            }

            [Fact]
            public void Should_Set_Errors()
            {
                var errors = CreateMany<ValidationError>();

                var exception = new ValidationException(errors);

                exception.Errors
                    .Should()
                    .BeEquivalentTo(errors);
            }

            [Fact]
            public void Should_Set_Message()
            {
                var errors = CreateMany<ValidationError>();

                var exception = new ValidationException(errors);

                exception.Message
                    .Should()
                    .BeEquivalentTo(string.Join(", ", errors.Select(error => error.Message)));
            }
        }

        public class Constructor_ValidationFailure : ValidationExceptionFixture
        {
            [Fact]
            public void Should_Throw_When_Errors_Null()
            {
                Invoking(() =>
                {
                    _ = new ValidationException((IEnumerable<ValidationFailure>)null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("failures");
            }

            [Fact]
            public void Should_Throw_When_Errors_Empty()
            {
                Invoking(() =>
                {
                    _ = new ValidationException(new List<ValidationFailure>());
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("errors");
            }

            [Fact]
            public void Should_Set_Errors()
            {
                var errors = CreateMany<ValidationFailure>();

                foreach (var error in errors)
                {
                    error.ErrorCode = $"{Create<ValidationErrorCode>()}";
                }

                var exception = new ValidationException(errors);

                var expected = errors
                    .SelectAsReadOnlyCollection(item => new ValidationError(
                        item.ErrorCode.As<ValidationErrorCode>(),
                        item.ErrorMessage,
                        item.PropertyName,
                        item.AttemptedValue));

                exception.Errors
                    .Should()
                    .BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Set_Message()
            {
                var errors = CreateMany<ValidationFailure>();

                foreach (var error in errors)
                {
                    error.ErrorCode = $"{Create<ValidationErrorCode>()}";
                }

                var exception = new ValidationException(errors);

                exception.Message
                    .Should()
                    .BeEquivalentTo(string.Join(", ", errors.Select(error => error.ErrorMessage)));
            }
        }
    }
}
