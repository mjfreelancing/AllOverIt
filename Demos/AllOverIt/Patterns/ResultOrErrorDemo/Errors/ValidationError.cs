using AllOverIt.Patterns.Result;

namespace ResultOrErrorDemo.Errors;

public sealed class ValidationError : EnrichedError<AppErrorType>
{
    public ValidationError()
        : base(
            AppErrorType.Validation,
            nameof(AppErrorType.Validation),
            "A validation error occurred")
    {
    }

    public ValidationError(string code, string description)
           : base(AppErrorType.Validation, code, description)
    {
    }

    public ValidationError(string description)
           : base(AppErrorType.Validation, null, description)
    {
    }
}