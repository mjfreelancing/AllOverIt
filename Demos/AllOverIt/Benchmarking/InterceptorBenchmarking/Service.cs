﻿using System.Threading.Tasks;

namespace InterceptorBenchmarking
{
    internal sealed class Service : IService
    {
        public string GetSecret()
        {
            return string.Empty;
        }

        public Task<string> GetSecretAsync(bool shouldThrow)
        {
            return Task.FromResult(string.Empty);
        }
    }
}