using AllOverIt.Filtering.Extensions;
using AllOverIt.Filtering.Filters;
using AllOverIt.Patterns.Specification.Extensions;
using System;
using System.Linq;

namespace FilteringDemo
{
    internal sealed class Product
    {
        public string Category { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
    }

    public sealed class ProductFilter : IFilter
    {
        public sealed class CategoryFilter
        {
            public StartsWith StartsWith { get; set; } 
        }

        public sealed class NameFilter
        {
            public Contains Contains { get; set; }
        }

        public sealed class PriceFilter
        {
            public GreaterThanOrEqual<double> GreaterThanOrEqual { get; set; }
            public LessThanOrEqual<double> LessThanOrEqual { get; set; }
        }

        public CategoryFilter Category { get; init; } = new();
        public NameFilter Name { get; init; } = new();
        public PriceFilter Price { get; init; } = new();
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            var products = GetProducts();

            var filter = new ProductFilter
            {
                Category = {
                    StartsWith = "fu"
                },
                Name = {
                    Contains = "r"
                },
                Price = {
                    GreaterThanOrEqual = 15.0,
                    LessThanOrEqual = 700.0
                }
            };

            var filterOptions = new QueryFilterOptions
            {
                UseParameterizedQueries = false,
                StringComparison = StringComparison.InvariantCultureIgnoreCase
            };

            var results = products
                .AsQueryable()
                .ApplyFilter(filter, (specificationBuilder, filterBuilder) =>
                {
                    var priceGte = specificationBuilder.Create(product => product.Price, f => f.Price.GreaterThanOrEqual);
                    var priceLte = specificationBuilder.Create(product => product.Price, f => f.Price.LessThanOrEqual);

                    filterBuilder
                        .Where(product => product.Category, f => f.Category.StartsWith)
                        .And(product => product.Name, f => f.Name.Contains)
                        .And(priceGte.And(priceLte));

                    // TODO
                    // var queryString = filterBuilder.ToString()
                }, filterOptions)
                .ToList();

            foreach (var result in results)
            {
                Console.WriteLine($"{result.Category}, {result.Name}, {result.Price}");
            }

            Console.WriteLine();
            Console.WriteLine("All Over It.");
            Console.ReadKey();
        }

        private static Product[] GetProducts()
        {
            return new Product[]
            {
                new Product
                {
                    Category = "Furniture",
                    Name = "Chairs",
                    Price = 1000
                },
                new Product
                {
                    Category = "Furniture",
                    Name = "Table",
                    Price = 800
                },
                new Product
                {
                    Category = "Furniture",
                    Name = "Cupboard",
                    Price = 500
                },
                new Product
                {
                    Category = "Furniture",
                    Name = "Dresser",
                    Price = 250
                },
                new Product
                {
                    Category = "Furniture",
                    Name = "Lamp",
                    Price = 50
                },
                new Product
                {
                    Category = "Clothing",
                    Name = "Shirt",
                    Price = 10
                },
                new Product
                {
                    Category = "Clothing",
                    Name = "Trousers",
                    Price = 15
                },
                new Product
                {
                    Category = "Clothing",
                    Name = "Jumper",
                    Price = 20
                },
                new Product
                {
                    Category = "Clothing",
                    Name = "Jacket",
                    Price = 20
                },
                new Product
                {
                    Category = "Clothing",
                    Name = "Shoes",
                    Price = 200
                },
                new Product
                {
                    Category = "Clothing",
                    Name = "Socks",
                    Price = 25
                },
                new Product
                {
                    Category = "Clothing",
                    Name = "Underwear",
                    Price = 18
                }
            };
        }
    }
}