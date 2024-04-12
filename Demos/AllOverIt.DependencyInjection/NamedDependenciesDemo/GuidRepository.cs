using System;

namespace NamedDependenciesDemo
{
    internal sealed class GuidRepository : IRepository
    {
        public string GetValue()
        {
            return $"{Guid.NewGuid()}";
        }
    }
}