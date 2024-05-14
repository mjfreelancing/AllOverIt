namespace ConsoleLoggingDemo
{
    public interface IAppService
    {
        Task<int[]> GetRandomNumbersAsync(int count, CancellationToken cancellationToken);
    }
}