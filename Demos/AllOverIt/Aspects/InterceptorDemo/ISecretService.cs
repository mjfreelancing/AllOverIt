namespace InterceptorDemo
{
    public interface ISecretService
    {
        void Initialize();
        Task InitializeAsync();
        int GetSecretId();
        string GetSecret(string accessKey);
        Task<string> GetSecretAsync(string accessKey, bool shouldThrow);
    }
}