using AllOverIt.Expressions.Strings;
using AllOverIt.Filtering.Extensions;
using AllOverIt.Filtering.Options;
using AllOverIt.Patterns.Specification.Extensions;
using System;
using System.Linq;

namespace FilteringDemo
{
    internal class Program
    {
        static void Main()
        {
            var products = GetProducts();

            var productFilter = new ProductFilter
            {
                Active =
                {
                    //EqualTo = true
                },
                Category = {
                    //StartsWith = "fu"
                },
                Name = {
                    Contains = "r"
                },
                Price = {
                    GreaterThanOrEqual = 15.0,
                    LessThanOrEqual = 700.0
                },
                LastUpdated = {
                    GreaterThanOrEqual = DateTime.UtcNow.Date.AddDays(-10),     // Change to 14 to see two records return
                }
            };

            var filterOptions = new DefaultQueryFilterOptions
            {
                UseParameterizedQueries = false,
                StringComparisonMode = StringComparisonMode.ToLower,
                IgnoreDefaultFilterValues = false
            };

            var results = products
                .AsQueryable()
                .ApplyFilter(productFilter, (specificationBuilder, filterBuilder) =>
                {
                    var priceGte = specificationBuilder.Create(product => product.Price, filter => filter.Price.GreaterThanOrEqual);
                    var priceLte = specificationBuilder.Create(product => product.Price, filter => filter.Price.LessThanOrEqual);

                    filterBuilder
                        .Where(product => product.Active, filter => filter.Active.EqualTo)
                        .And(product => product.Category, filter => filter.Category.StartsWith)
                        .And(product => product.Name, filter => filter.Name.Contains)
                        .And(priceGte.And(priceLte))
                        .And(product => product.LastUpdated, filter => filter.LastUpdated.GreaterThanOrEqual);

                    // Output the generated query as readable text, such as:
                    // (((Category StartsWith 'fu' AND Name Contains 'r') AND ((Price >= 15) AND (Price <= 700))) AND (LastUpdated >= '2022-07-07T00:00:00.000Z'))
                    var queryString = filterBuilder.ToQueryString();

                    Console.WriteLine();
                    Console.WriteLine($"Filter as query string: {queryString}");
                    Console.WriteLine();
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
            var today = DateTime.UtcNow.Date;

            return new Product[]
            {
                new Product
                {
                    Active = true,
                    Category = "Furniture",
                    Name = "Chairs",
                    Price = 1000,
                    LastUpdated = today.AddDays(-1)
                },
                new Product
                {
                    Active = true,
                    Category = "Furniture",
                    Name = "Table",
                    Price = 800,
                    LastUpdated = today
                },
                new Product
                {
                    Active = true,
                    Category = "Furniture",
                    Name = "Cupboard",
                    Price = 500,
                    LastUpdated = today.AddDays(-7)
                },
                new Product
                {
                    Active = true,
                    Category = "Furniture",
                    Name = "Dresser",
                    Price = 250,
                    LastUpdated = today.AddDays(-14)
                },
                new Product
                {
                    Active = true,
                    Category = "Furniture",
                    Name = "Lamp",
                    Price = 50,
                    LastUpdated = today.AddDays(-1)
                },
                new Product
                {
                    Active = true,
                    Category = "Clothing",
                    Name = "Shirt",
                    Price = 10,
                    LastUpdated = today.AddDays(-3)
                },
                new Product
                {
                    Active = true,
                    Category = "Clothing",
                    Name = "Trousers",
                    Price = 15,
                    LastUpdated = today.AddDays(-1)
                },
                new Product
                {
                    Active = true,
                    Category = "Clothing",
                    Name = "Jumper",
                    Price = 20,
                    LastUpdated = today.AddDays(-1)
                },
                new Product
                {
                    Active = true,
                    Category = "Clothing",
                    Name = "Jacket",
                    Price = 20,
                    LastUpdated = today.AddDays(-1)
                },
                new Product
                {
                    Active = true,
                    Category = "Clothing",
                    Name = "Shoes",
                    Price = 200,
                    LastUpdated = today.AddDays(-1)
                },
                new Product
                {
                    Active = true,
                    Category = "Clothing",
                    Name = "Socks",
                    Price = 25,
                    LastUpdated = today.AddDays(-1)
                },
                new Product
                {
                    Active = true,
                    Category = "Clothing",
                    Name = "Underwear",
                    Price = 18,
                    LastUpdated = today.AddDays(-1)
                }
            };
        }
    }
}