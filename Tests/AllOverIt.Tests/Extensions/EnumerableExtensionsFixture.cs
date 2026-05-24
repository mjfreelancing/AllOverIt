using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Shouldly.Extensions;
using Shouldly;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;

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
                  .ShouldThrow<ArgumentNullException>()
                  .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public void Should_Return_True_When_Empty()
            {
                var items = Enumerable.Empty<object>();

                var actual = items.NotAny();

                actual.ShouldBeTrue();
            }

            [Fact]
            public void Should_Return_False_When_Not_Empty()
            {
                var items = CreateMany<int>();

                var actual = items.NotAny();

                actual.ShouldBeFalse();
            }
        }

        public class AsArray : EnumerableExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Null()
            {
                IEnumerable<object> items = null;

                Invoking(() => items.AsArray())
                  .ShouldThrow<ArgumentNullException>()
                  .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public void Should_Not_Throw_When_Empty()
            {
                var items = new List<int>();

                Invoking(() => items.AsArray())
                  .ShouldNotThrow();
            }

            [Fact]
            public void Should_Not_Return_Same_Array()
            {
                var expected = CreateMany<int>();
                var actual = expected.AsArray();

                actual.ShouldNotBeSameAs(expected);
            }

            [Fact]
            public void Should_Return_Same_Array()
            {
                var expected = CreateMany<int>().ToArray();

                var actual = expected.AsArray();

                actual.ShouldBeSameAs(expected);
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

                sameReference.ShouldBeFalse();
                actual.ShouldBeOfType<KeyValuePair<int, string>[]>();
            }
        }

        public class AsList : EnumerableExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Null()
            {
                IEnumerable<object> items = null;

                Invoking(() => items.AsList())
                  .ShouldThrow<ArgumentNullException>()
                  .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public void Should_Not_Throw_When_Empty()
            {
                var items = new List<int>();

                Invoking(() => items.AsList())
                  .ShouldNotThrow();
            }

            [Fact]
            public void Should_Not_Return_Same_List()
            {
                // just to check that ToList() does indeed return a copy, unlike AsList()
                var expected = CreateMany<int>();
                var actual = expected.ToList();

                actual.ShouldNotBeSameAs(expected);
            }

            [Fact]
            public void Should_Return_Same_List()
            {
                var expected = CreateMany<int>();

                // This test would fail if this call used ToList()
                var actual = expected.AsList();

                actual.ShouldBeSameAs(expected);
            }

            [Fact]
            public void Should_Return_Same_ReadOnlyCollection()
            {
                var expected = new ReadOnlyCollection<int>(CreateMany<int>().AsList());

                // This test would fail if this call used ToList()
                var actual = expected.AsList();

                actual.ShouldBeSameAs(expected);
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

                sameReference.ShouldBeFalse();
                actual.ShouldBeOfType<List<KeyValuePair<int, string>>>();
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
                  .ShouldThrow<ArgumentNullException>()
                  .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public void Should_Not_Throw_When_Empty()
            {
                var items = new List<int>();

                Invoking(() => items.AsReadOnlyList())
                  .ShouldNotThrow();
            }

            [Fact]
            public void Should_Return_Same_List()
            {
                var expected = CreateMany<int>();

                var actual = expected.AsReadOnlyList();

                actual.ShouldBeSameAs(expected);
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

                sameReference.ShouldBeFalse();
                actual.ShouldBeAssignableTo<IReadOnlyList<KeyValuePair<int, string>>>();
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
                  .ShouldThrow<ArgumentNullException>()
                  .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public void Should_Not_Throw_When_Empty()
            {
                var items = new List<int>();

                Invoking(() => items.AsReadOnlyCollection())
                  .ShouldNotThrow();
            }

            [Fact]
            public void Should_Return_Same_List()
            {
                var expected = CreateMany<int>();

                var actual = expected.AsReadOnlyCollection();

                actual.ShouldBeSameAs(expected);
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

                sameReference.ShouldBeTrue();
            }

            [Fact]
            public void Should_Return_New_Collection()
            {
                var values = CreateMany<int>();
                var expected = new DummyCollection(values);

                var actual = expected.AsReadOnlyCollection();

                var sameReference = ReferenceEquals(expected, actual);

                sameReference.ShouldBeFalse();
                actual.ShouldBeOfType<ReadOnlyCollection<int>>();
                actual.Count.ShouldBe(values.Count);
            }
        }

        public class SelectAsync : EnumerableExtensionsFixture
        {
            [Fact]
            public async Task Should_Throw_When_Items_Null()
            {
                await Invoking(
                    async () =>
                    {
                        IEnumerable<bool> items = null;

                        await items.SelectAsync((item, token) => Task.FromResult(item)).ToListAsync();
                    })
                  .ShouldThrowAsync<ArgumentNullException>()
                  .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public async Task Should_Throw_When_Selector_Null()
            {
                await Invoking(
                    async () =>
                    {
                        IEnumerable<bool> items = [];

                        await items.SelectAsync((Func<bool, CancellationToken, Task<bool>>)null).ToListAsync();
                    })
                  .ShouldThrowAsync<ArgumentNullException>()
                  .WithNamedMessageWhenNull("selector");
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

                        await items.SelectAsync((item, token) => Task.FromResult(item), cts.Token).ToListAsync();
                    })
                  .ShouldThrowAsync<OperationCanceledException>();
            }

            [Fact]
            public async Task Should_Iterate_Collection()
            {
                var values = CreateMany<bool>();
                var expected = values.SelectToArray(item => !item);

                var actual = await values.SelectAsync((item, token) => Task.FromResult(!item)).ToListAsync();

                actual.ShouldBeEquivalentTo(expected);
            }
        }

        public class SelectManyAsync : EnumerableExtensionsFixture
        {
            private sealed class DummyParent
            {
                public DummyChild[] Children { get; set; }
            }

            private sealed class DummyChild
            {
                public string ChildName { get; set; }
            }

            private readonly DummyParent[] _parents;

            public SelectManyAsync()
            {
                _parents = CreateMany<DummyParent>().ToArray();
            }

            [Fact]
            public async Task Should_Throw_When_Items_Null()
            {
                await Invoking(
                    async () =>
                    {
                        IEnumerable<DummyParent> parents = null;

                        await parents
                            .SelectManyAsync((parent, token) => Task.FromResult(parent.Children.AsEnumerable()))
                            .ToListAsync();
                    })
                  .ShouldThrowAsync<ArgumentNullException>()
                  .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public async Task Should_Throw_When_Selector_Null()
            {
                await Invoking(
                    async () =>
                    {
                        await _parents
                            .SelectManyAsync((Func<DummyParent, CancellationToken, Task<IEnumerable<DummyChild>>>)null)
                            .ToListAsync();
                    })
                  .ShouldThrowAsync<ArgumentNullException>()
                  .WithNamedMessageWhenNull("selector");
            }

            [Fact]
            public async Task Should_Cancel_Iteration()
            {
                var cts = new CancellationTokenSource();
                cts.Cancel();

                await Invoking(
                    async () =>
                    {
                        await _parents
                            .SelectManyAsync((parent, token) => Task.FromResult(parent.Children.AsEnumerable()), cts.Token)
                            .ToListAsync();
                    })
                  .ShouldThrowAsync<OperationCanceledException>();
            }

            [Fact]
            public async Task Should_Iterate_Collection()
            {
                var actual = await _parents
                    .SelectManyAsync((parent, token) => Task.FromResult(parent.Children.AsEnumerable()))
                    .ToListAsync();

                var expected = _parents.SelectMany(parent => parent.Children);

                actual.ShouldBeEquivalentTo(expected);
            }
        }

        public class SelectManyToArray : EnumerableExtensionsFixture
        {
            private sealed class DummyParent
            {
                public DummyChild[] Children { get; set; }
            }

            private sealed class DummyChild
            {
                public string ChildName { get; set; }
            }

            private readonly DummyParent[] _parents;

            public SelectManyToArray()
            {
                _parents = CreateMany<DummyParent>().ToArray();
            }

            [Fact]
            public void Should_Throw_When_Items_Null()
            {
                Invoking(() =>
                {
                    IEnumerable<DummyParent> parents = null;

                    parents.SelectManyToArray(parent => parent.Children.AsEnumerable());
                })
                .ShouldThrow<ArgumentNullException>()
                .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public void Should_Throw_When_Selector_Null()
            {
                Invoking(() =>
                    {
                        _parents.SelectManyToArray((Func<DummyParent, IEnumerable<DummyChild>>)null);
                    })
                    .ShouldThrow<ArgumentNullException>()
                    .WithNamedMessageWhenNull("selector");
            }

            [Fact]
            public void Should_Project_To_Array()
            {
                var actual = _parents.SelectManyToArray(parent => parent.Children);

                actual.ShouldBeOfType<DummyChild[]>();
            }

            [Fact]
            public void Should_Iterate_Collection()
            {
                var actual = _parents.SelectManyToArray(parent => parent.Children);

                var expected = _parents.SelectMany(parent => parent.Children);

                actual.ShouldBeEquivalentTo(expected);
            }
        }

        public class SelectManyToList : EnumerableExtensionsFixture
        {
            private sealed class DummyParent
            {
                public DummyChild[] Children { get; set; }
            }

            private sealed class DummyChild
            {
                public string ChildName { get; set; }
            }

            private readonly DummyParent[] _parents;

            public SelectManyToList()
            {
                _parents = CreateMany<DummyParent>().ToArray();
            }

            [Fact]
            public void Should_Throw_When_Items_Null()
            {
                Invoking(() =>
                {
                    IEnumerable<DummyParent> parents = null;

                    parents.SelectManyToList(parent => parent.Children.AsEnumerable());
                })
                .ShouldThrow<ArgumentNullException>()
                .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public void Should_Throw_When_Selector_Null()
            {
                Invoking(() =>
                {
                    _parents.SelectManyToList((Func<DummyParent, IEnumerable<DummyChild>>)null);
                })
                    .ShouldThrow<ArgumentNullException>()
                    .WithNamedMessageWhenNull("selector");
            }

            [Fact]
            public void Should_Project_To_List()
            {
                var actual = _parents.SelectManyToList(parent => parent.Children);

                actual.ShouldBeOfType<List<DummyChild>>();
            }

            [Fact]
            public void Should_Iterate_Collection()
            {
                var actual = _parents.SelectManyToList(parent => parent.Children);

                var expected = _parents.SelectMany(parent => parent.Children);

                actual.ShouldBeEquivalentTo(expected);
            }
        }

        public class SelectManyToReadOnlyCollection : EnumerableExtensionsFixture
        {
            private sealed class DummyParent
            {
                public DummyChild[] Children { get; set; }
            }

            private sealed class DummyChild
            {
                public string ChildName { get; set; }
            }

            private readonly DummyParent[] _parents;

            public SelectManyToReadOnlyCollection()
            {
                _parents = CreateMany<DummyParent>().ToArray();
            }

            [Fact]
            public void Should_Throw_When_Items_Null()
            {
                Invoking(() =>
                {
                    IEnumerable<DummyParent> parents = null;

                    parents.SelectManyToReadOnlyCollection(parent => parent.Children.AsEnumerable());
                })
                .ShouldThrow<ArgumentNullException>()
                .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public void Should_Throw_When_Selector_Null()
            {
                Invoking(() =>
                {
                    _parents.SelectManyToReadOnlyCollection((Func<DummyParent, IEnumerable<DummyChild>>)null);
                })
                    .ShouldThrow<ArgumentNullException>()
                    .WithNamedMessageWhenNull("selector");
            }

            [Fact]
            public void Should_Project_To_Collection()
            {
                var actual = _parents.SelectManyToReadOnlyCollection(parent => parent.Children);

                actual.ShouldBeOfType<ReadOnlyCollection<DummyChild>>();
            }

            [Fact]
            public void Should_Iterate_Collection()
            {
                var actual = _parents.SelectManyToReadOnlyCollection(parent => parent.Children);

                var expected = _parents.SelectMany(parent => parent.Children);

                actual.ShouldBeEquivalentTo(expected);
            }
        }








        public class SelectManyToArrayAsync : EnumerableExtensionsFixture
        {
            private sealed class DummyParent
            {
                public DummyChild[] Children { get; set; }
            }

            private sealed class DummyChild
            {
                public string ChildName { get; set; }
            }

            private readonly DummyParent[] _parents;

            public SelectManyToArrayAsync()
            {
                _parents = CreateMany<DummyParent>().ToArray();
            }

            [Fact]
            public async Task Should_Throw_When_Items_Null()
            {
                await Invoking(
                    async () =>
                    {
                        IEnumerable<DummyParent> parents = null;

                        await parents.SelectManyToArrayAsync((parent, token) => Task.FromResult(parent.Children.AsEnumerable()));
                    })
                    .ShouldThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public async Task Should_Throw_When_Selector_Null()
            {
                await Invoking(
                    async () =>
                    {
                        await _parents.SelectManyToArrayAsync((Func<DummyParent, CancellationToken, Task<IEnumerable<DummyChild>>>)null);
                    })
                    .ShouldThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("selector");
            }

            [Fact]
            public async Task Should_Cancel_Iteration()
            {
                var cts = new CancellationTokenSource();
                cts.Cancel();

                await Invoking(
                    async () =>
                    {
                        await _parents.SelectManyToArrayAsync((parent, token) => Task.FromResult(parent.Children.AsEnumerable()), cts.Token);
                    })
                    .ShouldThrowAsync<OperationCanceledException>();
            }

            [Fact]
            public async Task Should_Project_To_Array()
            {
                var actual = await _parents.SelectManyToArrayAsync((parent, token) => Task.FromResult(parent.Children.AsEnumerable()));

                actual.ShouldBeOfType<DummyChild[]>();
            }

            [Fact]
            public async Task Should_Iterate_Collection()
            {
                var actual = await _parents.SelectManyToArrayAsync((parent, token) => Task.FromResult(parent.Children.AsEnumerable()));

                var expected = _parents.SelectMany(parent => parent.Children);

                actual.ShouldBeEquivalentTo(expected);
            }
        }

        public class SelectManyToListAsync : EnumerableExtensionsFixture
        {
            private sealed class DummyParent
            {
                public DummyChild[] Children { get; set; }
            }

            private sealed class DummyChild
            {
                public string ChildName { get; set; }
            }

            private readonly DummyParent[] _parents;

            public SelectManyToListAsync()
            {
                _parents = CreateMany<DummyParent>().ToArray();
            }

            [Fact]
            public async Task Should_Throw_When_Items_Null()
            {
                await Invoking(
                    async () =>
                    {
                        IEnumerable<DummyParent> parents = null;

                        await parents.SelectManyToListAsync((parent, token) => Task.FromResult(parent.Children.AsEnumerable()));
                    })
                    .ShouldThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public async Task Should_Throw_When_Selector_Null()
            {
                await Invoking(
                    async () =>
                    {
                        await _parents.SelectManyToListAsync((Func<DummyParent, CancellationToken, Task<IEnumerable<DummyChild>>>)null);
                    })
                    .ShouldThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("selector");
            }

            [Fact]
            public async Task Should_Cancel_Iteration()
            {
                var cts = new CancellationTokenSource();
                cts.Cancel();

                await Invoking(
                    async () =>
                    {
                        await _parents.SelectManyToListAsync((parent, token) => Task.FromResult(parent.Children.AsEnumerable()), cts.Token);
                    })
                    .ShouldThrowAsync<OperationCanceledException>();
            }

            [Fact]
            public async Task Should_Project_To_List()
            {
                var actual = await _parents.SelectManyToListAsync((parent, token) => Task.FromResult(parent.Children.AsEnumerable()));

                actual.ShouldBeOfType<List<DummyChild>>();
            }

            [Fact]
            public async Task Should_Iterate_Collection()
            {
                var actual = await _parents.SelectManyToListAsync((parent, token) => Task.FromResult(parent.Children.AsEnumerable()));

                var expected = _parents.SelectMany(parent => parent.Children);

                actual.ShouldBeEquivalentTo(expected);
            }
        }

        public class SelectManyToReadOnlyCollectionAsync : EnumerableExtensionsFixture
        {
            private sealed class DummyParent
            {
                public DummyChild[] Children { get; set; }
            }

            private sealed class DummyChild
            {
                public string ChildName { get; set; }
            }

            private readonly DummyParent[] _parents;

            public SelectManyToReadOnlyCollectionAsync()
            {
                _parents = CreateMany<DummyParent>().ToArray();
            }

            [Fact]
            public async Task Should_Throw_When_Items_Null()
            {
                await Invoking(
                    async () =>
                    {
                        IEnumerable<DummyParent> parents = null;

                        await parents.SelectManyToReadOnlyCollectionAsync((parent, token) => Task.FromResult(parent.Children.AsEnumerable()));
                    })
                    .ShouldThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public async Task Should_Throw_When_Selector_Null()
            {
                await Invoking(
                    async () =>
                    {
                        await _parents.SelectManyToReadOnlyCollectionAsync((Func<DummyParent, CancellationToken, Task<IEnumerable<DummyChild>>>)null);
                    })
                    .ShouldThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("selector");
            }

            [Fact]
            public async Task Should_Cancel_Iteration()
            {
                var cts = new CancellationTokenSource();
                cts.Cancel();

                await Invoking(
                    async () =>
                    {
                        await _parents.SelectManyToReadOnlyCollectionAsync((parent, token) => Task.FromResult(parent.Children.AsEnumerable()), cts.Token);
                    })
                    .ShouldThrowAsync<OperationCanceledException>();
            }

            [Fact]
            public async Task Should_Project_To_Collection()
            {
                var actual = await _parents.SelectManyToReadOnlyCollectionAsync((parent, token) => Task.FromResult(parent.Children.AsEnumerable()));

                actual.ShouldBeOfType<ReadOnlyCollection<DummyChild>>();
            }

            [Fact]
            public async Task Should_Iterate_Collection()
            {
                var actual = await _parents.SelectManyToReadOnlyCollectionAsync((parent, token) => Task.FromResult(parent.Children.AsEnumerable()));

                var expected = _parents.SelectMany(parent => parent.Children);

                actual.ShouldBeEquivalentTo(expected);
            }
        }

        public class SelectAsParallelAsync_DegreeOfParallelism : EnumerableExtensionsFixture
        {
            [Fact]
            public async Task Should_Throw_When_Items_Null()
            {
                await Invoking(
                    async () =>
                    {
                        IEnumerable<bool> items = null;

                        await items.SelectAsParallelAsync((item, token) => Task.FromResult(item), 1).ToListAsync();
                    })
                  .ShouldThrowAsync<ArgumentNullException>()
                  .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public async Task Should_Throw_When_Selector_Null()
            {
                await Invoking(
                    async () =>
                    {
                        IEnumerable<bool> items = [];

                        await items.SelectAsParallelAsync((Func<bool, CancellationToken, Task<bool>>)null, 1).ToListAsync();
                    })
                  .ShouldThrowAsync<ArgumentNullException>()
                  .WithNamedMessageWhenNull("selector");
            }

            [Fact]
            public async Task Should_Throw_When_DegreeOfParallelism_Out_Of_Range()
            {
                await Invoking(
                    async () =>
                    {
                        IEnumerable<bool> items = [];

                        await items.SelectAsParallelAsync((item, token) => Task.FromResult(item), 0).ToListAsync();
                    })
                  .ShouldThrowAsync<ArgumentOutOfRangeException>()
                  .WithMessage("The degree of parallelism must be greater than zero. (Parameter 'degreeOfParallelism')");
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

                        await items.SelectAsParallelAsync((item, token) => Task.FromResult(item), 1, cts.Token).ToListAsync();
                    })
                  .ShouldThrowAsync<OperationCanceledException>();
            }

            [Fact]
            public async Task Should_Iterate_Collection()
            {
                var values = CreateMany<bool>(100);
                var expected = values.SelectToArray(item => !item);

                var actual = await values.SelectAsParallelAsync((item, token) => Task.FromResult(!item), GetWithinRange(1, 4)).ToListAsync();

                actual.ShouldBeEquivalentTo(expected);
            }

            [Fact]
            public async Task Should_Have_Max_Degree_Of_Parallelism()
            {
                var rnd = new Random();
                var degreeOfParallelism = Environment.ProcessorCount;
                var counts = new ConcurrentQueue<int>();
                var counter = 0;

                var actual = await Enumerable.Range(1, 100).SelectAsParallelAsync(async (item, token) =>
                {
                    var count = Interlocked.Increment(ref counter);

                    counts.Enqueue(count);

                    await Task.Delay((int)Math.Floor(rnd.NextDouble() * 25), token);

                    Interlocked.Decrement(ref counter);

                    return item;
                }, degreeOfParallelism).ToListAsync();

                counts.Min().ShouldBeGreaterThan(0);
                counts.Max().ShouldBeGreaterThan(1);
                counts.Max().ShouldBeLessThanOrEqualTo(degreeOfParallelism);
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
                  .ShouldThrow<ArgumentNullException>()
                  .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public void Should_Throw_When_Selector_Null()
            {
                Invoking(
                    () =>
                    {
                        IEnumerable<bool> items = [];

                        items.SelectToArray((Func<bool, bool>)null);
                    })
                  .ShouldThrow<ArgumentNullException>()
                  .WithNamedMessageWhenNull("selector");
            }

            [Fact]
            public void Should_Not_Throw_When_Empty()
            {
                var items = new List<int>();

                Invoking(() => items.SelectToArray(item => item))
                  .ShouldNotThrow();
            }

            [Fact]
            public void Should_Return_Projected_Array()
            {
                var source = CreateMany<int>();
                var expected = source.Select(item => item * 2);

                var actual = source.SelectToArray(item => item * 2);

                actual.ShouldBeEquivalentTo(expected);
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
                  .ShouldThrow<ArgumentNullException>()
                  .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public void Should_Throw_When_Selector_Null()
            {
                Invoking(
                    () =>
                    {
                        IEnumerable<bool> items = [];

                        items.SelectToList((Func<bool, bool>)null);
                    })
                  .ShouldThrow<ArgumentNullException>()
                  .WithNamedMessageWhenNull("selector");
            }

            [Fact]
            public void Should_Not_Throw_When_Empty()
            {
                var items = new List<int>();

                Invoking(() => items.SelectToList(item => item))
                  .ShouldNotThrow();
            }

            [Fact]
            public void Should_Return_Projected_List()
            {
                var source = CreateMany<int>();
                var expected = source.Select(item => item * 2);

                var actual = source.SelectToList(item => item * 2);

                actual.ShouldBeEquivalentTo(expected);
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
                  .ShouldThrow<ArgumentNullException>()
                  .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public void Should_Throw_When_Selector_Null()
            {
                Invoking(
                    () =>
                    {
                        IEnumerable<bool> items = [];

                        items.SelectToReadOnlyCollection((Func<bool, bool>)null);
                    })
                  .ShouldThrow<ArgumentNullException>()
                  .WithNamedMessageWhenNull("selector");
            }

            [Fact]
            public void Should_Not_Throw_When_Empty()
            {
                var items = new List<int>();

                Invoking(() => items.SelectToReadOnlyCollection(item => item))
                  .ShouldNotThrow();
            }

            [Fact]
            public void Should_Return_Projected_Collection()
            {
                var source = CreateMany<int>();
                var expected = source.Select(item => item * 2);

                var actual = source.SelectToReadOnlyCollection(item => item * 2);

                actual.ShouldBeEquivalentTo(expected);
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

                        return items.SelectToArrayAsync((item, token) => Task.FromResult(item));
                    })
                  .ShouldThrowAsync<ArgumentNullException>()
                  .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public async Task Should_Not_Throw_When_Empty()
            {
                var items = new List<int>();

                await Invoking(() => items.SelectToArrayAsync((item, token) => Task.FromResult(item)))
                  .ShouldNotThrowAsync();
            }

            [Fact]
            public async Task Should_Throw_When_Selector_Null()
            {
                await Invoking(
                    () =>
                    {
                        IEnumerable<bool> items = [];

                        return items.SelectToArrayAsync((Func<bool, CancellationToken, Task<bool>>)null);
                    })
                  .ShouldThrowAsync<ArgumentNullException>()
                  .WithNamedMessageWhenNull("selector");
            }

            [Fact]
            public async Task Should_Return_Projected_Array()
            {
                var source = CreateMany<int>();
                var expected = source.Select(item => item * 2);

                var actual = await source.SelectToArrayAsync((item, token) => Task.FromResult(item * 2));

                actual.ShouldBeEquivalentTo(expected);
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

                        return items.SelectToListAsync((item, token) => Task.FromResult(item));
                    })
                  .ShouldThrowAsync<ArgumentNullException>()
                  .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public async Task Should_Not_Throw_When_Empty()
            {
                var items = new List<int>();

                await Invoking(() => items.SelectToListAsync((item, token) => Task.FromResult(item)))
                  .ShouldNotThrowAsync();
            }

            [Fact]
            public async Task Should_Throw_When_Selector_Null()
            {
                await Invoking(
                    () =>
                    {
                        IEnumerable<bool> items = [];

                        return items.SelectToListAsync((Func<bool, CancellationToken, Task<bool>>)null);
                    })
                  .ShouldThrowAsync<ArgumentNullException>()
                  .WithNamedMessageWhenNull("selector");
            }

            [Fact]
            public async Task Should_Return_Projected_List()
            {
                var source = CreateMany<int>();
                var expected = source.Select(item => item * 2);

                var actual = await source.SelectToListAsync((item, token) => Task.FromResult(item * 2));

                actual.ShouldBeEquivalentTo(expected);
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

                        return items.SelectToReadOnlyCollectionAsync((item, token) => Task.FromResult(item));
                    })
                  .ShouldThrowAsync<ArgumentNullException>()
                  .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public async Task Should_Not_Throw_When_Empty()
            {
                var items = new List<int>();

                await Invoking(() => items.SelectToReadOnlyCollectionAsync((item, token) => Task.FromResult(item)))
                  .ShouldNotThrowAsync();
            }

            [Fact]
            public async Task Should_Throw_When_Selector_Null()
            {
                await Invoking(
                    () =>
                    {
                        IEnumerable<bool> items = [];

                        return items.SelectToReadOnlyCollectionAsync((Func<bool, CancellationToken, Task<bool>>)null);
                    })
                  .ShouldThrowAsync<ArgumentNullException>()
                  .WithNamedMessageWhenNull("selector");
            }

            [Fact]
            public async Task Should_Return_Projected_Collection()
            {
                var source = CreateMany<int>();
                var expected = source.Select(item => item * 2);

                var actual = await source.SelectToReadOnlyCollectionAsync((item, token) => Task.FromResult(item * 2));

                actual.ShouldBeEquivalentTo(expected);
            }
        }

        public class IsNullOrEmpty : EnumerableExtensionsFixture
        {
            [Fact]
            public void Should_Return_True_When_Null()
            {
                var actual = EnumerableExtensions.IsNullOrEmpty(null);

                actual.ShouldBeTrue();
            }

            [Fact]
            public void Should_Return_True_When_Array_Empty()
            {
                var actual = EnumerableExtensions.IsNullOrEmpty(Array.Empty<object>());

                actual.ShouldBeTrue();
            }

            [Fact]
            public void Should_Return_False_When_Array_Not_Empty()
            {
                var actual = EnumerableExtensions.IsNullOrEmpty(new[] { true, false });

                actual.ShouldBeFalse();
            }

            [Fact]
            public void Should_Return_True_When_String_Empty()
            {
                var value = string.Empty;
                var actual = EnumerableExtensions.IsNullOrEmpty(value);

                actual.ShouldBeTrue();
            }

            [Fact]
            public void Should_Return_False_When_String_Not_Empty()
            {
                var value = Create<string>();
                var actual = EnumerableExtensions.IsNullOrEmpty(value);

                actual.ShouldBeFalse();
            }
        }

        public class IsNotNullOrEmpty : EnumerableExtensionsFixture
        {
            [Fact]
            public void Should_Return_False_When_Null()
            {
                var actual = EnumerableExtensions.IsNotNullOrEmpty(null);

                actual.ShouldBeFalse();
            }

            [Fact]
            public void Should_Return_False_When_Array_Empty()
            {
                var actual = EnumerableExtensions.IsNotNullOrEmpty(Array.Empty<object>());

                actual.ShouldBeFalse();
            }

            [Fact]
            public void Should_Return_True_When_Array_Not_Empty()
            {
                var actual = EnumerableExtensions.IsNotNullOrEmpty(new[] { true, false });

                actual.ShouldBeTrue();
            }

            [Fact]
            public void Should_Return_False_When_String_Empty()
            {
                var value = string.Empty;
                var actual = EnumerableExtensions.IsNotNullOrEmpty(value);

                actual.ShouldBeFalse();
            }

            [Fact]
            public void Should_Return_True_When_String_Not_Empty()
            {
                var value = Create<string>();
                var actual = EnumerableExtensions.IsNotNullOrEmpty(value);

                actual.ShouldBeTrue();
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
                    .ShouldThrow<ArgumentNullException>()
                    .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public void Should_Return_No_Batches()
            {
                var items = new List<int>();

                var actual = items.Batch(Create<int>());

                actual.ShouldBeEmpty();
            }

            [Fact]
            public void Should_Return_Batches_Of_Same_Size()
            {
                var items = CreateMany<int>(10);

                var actual = items.Batch(5).ToList();

                actual.Count.ShouldBe(2);
                actual.First().Count().ShouldBe(5);
                actual.Skip(1).First().Count().ShouldBe(5);
            }

            [Fact]
            public void Should_Return_Batches_Of_Expected_Size()
            {
                var items = CreateMany<int>(9);

                var actual = items.Batch(5).ToList();

                actual.Count.ShouldBe(2);
                actual.First().Count().ShouldBe(5);
                actual.Skip(1).First().Count().ShouldBe(4);
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
                    .ShouldThrow<ArgumentNullException>()
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
                        .ShouldBeEquivalentTo((value, idx));
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
                    .ShouldThrow<ArgumentNullException>()
                    .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public void Should_Iterate_Items_With_Index()
            {
                var values = Create<string>();
                var count = 0;

                values.ForEach((item, index) =>
                {
                    item.ShouldBe(values.ElementAt(index));
                    count++;
                });

                count.ShouldBe(values.Length);
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
                    .ShouldThrowAsync<ArgumentNullException>()
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

                    item.ShouldBe(values.ElementAt(index));
                    count++;
                });

                count.ShouldBe(values.Length);
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
                    .ShouldThrowAsync<OperationCanceledException>();
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

                actual.ShouldBe(cts.Token);  // Be() and not BeSameAs() as structs are copied by value
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
                    .ShouldThrow<ArgumentNullException>()
                    .WithNamedMessageWhenNull("first");
            }

            [Fact]
            public void Should_Not_Throw_When_First_Empty()
            {
                Invoking(() =>
                    {
                        EnumerableExtensions.FindMatches(Enumerable.Empty<int>(), CreateMany<int>(), item => item, item => item);
                    })
                    .ShouldNotThrow();
            }

            [Fact]
            public void Should_Throw_When_Second_Null()
            {
                Invoking(() =>
                    {
                        EnumerableExtensions.FindMatches<int, int, int>(Enumerable.Empty<int>(), null, item => item, item => item);
                    })
                    .ShouldThrow<ArgumentNullException>()
                    .WithNamedMessageWhenNull("second");
            }

            [Fact]
            public void Should_Not_Throw_When_Second_Empty()
            {
                Invoking(() =>
                    {
                        EnumerableExtensions.FindMatches(CreateMany<int>(), Enumerable.Empty<int>(), item => item, item => item);
                    })
                    .ShouldNotThrow();
            }

            [Fact]
            public void Should_Find_No_Matches()
            {
                var first = CreateManyDistinct<int>();
                var firstMax = first.Max();

                var second = first.Select(value => value + firstMax);

                var matches = EnumerableExtensions.FindMatches(first, second, item => item, item => item);

                matches.ShouldBeEmpty();
            }

            [Fact]
            public void Should_Find_All_Matches()
            {
                var first = CreateManyDistinct<int>();
                var second = new List<int>(first);

                var matches = EnumerableExtensions.FindMatches(first, second, item => item, item => item);

                first.ShouldBeEquivalentTo(matches);
            }

            [Fact]
            public void Should_Find_Some_Matches()
            {
                var first = new[] { 1, 2, 3, 4, 5 };
                var second = new[] { 7, 8, 3, 4, 5 };

                var matches = EnumerableExtensions.FindMatches(first, second, item => item, item => item);

                var expected = new[] { 3, 4, 5 };

                matches.ShouldBeEquivalentTo(expected);
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
                    .ShouldThrow<ArgumentNullException>()
                    .WithNamedMessageWhenNull("source");
            }

            [Fact]
            public void Should_Return_True_When_All_Keys_Distinct()
            {
                var items = CreateMany<string>();

                var actual = items.HasDistinctGrouping(item => item);

                actual.ShouldBeTrue();
            }

            [Fact]
            public void Should_Return_False_When_All_Keys_Not_Distinct()
            {
                var duplicate = Create<string>();
                var items = CreateMany<string>().Concat(new[] { duplicate, duplicate }).ToList();

                var actual = items.HasDistinctGrouping(item => item);

                actual.ShouldBeFalse();
            }
        }
    }
}








