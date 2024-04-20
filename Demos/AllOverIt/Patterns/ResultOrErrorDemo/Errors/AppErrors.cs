using AllOverIt.Patterns.Result;

namespace ResultOrErrorDemo.Errors;

public static class AppErrors
{
    public static EnrichedError BadRequest(
        string code = nameof(AppErrorType.BadRequest),
        string description = "A 'Bad Request' error has occurred.")
    {
        return new EnrichedError<AppErrorType>(AppErrorType.BadRequest, code, description);
    }

    public static EnrichedError Unexpected { get; } = new UnexpectedError();
    public static EnrichedError Validation { get; } = new ValidationError();
}
