namespace FilteringDemo
{
    internal sealed class Product
    {
        public required bool Active { get; set; }
        public required string Category { get; set; }
        public required string Name { get; set; }
        public required double Price { get; set; }
        public required DateTimeValue LastUpdated { get; set; }      // Demonstrating how to use 'value types'
    }
}