using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Patterns.Specification;
using AllOverIt.Tests.Patterns.Specification.Dummies;
using FluentAssertions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AllOverIt.Tests.Extensions
{
    public class EnumerableExtensionsFixture : FixtureBase
    {
        public class AsList : EnumerableExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Null()
            {
                IEnumerable<object> items = null;

                Invoking(() => items.AsList())
                  .Should()
                  .Throw<ArgumentNullException>()
                  .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public void Should_Not_Throw_When_Empty()
            {
                var items = new List<int>();

                Invoking(() => items.AsList())
                  .Should()
                  .NotThrow();
            }

            [Fact]
            public void Should_Not_Return_Same_List()
            {
                // just to check that ToList() does indeed return a copy, unlike AsList()
                var expected = CreateMany<int>();
                var actual = expected.ToList();

                actual.Should().NotBeSameAs(expected);
            }

            [Fact]
            public void Should_Return_Same_List()
            {
                var expected = CreateMany<int>();

                // This test would fail if this call used ToList()
                var actual = expected.AsList();

                actual.Should().BeSameAs(expected);
            }

            [Fact]
            public void Should_Return_Same_ReadOnlyCollection()
            {
                var expected = new ReadOnlyCollection<int>(CreateMany<int>().AsList());

                // This test would fail if this call used ToList()
                var actual = expected.AsList();

                actual.Should().BeSameAs(expected);
            }

            [Fact]
            public void Should_Return_New_List()
            {
                var dictionary = new Dictionary<int, string>
                {
                    {Create<int>(), Create<string>()}, {Create<int>(), Create<string>()}, {Create<int>(), Create<string>()}
                };

                var actual = dictionary.AsList();

                var sameReference = ReferenceEquals(dictionary, actual);

                sameReference.Should().BeFalse();
                actual.Should().BeOfType<List<KeyValuePair<int, string>>>();
            }
        }

        public class AsReadOnlyList : EnumerableExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Null()
            {
                Invoking(
                    () =>
                    {
                        IEnumerable<object> items = null;

                        items.AsReadOnlyList();
                    })
                  .Should()
                  .Throw<ArgumentNullException>()
                  .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public void Should_Not_Throw_When_Empty()
            {
                var items = new List<int>();

                Invoking(() => items.AsReadOnlyList())
                  .Should()
                  .NotThrow();
            }

            [Fact]
            public void Should_Return_Same_List()
            {
                var expected = CreateMany<int>();

                var actual = expected.AsReadOnlyList();

                actual.Should().BeSameAs(expected);
            }

            [Fact]
            public void Should_Return_New_List()
            {
                var dictionary = new Dictionary<int, string>
                {
                    {Create<int>(), Create<string>()}, {Create<int>(), Create<string>()}, {Create<int>(), Create<string>()}
                };

                var actual = dictionary.AsReadOnlyList();

                var sameReference = ReferenceEquals(dictionary, actual);

                sameReference.Should().BeFalse();
                actual.Should().BeOfType<List<KeyValuePair<int, string>>>();
            }
        }

        public class AsReadOnlyCollection : EnumerableExtensionsFixture
        {
            private class DummyCollection : IEnumerable<int>
            {
                private readonly IList<int> _items;

                public DummyCollection(IEnumerable<int> items)
                {
                    _items = new List<int>(items);
                }

                public IEnumerator<int> GetEnumerator()
                {
                    return _items.GetEnumerator();
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return GetEnumerator();
                }
            }

            [Fact]
            public void Should_Throw_When_Null()
            {
                Invoking(
                    () =>
                    {
                        IEnumerable<object> items = null;

                        items.AsReadOnlyCollection();
                    })
                  .Should()
                  .Throw<ArgumentNullException>()
                  .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public void Should_Not_Throw_When_Empty()
            {
                var items = new List<int>();

                Invoking(() => items.AsReadOnlyCollection())
                  .Should()
                  .NotThrow();
            }

            [Fact]
            public void Should_Return_Same_List()
            {
                var expected = CreateMany<int>();

                var actual = expected.AsReadOnlyCollection();

                actual.Should().BeSameAs(expected);
            }

            [Fact]
            public void Should_Return_Same_Dictionary()
            {
                var dictionary = new Dictionary<int, string>
                {
                    {Create<int>(), Create<string>()}, {Create<int>(), Create<string>()}, {Create<int>(), Create<string>()}
                };

                var actual = dictionary.AsReadOnlyCollection();

                var sameReference = ReferenceEquals(dictionary, actual);

                sameReference.Should().BeTrue();
            }

            [Fact]
            public void Should_Return_New_Collection()
            {
                var values = CreateMany<int>();
                var expected = new DummyCollection(values);

                var actual = expected.AsReadOnlyCollection();

                var sameReference = ReferenceEquals(expected, actual);

                sameReference.Should().BeFalse();
                actual.Should().BeOfType<ReadOnlyCollection<int>>();
                actual.Should().HaveCount(values.Count);
            }
        }

        public class SelectAsList : EnumerableExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Null()
            {
                Invoking(
                    () =>
                    {
                        IEnumerable<object> items = null;

                        items.SelectAsList(item => item);
                    })
                  .Should()
                  .Throw<ArgumentNullException>()
                  .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public void Should_Not_Throw_When_Empty()
            {
                var items = new List<int>();

                Invoking(() => items.SelectAsList(item => item))
                  .Should()
                  .NotThrow();
            }

            [Fact]
            public void Should_Return_Projected_List()
            {
                var source = CreateMany<int>();
                var expected = source.Select(item => item * 2);

                var actual = source.SelectAsList(item => item * 2);

                actual.Should().BeEquivalentTo(expected);
            }
        }

        public class SelectAsReadOnlyCollection : EnumerableExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Null()
            {
                Invoking(
                    () =>
                    {
                        IEnumerable<object> items = null;

                        items.SelectAsReadOnlyCollection(item => item);
                    })
                  .Should()
                  .Throw<ArgumentNullException>()
                  .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public void Should_Not_Throw_When_Empty()
            {
                var items = new List<int>();

                Invoking(() => items.SelectAsReadOnlyCollection(item => item))
                  .Should()
                  .NotThrow();
            }

            [Fact]
            public void Should_Return_Projected_List()
            {
                var source = CreateMany<int>();
                var expected = source.Select(item => item * 2);

                var actual = source.SelectAsReadOnlyCollection(item => item * 2);

                actual.Should().BeEquivalentTo(expected);
            }
        }

        public class SelectAsReadOnlyList : EnumerableExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Null()
            {
                Invoking(
                    () =>
                    {
                        IEnumerable<object> items = null;

                        items.SelectAsReadOnlyList(item => item);
                    })
                  .Should()
                  .Throw<ArgumentNullException>()
                  .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public void Should_Not_Throw_When_Empty()
            {
                var items = new List<int>();

                Invoking(() => items.SelectAsReadOnlyList(item => item))
                  .Should()
                  .NotThrow();
            }

            [Fact]
            public void Should_Return_Projected_List()
            {
                var source = CreateMany<int>();
                var expected = source.Select(item => item * 2);

                var actual = source.SelectAsReadOnlyList(item => item * 2);

                actual.Should().BeEquivalentTo(expected);
            }
        }

#if !NETSTANDARD2_0
        public class SelectAsync : EnumerableExtensionsFixture
        {
            [Fact]
            public async Task Should_Throw_When_Null()
            {                
                await Invoking(
                    async () =>
                    {
                        IEnumerable<bool> items = null;

                        await items.SelectAsync(item => Task.FromResult(item)).AsListAsync();
                    })
                  .Should()
                  .ThrowAsync<ArgumentNullException>()
                  .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public async Task Should_Cancel_Iteration()
            {
                var cts = new CancellationTokenSource();
                cts.Cancel();

                await Invoking(
                    async () =>
                    {
                        IEnumerable<bool> items = new[] { true };

                        await items.SelectAsync(item => Task.FromResult(item), cts.Token).AsListAsync();
                    })
                  .Should()
                  .ThrowAsync<OperationCanceledException>();
            }

            [Fact]
            public async Task Should_Iterate_Collection()
            {
                var values = CreateMany<bool>();
                var expected = values.SelectAsReadOnlyCollection(item => !item);

                var actual = await values.SelectAsync(item => Task.FromResult(!item)).AsListAsync();

                actual.Should().BeEquivalentTo(expected);
            }
        }
#endif

        public class IsNullOrEmpty : EnumerableExtensionsFixture
        {
            [Fact]
            public void Should_Return_True_When_Null()
            {
                var actual = EnumerableExtensions.IsNullOrEmpty(null);

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Return_True_When_Array_Empty()
            {
                var actual = EnumerableExtensions.IsNullOrEmpty(new object[] { });

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Return_False_When_Array_Not_Empty()
            {
                var actual = EnumerableExtensions.IsNullOrEmpty(new[] { true, false });

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Return_True_When_String_Empty()
            {
                var value = string.Empty;
                var actual = EnumerableExtensions.IsNullOrEmpty(value);

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Return_False_When_String_Not_Empty()
            {
                var value = Create<string>();
                var actual = EnumerableExtensions.IsNullOrEmpty(value);

                actual.Should().BeFalse();
            }
        }

        public class Batch : EnumerableExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Null()
            {
                Invoking(
                        () =>
                        {
                            IEnumerable<object> items = null;

                            // ToList() is required to invoke the method
                            items.Batch(Create<int>()).ToList();
                        })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public void Should_Return_No_Batches()
            {
                var items = new List<int>();

                var actual = items.Batch(Create<int>());

                actual.Should().BeEmpty();
            }

            [Fact]
            public void Should_Return_Batches_Of_Same_Size()
            {
                var items = CreateMany<int>(10);

                var actual = items.Batch(5).ToList();

                actual.Should().HaveCount(2);
                actual.First().Should().HaveCount(5);
                actual.Skip(1).First().Should().HaveCount(5);
            }

            [Fact]
            public void Should_Return_Batches_Of_Expected_Size()
            {
                var items = CreateMany<int>(9);

                var actual = items.Batch(5).ToList();

                actual.Should().HaveCount(2);
                actual.First().Should().HaveCount(5);
                actual.Skip(1).First().Should().HaveCount(4);
            }
        }

        public class Specifications : SpecificationExtensionsFixture
        {
            private readonly ISpecification<int> _isEven;

            protected Specifications()
            {
                _isEven = new IsEven();
            }

            public class Where : Specifications
            {
                [Fact]
                public void Should_Return_Expected_Result()
                {
                    var expected = new[] { 2, 4, 6, 8, 10 };

                    var actual = Enumerable.Range(1, 10).Where(_isEven);

                    actual.Should().BeEquivalentTo(expected);
                }
            }

            public class Any : Specifications
            {
                [Theory]
                [InlineData(new[] { 1, 2 }, true)]
                [InlineData(new[] { 1, 3 }, false)]
                public void Should_Return_Expected_Result(int[] values, bool expected)
                {
                    var actual = values.Any(_isEven);

                    actual.Should().Be(expected);
                }
            }

            public class All : Specifications
            {
                [Theory]
                [InlineData(new[] { 2, 4, 6 }, true)]
                [InlineData(new[] { 1, 2, 4 }, false)]
                public void Should_Return_Expected_Result(int[] values, bool expected)
                {
                    var actual = values.All(_isEven);

                    actual.Should().Be(expected);
                }
            }

            public class Count : Specifications
            {
                [Theory]
                [InlineData(new[] { 2, 4, 6 }, 3)]
                [InlineData(new[] { 1, 2, 4 }, 2)]
                public void Should_Return_Expected_Result(int[] values, int expected)
                {
                    var actual = values.Count(_isEven);

                    actual.Should().Be(expected);
                }
            }

            public class First : Specifications
            {
                [Theory]
                [InlineData(new[] { 2, 4 }, 2)]
                [InlineData(new[] { -1, 2, 4 }, 2)]
                public void Should_Return_Expected_Result(int[] values, int expected)
                {
                    var actual = values.First(_isEven);

                    actual.Should().Be(expected);
                }
            }

            public class FirstOrDefault : Specifications
            {
                [Theory]
                [InlineData(new int[] { }, 0)]
                [InlineData(new[] { -1, 2, 4 }, 2)]
                public void Should_Return_Expected_Result(int[] values, int expected)
                {
                    var actual = values.FirstOrDefault(_isEven);

                    actual.Should().Be(expected);
                }
            }

            public class Last : Specifications
            {
                [Theory]
                [InlineData(new[] { 2, 4, 1 }, 4)]
                [InlineData(new[] { -1, 2, -4 }, -4)]
                public void Should_Return_Expected_Result(int[] values, int expected)
                {
                    var actual = values.Last(_isEven);

                    actual.Should().Be(expected);
                }
            }

            public class LastOrDefault : Specifications
            {
                [Theory]
                [InlineData(new int[] { }, 0)]
                [InlineData(new[] { -1, 2, 3 }, 2)]
                public void Should_Return_Expected_Result(int[] values, int expected)
                {
                    var actual = values.LastOrDefault(_isEven);

                    actual.Should().Be(expected);
                }
            }

            public class SkipWhile : Specifications
            {
                [Fact]
                public void Should_Return_Expected_Result()
                {
                    var values = Enumerable.Range(1, 10);

                    ISpecification<int> specification = Specification<int>.Create(value => value < 5);
                    var actual = values.SkipWhile(specification).ToList();

                    actual.Should().BeEquivalentTo(new[] { 5, 6, 7, 8, 9, 10 });
                }
            }

            public class TakeWhile : Specifications
            {
                [Fact]
                public void Should_Return_Expected_Result()
                {
                    var values = Enumerable.Range(1, 10);

                    ISpecification<int> specification = Specification<int>.Create(value => value < 5);
                    var actual = values.TakeWhile(specification).ToList();

                    actual.Should().BeEquivalentTo(new[] { 1, 2, 3, 4 });
                }
            }
        }



        
    }
}