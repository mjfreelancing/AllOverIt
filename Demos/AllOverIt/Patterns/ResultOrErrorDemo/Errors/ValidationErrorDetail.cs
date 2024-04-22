namespace ResultOrErrorDemo.Errors;

public sealed record ValidationErrorDetail
{
    public object? AttemptedValue { get; set; }
    public required string Message { get; set; }
}
