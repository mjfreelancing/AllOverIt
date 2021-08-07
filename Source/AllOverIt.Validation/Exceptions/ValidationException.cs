using AllOverIt.Extensions;
using AllOverIt.Helpers;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;

namespace AllOverIt.Validation.Exceptions
{
    [Serializable]
    public sealed class ValidationException : Exception
    {
        private readonly string _message;
        public override string Message => _message ?? base.Message;

        public IEnumerable<ValidationError> Errors { get; }

        public ValidationException(IEnumerable<ValidationError> errors)
        {
            Errors = errors
                .WhenNotNullOrEmpty(nameof(errors))
                .AsReadOnlyCollection();

            if (Errors.Any())
            {
                _message = string.Join(", ", Errors.Select(error => error.Message));
            }
        }

        public ValidationException(IEnumerable<ValidationFailure> failures)
            : this(CreateValidationErrors(failures))
        {
        }

        [ExcludeFromCodeCoverage]
        private ValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            var errorsInfo = info.GetValue(nameof(Errors), typeof(IEnumerable<ValidationError>)) ?? Enumerable.Empty<ValidationError>();
            Errors = errorsInfo as IEnumerable<ValidationError>;
        }

        [ExcludeFromCodeCoverage]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            _ = info.WhenNotNull(nameof(info));

            info.AddValue(nameof(Errors), Errors);
            base.GetObjectData(info, context);
        }

        private static IEnumerable<ValidationError> CreateValidationErrors(IEnumerable<ValidationFailure> failures)
        {
            return failures
                .WhenNotNull(nameof(failures))
                .SelectAsReadOnlyCollection(item => new ValidationError(
                    item.ErrorCode.As<ValidationErrorCode>(),
                    item.ErrorMessage,
                    item.PropertyName,
                    item.AttemptedValue));
        }
    }
}