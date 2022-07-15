using AllOverIt.Filtering;
using AllOverIt.Filtering.Operations;
using System.Linq;

namespace FilteringDemo
{
    internal sealed class Product
    {
        public string Category { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            var products = GetProducts();


            //IFilter<Product> filter = new Filter<Product>();

            ////filter.By(nameof(Product.Name), Operation.EqualTo, "Chairs");

            //filter.By(entity => entity.Price, Operation.GreaterThan, 15)
            //    .And
            //    .By(entity => entity.Price, Operation.LessThan, 30.0)
            //    .Or
            //    .Group
            //    .By(entity => entity.Category, Operation.EqualTo, "Furniture");

            //var str = filter.ToString();

            //var results = products.Where(filter).ToList();



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