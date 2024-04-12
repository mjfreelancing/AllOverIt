using System;

namespace NamedDependenciesDemo
{
    internal sealed class DateTimeRepository : IRepository
    {
        public string GetValue()
        {
            return $"{DateTime.Now}";
        }
    }
}