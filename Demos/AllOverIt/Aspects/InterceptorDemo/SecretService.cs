using System;
using System.Threading.Tasks;

namespace InterceptorDemo
{
    internal sealed class SecretService : ISecretService
    {
        public void Initialize()
        {
        }

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public int GetSecretId()
        {
            return 42;
        }

        public string GetSecret(string accessKey)
        {
            return $"{accessKey}-{Guid.NewGuid()}";
        }

        public async Task<string> GetSecretAsync(string accessKey, bool shouldThrow)
        {
            await Task.Delay(1000);

            if (shouldThrow)
            {
                throw new Exception("Test Exception");
            }

            return GetSecret(accessKey);
        }
    }
}