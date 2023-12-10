using System.Threading.Tasks;

namespace InterceptorDemo
{
    public interface ISecretService
    {
        // Testing: void return type
        void Initialize();

        // Testing: Task return type
        Task InitializeAsync();

        // Testing: value type result
        int GetSecretId();

        // Testing: reference result
        string GetSecret(string accessKey);

        // Testing: Task result
        Task<string> GetSecretAsync(string accessKey, bool shouldThrow);
    }
}