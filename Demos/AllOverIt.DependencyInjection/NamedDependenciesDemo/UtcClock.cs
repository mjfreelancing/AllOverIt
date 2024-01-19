namespace NamedDependenciesDemo
{
    internal sealed class UtcClock : IClock
    {
        public string GetValue()
        {
            return $"{DateTime.UtcNow}";
        }
    }
}