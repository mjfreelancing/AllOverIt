using System;

namespace FilteringDemo
{
    internal sealed class Product
    {
        public bool? Active { get; set; }
        public string Category { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}