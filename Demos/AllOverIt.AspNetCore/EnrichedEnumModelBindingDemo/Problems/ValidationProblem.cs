﻿using AllOverIt.Extensions;
using FluentValidation;
using FluentValidation.Results;

namespace EnrichedEnumModelBindingDemo.Problems
{
    internal sealed class ValidationProblem : ProblemBase
    {
        public ValidationProblem(ValidationException exception)
            : base(StatusCodes.Status422UnprocessableEntity)
        {
            Detail = "Validation Error";
            AppendErrorCodes(Extensions, exception.Errors);
            // TraceId has already been added to the output
        }

        private static void AppendErrorCodes(IDictionary<string, object?> extensions, IEnumerable<ValidationFailure> errors)
        {
            var errorDetails = errors
                .SelectToReadOnlyCollection(error => new
                {
                    Field = error.PropertyName,
                    error.ErrorCode,
                    error.AttemptedValue,
                    error.ErrorMessage
                });

            extensions.Add("error", errorDetails);
        }
    }
}