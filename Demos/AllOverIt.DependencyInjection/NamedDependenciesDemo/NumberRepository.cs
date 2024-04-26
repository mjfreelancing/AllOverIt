namespace NamedDependenciesDemo
{
    internal sealed class NumberRepository : IRepository
    {
        private readonly Random _random = new((int) DateTime.Now.Ticks);

        public string GetValue()
        {
            return $"{_random.Next(1, 100)}";
        }
    }
}