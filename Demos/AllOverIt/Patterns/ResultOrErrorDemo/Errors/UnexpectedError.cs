using AllOverIt.Patterns.Result;

namespace ResultOrErrorDemo.Errors;

public sealed class UnexpectedError : EnrichedError<AppErrorType>
{
    public UnexpectedError()
        : base(
            AppErrorType.Unexpected,
            nameof(AppErrorType.Unexpected),
            "An unexpected error occurred.")
    {
    }

    public UnexpectedError(string code, string description)
           : base(AppErrorType.Unexpected, code, description)
    {
    }

    public UnexpectedError(string description)
           : base(AppErrorType.Unexpected, null, description)
    {
    }
}
