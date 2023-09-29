using System.Threading.Tasks;

namespace InterceptorDemo
{
    public interface ISecretService
    {
        string GetSecret(string accessKey);
        Task<string> GetSecretAsync(string accessKey, bool shouldThrow);
    }
}