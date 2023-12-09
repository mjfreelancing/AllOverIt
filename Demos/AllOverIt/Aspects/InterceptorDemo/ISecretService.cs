using System.Threading.Tasks;

namespace InterceptorDemo
{
    public interface ISecretService
    {
        int GetSecretId();
        string GetSecret(string accessKey);
        Task<string> GetSecretAsync(string accessKey, bool shouldThrow);
    }
}