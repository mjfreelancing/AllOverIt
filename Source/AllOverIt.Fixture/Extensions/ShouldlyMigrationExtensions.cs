using Shouldly;

namespace AllOverIt.Fixture.Extensions;

public static class ShouldlyMigrationExtensions
{
    public static TInner WithInnerException<TInner>(this Exception exception)
        where TInner : Exception
    {
        exception.InnerException.ShouldNotBeNull();
        return exception.InnerException.ShouldBeOfType<TInner>();
    }

    public static async Task<TInner> WithInnerException<TInner, TException>(this Task<TException> exceptionTask)
        where TInner : Exception
        where TException : Exception
    {
        var exception = await exceptionTask;

        return exception.WithInnerException<TInner>();
    }

    public static TException ShouldThrow<TException, TResult>(this Func<TResult> action)
        where TException : Exception
    {
        return Should.Throw<TException>(() => _ = action());
    }

    public static Task<TException> ShouldThrowAsync<TException, TResult>(this Func<Task<TResult>> action)
        where TException : Exception
    {
        return Should.ThrowAsync<TException>(async () => _ = await action());
    }

    public static IEnumerable<T> ShouldNotBeNullOrEmpty<T>(this IEnumerable<T>? value, string? customMessage = null)
    {
        value.ShouldNotBeNull(customMessage);
        value.Any().ShouldBeTrue(customMessage);

        return value;
    }
}