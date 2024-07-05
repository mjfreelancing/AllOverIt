namespace InterceptorBenchmark
{
    public interface IService
    {
        string GetSecret();
        Task<string> GetSecretAsync(bool shouldThrow);
    }
}