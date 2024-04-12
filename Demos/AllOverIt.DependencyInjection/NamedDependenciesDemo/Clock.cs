using System;

namespace NamedDependenciesDemo
{
    internal sealed class Clock : IClock
    {
        public string GetValue()
        {
            return $"{DateTime.Now}";
        }
    }
}