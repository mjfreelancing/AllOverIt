namespace ExternalDependencies
{
    internal sealed class NameRepository : IRepository
    {
        private static readonly string[] Names = ["Peter", "Billy", "Mary", "Lisa", "Joseph", "Bernadette"];
        private readonly Random _rnd = new();

        public string GetRandomName()
        {
            var index = _rnd.Next(6);
            return Names[index];
        }
    }
}