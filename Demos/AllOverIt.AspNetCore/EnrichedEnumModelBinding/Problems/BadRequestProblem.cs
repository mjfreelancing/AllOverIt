using AllOverIt.Extensions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;

namespace EnrichedEnumModelBinding.Problems
{
    internal sealed class BadRequestProblem : ProblemBase
    {
        public BadRequestProblem(ActionContext context)
            : base(StatusCodes.Status400BadRequest)
        {
            var failures = CreateValidationFailures(context);

            Detail = "Invalid Input";
            AppendErrorCodes(Extensions, failures);
            TryAddTraceId(context.HttpContext);
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

        private static IReadOnlyCollection<ValidationFailure> CreateValidationFailures(ActionContext context)
        {
            var failures = new List<ValidationFailure>();

            foreach (var keyModelStatePair in context.ModelState)
            {
                var value = keyModelStatePair.Value;

                if (value.ValidationState == ModelValidationState.Invalid)
                {
                    var field = keyModelStatePair.Key;
                    var errors = value.Errors;

                    for (var i = 0; i < errors.Count; i++)
                    {
                        var error = errors[i];

                        var errorMessage = error.ErrorMessage.IsNullOrEmpty()
                            ? "Invalid input."
                            : error.ErrorMessage;

                        var failure = new ValidationFailure(field, errorMessage, value.AttemptedValue)
                        {
                            ErrorCode = "Invalid"
                        };

                        failures.Add(failure);
                    }
                }
            }

            return failures;
        }
    }
}