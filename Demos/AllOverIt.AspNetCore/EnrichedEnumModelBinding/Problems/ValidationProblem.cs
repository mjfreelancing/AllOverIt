using AllOverIt.Extensions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace EnrichedEnumModelBinding.Problems
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

        private static void AppendErrorCodes(IDictionary<string, object> extensions, IEnumerable<ValidationFailure> errors)
        {
            var errorDetails = errors
                .SelectAsReadOnlyCollection(error => new
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