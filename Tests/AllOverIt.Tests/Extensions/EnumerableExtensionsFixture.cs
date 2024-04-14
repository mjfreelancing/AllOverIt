using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Tests.Extensions
{
    public partial class EnumerableExtensionsFixture : FixtureBase
    {
        public class NotAny : EnumerableExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Null()
            {
                IEnumerable<object> items = null;

                Invoking(() => items.NotAny())
                  .Should()
                  .Throw<ArgumentNullException>()
                  .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public void Should_Return_True_When_Empty()
            {
                var items = Enumerable.Empty<object>();

                var actual = items.NotAny();

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Return_False_When_Not_Empty()
            {
                var items = CreateMany<int>();

                var actual = items.NotAny();

                actual.Should().BeFalse();
            }
        }

        public class AsArray : EnumerableExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Null()
            {
                IEnumerable<object> items = null;

                Invoking(() => items.AsArray())
                  .Should()
                  .Throw<ArgumentNullException>()
                  .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public void Should_Not_Throw_When_Empty()
            {
                var items = new List<int>();

                Invoking(() => items.AsArray())
                  .Should()
                  .NotThrow();
            }

            [Fact]
            public void Should_Not_Return_Same_Array()
            {
                var expected = CreateMany<int>();
                var actual = expected.AsArray();

                actual.Should().NotBeSameAs(expected);
            }

            [Fact]
            public void Should_Return_Same_Array()
            {
                var expected = CreateMany<int>().ToArray();

                var actual = expected.AsArray();

                actual.Should().BeSameAs(expected);
            }

            [Fact]
            public void Should_Return_New_Array()
            {
                var dictionary = new Dictionary<int, string>
                {
                    {Create<int>(), Create<string>()},
                    {Create<int>(), Create<string>()},
                    {Create<int>(), Create<string>()}
                };

                var actual = dictionary.AsArray();

                var sameReference = ReferenceEquals(dictionary, actual);

                sameReference.Should().BeFalse();
                actual.Should().BeOfType<KeyValuePair<int, string>[]>();
            }
        }

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

        public class SelectAsync : EnumerableExtensionsFixture
        {
            [Fact]
            public async Task Should_Throw_When_Null()
            {
                await Invoking(
                    async () =>
                    {
                        IEnumerable<bool> items = null;

                        await items.SelectAsync(item => Task.FromResult(item)).ToListAsync();
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

                        await items.SelectAsync(item => Task.FromResult(item), cts.Token).ToListAsync();
                    })
                  .Should()
                  .ThrowAsync<OperationCanceledException>();
            }

            [Fact]
            public async Task Should_Iterate_Collection()
            {
                var values = CreateMany<bool>();
                var expected = values.SelectAsReadOnlyCollection(item => !item);

                var actual = await values.SelectAsync(item => Task.FromResult(!item)).ToListAsync();

                expected.Should().BeEquivalentTo(actual);
            }
        }

        public class SelectToArray : EnumerableExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Null()
            {
                Invoking(
                    () =>
                    {
                        IEnumerable<object> items = null;

                        items.SelectToArray(item => item);
                    })
                  .Should()
                  .Throw<ArgumentNullException>()
                  .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public void Should_Not_Throw_When_Empty()
            {
                var items = new List<int>();

                Invoking(() => items.SelectToArray(item => item))
                  .Should()
                  .NotThrow();
            }

            [Fact]
            public void Should_Return_Projected_List()
            {
                var source = CreateMany<int>();
                var expected = source.Select(item => item * 2);

                var actual = source.SelectToArray(item => item * 2);

                expected.Should().BeEquivalentTo(actual);
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

                expected.Should().BeEquivalentTo(actual);
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

                expected.Should().BeEquivalentTo(actual);
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

                expected.Should().BeEquivalentTo(actual);
            }
        }

        public class SelectToList : EnumerableExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Null()
            {
                Invoking(
                    () =>
                    {
                        IEnumerable<object> items = null;

                        items.SelectToList(item => item);
                    })
                  .Should()
                  .Throw<ArgumentNullException>()
                  .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public void Should_Not_Throw_When_Empty()
            {
                var items = new List<int>();

                Invoking(() => items.SelectToList(item => item))
                  .Should()
                  .NotThrow();
            }

            [Fact]
            public void Should_Return_Projected_List()
            {
                var source = CreateMany<int>();
                var expected = source.Select(item => item * 2);

                var actual = source.SelectToList(item => item * 2);

                expected.Should().BeEquivalentTo(actual);
            }
        }

        public class SelectToReadOnlyCollection : EnumerableExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Null()
            {
                Invoking(
                    () =>
                    {
                        IEnumerable<object> items = null;

                        items.SelectToReadOnlyCollection(item => item);
                    })
                  .Should()
                  .Throw<ArgumentNullException>()
                  .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public void Should_Not_Throw_When_Empty()
            {
                var items = new List<int>();

                Invoking(() => items.SelectToReadOnlyCollection(item => item))
                  .Should()
                  .NotThrow();
            }

            [Fact]
            public void Should_Return_Projected_List()
            {
                var source = CreateMany<int>();
                var expected = source.Select(item => item * 2);

                var actual = source.SelectToReadOnlyCollection(item => item * 2);

                expected.Should().BeEquivalentTo(actual);
            }
        }

        public class SelectToArrayAsync : EnumerableExtensionsFixture
        {
            [Fact]
            public async Task Should_Throw_When_Null()
            {
                await Invoking(
                    () =>
                    {
                        IEnumerable<object> items = null;

                        return items.SelectToArrayAsync(item => Task.FromResult(item));
                    })
                  .Should()
                  .ThrowAsync<ArgumentNullException>()
                  .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public async Task Should_Not_Throw_When_Empty()
            {
                var items = new List<int>();

                await Invoking(() => items.SelectToArrayAsync(item => Task.FromResult(item)))
                  .Should()
                  .NotThrowAsync();
            }

            [Fact]
            public async Task Should_Return_Projected_List()
            {
                var source = CreateMany<int>();
                var expected = source.Select(item => item * 2);

                var actual = await source.SelectToArrayAsync(item => Task.FromResult(item * 2));

                expected.Should().BeEquivalentTo(actual);
            }
        }

        public class SelectToListAsync : EnumerableExtensionsFixture
        {
            [Fact]
            public async Task Should_Throw_When_Null()
            {
                await Invoking(
                    () =>
                    {
                        IEnumerable<object> items = null;

                        return items.SelectToListAsync(item => Task.FromResult(item));
                    })
                  .Should()
                  .ThrowAsync<ArgumentNullException>()
                  .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public async Task Should_Not_Throw_When_Empty()
            {
                var items = new List<int>();

                await Invoking(() => items.SelectToListAsync(item => Task.FromResult(item)))
                  .Should()
                  .NotThrowAsync();
            }

            [Fact]
            public async Task Should_Return_Projected_List()
            {
                var source = CreateMany<int>();
                var expected = source.Select(item => item * 2);

                var actual = await source.SelectToListAsync(item => Task.FromResult(item * 2));

                expected.Should().BeEquivalentTo(actual);
            }
        }

        public class SelectAsReadOnlyCollectionAsync : EnumerableExtensionsFixture
        {
            [Fact]
            public async Task Should_Throw_When_Null()
            {
                await Invoking(
                    () =>
                    {
                        IEnumerable<object> items = null;

                        return items.SelectAsReadOnlyCollectionAsync(item => Task.FromResult(item));
                    })
                  .Should()
                  .ThrowAsync<ArgumentNullException>()
                  .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public async Task Should_Not_Throw_When_Empty()
            {
                var items = new List<int>();

                await Invoking(() => items.SelectAsReadOnlyCollectionAsync(item => Task.FromResult(item)))
                  .Should()
                  .NotThrowAsync();
            }

            [Fact]
            public async Task Should_Return_Projected_List()
            {
                var source = CreateMany<int>();
                var expected = source.Select(item => item * 2);

                var actual = await source.SelectAsReadOnlyCollectionAsync(item => Task.FromResult(item * 2));

                expected.Should().BeEquivalentTo(actual);
            }
        }

        public class SelectAsReadOnlyListAsync : EnumerableExtensionsFixture
        {
            [Fact]
            public async Task Should_Throw_When_Null()
            {
                await Invoking(
                    () =>
                    {
                        IEnumerable<object> items = null;

                        return items.SelectAsReadOnlyListAsync(item => Task.FromResult(item));
                    })
                  .Should()
                  .ThrowAsync<ArgumentNullException>()
                  .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public async Task Should_Not_Throw_When_Empty()
            {
                var items = new List<int>();

                await Invoking(() => items.SelectAsReadOnlyListAsync(item => Task.FromResult(item)))
                  .Should()
                  .NotThrowAsync();
            }

            [Fact]
            public async Task Should_Return_Projected_List()
            {
                var source = CreateMany<int>();
                var expected = source.Select(item => item * 2);

                var actual = await source.SelectAsReadOnlyListAsync(item => Task.FromResult(item * 2));

                expected.Should().BeEquivalentTo(actual);
            }
        }

        public class SelectToReadOnlyCollectionAsync : EnumerableExtensionsFixture
        {
            [Fact]
            public async Task Should_Throw_When_Null()
            {
                await Invoking(
                    () =>
                    {
                        IEnumerable<object> items = null;

                        return items.SelectToReadOnlyCollectionAsync(item => Task.FromResult(item));
                    })
                  .Should()
                  .ThrowAsync<ArgumentNullException>()
                  .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public async Task Should_Not_Throw_When_Empty()
            {
                var items = new List<int>();

                await Invoking(() => items.SelectToReadOnlyCollectionAsync(item => Task.FromResult(item)))
                  .Should()
                  .NotThrowAsync();
            }

            [Fact]
            public async Task Should_Return_Projected_List()
            {
                var source = CreateMany<int>();
                var expected = source.Select(item => item * 2);

                var actual = await source.SelectToReadOnlyCollectionAsync(item => Task.FromResult(item * 2));

                expected.Should().BeEquivalentTo(actual);
            }
        }

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
                var actual = EnumerableExtensions.IsNullOrEmpty(Array.Empty<object>());

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

        public class IsNotNullOrEmpty : EnumerableExtensionsFixture
        {
            [Fact]
            public void Should_Return_False_When_Null()
            {
                var actual = EnumerableExtensions.IsNotNullOrEmpty(null);

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Return_False_When_Array_Empty()
            {
                var actual = EnumerableExtensions.IsNotNullOrEmpty(Array.Empty<object>());

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Return_True_When_Array_Not_Empty()
            {
                var actual = EnumerableExtensions.IsNotNullOrEmpty(new[] { true, false });

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Return_False_When_String_Empty()
            {
                var value = string.Empty;
                var actual = EnumerableExtensions.IsNotNullOrEmpty(value);

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Return_True_When_String_Not_Empty()
            {
                var value = Create<string>();
                var actual = EnumerableExtensions.IsNotNullOrEmpty(value);

                actual.Should().BeTrue();
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
                            _ = items.Batch(Create<int>()).ToList();
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

        public class WithIndex : EnumerableExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Null()
            {
                Invoking(
                        () =>
                        {
                            IEnumerable<object> items = null;

                            // ToList() is required to invoke the method
                            _ = items.WithIndex().ToList();
                        })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public void Should_Provide_Item_Index()
            {
                var values = Create<string>();
                var expectedValues = values.Select((item, index) => (item, index)).AsReadOnlyCollection();

                var index = 0;

                foreach (var (value, idx) in values.WithIndex())
                {
                    var expected = expectedValues.ElementAt(index++);

                    expected
                        .Should()
                        .BeEquivalentTo((value, idx));
                }
            }
        }

        public class ForEach : EnumerableExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Null()
            {
                Invoking(
                        () =>
                        {
                            IEnumerable<object> items = null;

                            items.ForEach((_, _) => { });
                        })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public void Should_Iterate_Items_With_Index()
            {
                var values = Create<string>();
                var count = 0;

                values.ForEach((item, index) =>
                {
                    item.Should().Be(values.ElementAt(index));
                    count++;
                });

                count.Should().Be(values.Length);
            }
        }

        public class ForEachAsync : EnumerableExtensionsFixture
        {
            [Fact]
            public async Task Should_Throw_When_Null()
            {
                await Invoking(
                        async () =>
                        {
                            IEnumerable<object> items = null;

                            await items.ForEachAsync((_, _, _) => Task.CompletedTask);
                        })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public async Task Should_Iterate_Items_With_Index()
            {
                var values = Create<string>();
                var count = 0;

                await values.ForEachAsync(async (item, index, _) =>
                {
                    await Task.CompletedTask;

                    item.Should().Be(values.ElementAt(index));
                    count++;
                });

                count.Should().Be(values.Length);
            }

            [Fact]
            public async Task Should_Throw_When_Cancelled()
            {
                await Invoking(
                        async () =>
                        {
                            var cts = new CancellationTokenSource();
                            cts.Cancel();

                            IEnumerable<int> items = new[] { 1, 2, 3 };

                            await items.ForEachAsync((_, _, _) => Task.CompletedTask, cts.Token);
                        })
                    .Should()
                    .ThrowAsync<OperationCanceledException>();
            }

            [Fact]
            public async Task Should_Pass_CancellationToken()
            {
                var cts = new CancellationTokenSource();

                CancellationToken actual = default;

                IEnumerable<int> items = new[] { 1 };

                await items.ForEachAsync((_, _, token) =>
                {
                    actual = token;

                    return Task.CompletedTask;
                }, cts.Token);

                actual.Should().Be(cts.Token);  // Be() and not BeSameAs() as stucts are copied by value
            }
        }

        public class FindMatches : EnumerableExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_First_Null()
            {
                Invoking(() =>
                    {
                        EnumerableExtensions.FindMatches<int, int, int>(null, Enumerable.Empty<int>(), item => item, item => item);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("first");
            }

            [Fact]
            public void Should_Not_Throw_When_First_Empty()
            {
                Invoking(() =>
                    {
                        EnumerableExtensions.FindMatches(Enumerable.Empty<int>(), CreateMany<int>(), item => item, item => item);
                    })
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Throw_When_Second_Null()
            {
                Invoking(() =>
                    {
                        EnumerableExtensions.FindMatches<int, int, int>(Enumerable.Empty<int>(), null, item => item, item => item);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("second");
            }

            [Fact]
            public void Should_Not_Throw_When_Second_Empty()
            {
                Invoking(() =>
                    {
                        EnumerableExtensions.FindMatches(CreateMany<int>(), Enumerable.Empty<int>(), item => item, item => item);
                    })
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Find_No_Matches()
            {
                var first = CreateManyDistinct<int>();
                var firstMax = first.Max();

                var second = first.Select(value => value + firstMax);

                var matches = EnumerableExtensions.FindMatches(first, second, item => item, item => item);

                matches.Should().BeEmpty();
            }

            [Fact]
            public void Should_Find_All_Matches()
            {
                var first = CreateManyDistinct<int>();
                var second = new List<int>(first);

                var matches = EnumerableExtensions.FindMatches(first, second, item => item, item => item);

                first.Should().BeEquivalentTo(matches);
            }

            [Fact]
            public void Should_Find_Some_Matches()
            {
                var first = new[] { 1, 2, 3, 4, 5 };
                var second = new[] { 7, 8, 3, 4, 5 };

                var matches = EnumerableExtensions.FindMatches(first, second, item => item, item => item);

                var expected = new[] { 3, 4, 5 };

                expected.Should().BeEquivalentTo(matches);
            }
        }

        public class HasDistinctGrouping : EnumerableExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Null()
            {
                Invoking(() =>
                {
                    EnumerableExtensions.HasDistinctGrouping<string, string>(null, item => item);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("source");
            }

            [Fact]
            public void Should_Return_True_When_All_Keys_Distinct()
            {
                var items = CreateMany<string>();

                var actual = items.HasDistinctGrouping(item => item);

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Return_False_When_All_Keys_Not_Distinct()
            {
                var duplicate = Create<string>();
                var items = CreateMany<string>().Concat(new[] { duplicate, duplicate }).ToList();

                var actual = items.HasDistinctGrouping(item => item);

                actual.Should().BeFalse();
            }
        }
    }
}