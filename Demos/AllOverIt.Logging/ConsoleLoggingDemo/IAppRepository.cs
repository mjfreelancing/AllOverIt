namespace ConsoleLoggingDemo
{
    public interface IAppRepository
    {
        int[] GetRandomNumbers(int count, int maxValue, CancellationToken cancellationToken);
    }
}