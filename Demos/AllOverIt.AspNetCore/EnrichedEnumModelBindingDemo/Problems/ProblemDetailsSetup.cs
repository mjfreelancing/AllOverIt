using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ProblemDetailsOptions = Hellang.Middleware.ProblemDetails.ProblemDetailsOptions;

namespace EnrichedEnumModelBindingDemo.Problems
{
    internal static class ProblemDetailsSetup
    {
        private static readonly List<Type> IgnoredExceptions = [
            typeof(OperationCanceledException)
        ];

        private static readonly Dictionary<Type, Func<Exception, ProblemDetails>> CustomExceptionProblems = new()
        {
            {
                typeof(ValidationException),
                exception => new ValidationProblem(exception as ValidationException)
            }
        };

        public static void Configure(ProblemDetailsOptions options)
        {
            options.IncludeExceptionDetails = (ctx, ex) => ctx.RequestServices.GetRequiredService<IHostEnvironment>().IsDevelopment() &&
                                                           !CustomExceptionProblems.ContainsKey(ex.GetType());

            // examples of what else can be performed
            // options.Rethrow<NotSupportedException>();
            // options.MapToStatusCode<NotImplementedException>(StatusCodes.Status501NotImplemented);
            // options.MapToStatusCode<HttpRequestException>(StatusCodes.Status503ServiceUnavailable);

            MapProblemException<ValidationException>(options);

            options.MapToStatusCode<OperationCanceledException>(StatusCodes.Status200OK);

            // the "catch all" mapping - must be the last in the list
            options.MapToStatusCode<Exception>(StatusCodes.Status500InternalServerError);

            // need to re-throw unhandled exceptions so bugsnag can report it
            options.Rethrow((_, exception) =>
            {
                var exceptionType = exception.GetType();

                return !(IgnoredExceptions.Contains(exceptionType) || CustomExceptionProblems.ContainsKey(exceptionType));
            });
        }

        private static void MapProblemException<TException>(ProblemDetailsOptions options) where TException : Exception
        {
            options.Map<TException>(GetProblemDetails);
        }

        private static ProblemDetails GetProblemDetails<TException>(TException exception) where TException : Exception
        {
            var exceptionType = typeof(TException);

            if (CustomExceptionProblems.TryGetValue(exceptionType, out var resolver))
            {
                return resolver.Invoke(exception);
            }

            throw new InvalidOperationException($"Exception of type {exceptionType.Name} has not been registered");
        }
    }
}