using AllOverIt.Expressions;
using AllOverIt.Fixture;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace AllOverIt.Tests.Expressions
{
    public class PredicateBuilderFunctionalFixture : FixtureBase
    {
        private class Product
        {
            public int Id { get; }
            public string Category { get; }
            public bool Promoted { get; }

            public Product(int id, string category, bool promoted)
            {
                Id = id;
                Category = category;
                Promoted = promoted;
            }

            public static Expression<Func<Product, bool>> IsPromoted()
            {
                return product => product.Promoted;
            }

            public static Expression<Func<Product, bool>> IsInCategory(params string[] categories)
            {
                var predicate = PredicateBuilder.False<Product>();

                foreach (var category in categories)
                {
                    var temp = category;    // must capture for the closure
                    predicate = predicate.Or(p => p.Category.Contains(temp));
                }

                return predicate;
            }
        }

        private readonly IList<Product> _products;
        readonly Expression<Func<Product, bool>> _isFoodOrVegetable;
        readonly Expression<Func<Product, bool>> _isPromoted;

        public PredicateBuilderFunctionalFixture()
        {
            _products = new List<Product>
            {
                new Product(1, "Fruit", true),
                new Product(2, "Fruit", false),
                new Product(3, "Vegetable", true),
                new Product(4, "Vegetable", false),
                new Product(5, "Vehicle", true),
                new Product(6, "Vehicle", false)
            };

            _isFoodOrVegetable = Product.IsInCategory("Fruit", "Vegetable");
            _isPromoted = Product.IsPromoted();
        }

        [Fact]
        public void Should_Filter_To_Promoted()
        {
            // expression based queries require AsQueryable
            var filtered = _products
              .AsQueryable()
              .Where(Product.IsPromoted());

            var actual = filtered.Select(item => item.Id);

            actual.Should().BeEquivalentTo(1, 3, 5);
        }

        [Fact]
        public void Should_Compose_Predicates()
        {
            var isPromotedFruitOrVegetable = _isFoodOrVegetable.And(_isPromoted);

            // expression based queries require AsQueryable
            var filtered = _products
              .AsQueryable()
              .Where(isPromotedFruitOrVegetable);

            var actual = filtered.Select(item => item.Id);

            actual.Should().BeEquivalentTo(1, 3);
        }

        [Fact]
        public void Should_Compile_Composed_Predicate()
        {
            var isPromotedFruitOrVegetable = _isFoodOrVegetable.And(_isPromoted);
            var compiledPredicate = isPromotedFruitOrVegetable.Compile();

            var filtered = _products.Where(compiledPredicate);

            var actual = filtered.Select(item => item.Id);

            actual.Should().BeEquivalentTo(1, 3);
        }
    }
}