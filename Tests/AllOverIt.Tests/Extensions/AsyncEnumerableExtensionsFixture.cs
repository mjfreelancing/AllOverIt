using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System.Collections.ObjectModel;

namespace AllOverIt.Tests.Extensions
{
    public class AsyncEnumerableExtensionsFixture : FixtureBase
    {
        private sealed class DummyParent
        {
            public DummyChild[] Children { get; set; }
        }

        private sealed class DummyChild
        {
            public string ChildName { get; set; }
        }

        private readonly DummyParent[] _parentsArray;
        private readonly IAsyncEnumerable<DummyParent> _parents;

        public AsyncEnumerableExtensionsFixture()
        {
            _parentsArray = CreateMany<DummyParent>().ToArray();
            _parents = AsAsyncEnumerable(_parentsArray);
        }

        public class SelectAsync : AsyncEnumerableExtensionsFixture
        {
            [Fact]
            public async Task Should_Throw_When_Items_Null()
            {
                await Invoking(
                        async () =>
                        {
                            IAsyncEnumerable<bool> items = null;

                            await items.SelectAsync((item, token) => Task.FromResult(item)).ToListAsync();
                        })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public async Task Should_Throw_When_Selector_Null()
            {
                await Invoking(
                        async () =>
                        {
                            await _parents
                                .SelectAsync<DummyParent, DummyParent>((Func<DummyParent, CancellationToken, Task<DummyParent>>) null)
                                .ToListAsync();
                        })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
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
                                .SelectAsync((parent, token) => Task.FromResult(parent), cts.Token)
                                .ToListAsync();
                        })
                    .Should()
                    .ThrowAsync<OperationCanceledException>();
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public async Task Should_Iterate_Collection(bool useCancellationToken)
            {
                List<DummyChild> actual;

                if (useCancellationToken)
                {
                    using (var cts = new CancellationTokenSource())
                    {
                        actual = await _parents
                            .SelectAsync((parent, token) => Task.FromResult(parent.Children.First()), cts.Token)
                            .ToListAsync();
                    }
                }
                else
                {
                    actual = await _parents
                        .SelectAsync((parent, token) => Task.FromResult(parent.Children.First()))
                        .ToListAsync();
                }

                var expected = _parentsArray.Select(parent => parent.Children.First());

                expected.Should().BeEquivalentTo(actual);
            }

            private static async IAsyncEnumerable<bool> AsAsyncEnumerable(IEnumerable<bool> items)
            {
                foreach (var item in items)
                {
                    yield return item;
                }

                await Task.CompletedTask;
            }
        }

        public class SelectManyAsync : AsyncEnumerableExtensionsFixture
        {
            [Fact]
            public async Task Should_Throw_When_Null()
            {
                await Invoking(
                        async () =>
                        {
                            IAsyncEnumerable<DummyParent> parents = null;

                            await parents
                                .SelectManyAsync((parent, token) => Task.FromResult(parent.Children.AsEnumerable()))
                                .ToListAsync();
                        })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public async Task Should_Throw_When_Selector_Null()
            {
                await Invoking(
                    async () =>
                    {
                        await _parents
                            .SelectManyAsync((Func<DummyParent, CancellationToken, Task<IEnumerable<DummyChild>>>) null)
                            .ToListAsync();
                    })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
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
                                .SelectManyAsync((item, token) => Task.FromResult(item.Children.AsEnumerable()), cts.Token)
                                .ToListAsync();
                        })
                    .Should()
                    .ThrowAsync<OperationCanceledException>();
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public async Task Should_Iterate_Collection(bool useCancellationToken)
            {
                List<DummyChild> actual;

                if (useCancellationToken)
                {
                    using (var cts = new CancellationTokenSource())
                    {
                        actual = await _parents
                            .SelectManyAsync((item, token) => Task.FromResult(item.Children.AsEnumerable()), cts.Token)
                            .ToListAsync();
                    }
                }
                else
                {
                    actual = await _parents
                        .SelectManyAsync((item, token) => Task.FromResult(item.Children.AsEnumerable()))
                        .ToListAsync();
                }

                var expected = _parentsArray.SelectMany(parent => parent.Children);

                expected.Should().BeEquivalentTo(actual);
            }
        }








        public class SelectManyToArrayAsync : AsyncEnumerableExtensionsFixture
        {
            [Fact]
            public async Task Should_Throw_When_Items_Null()
            {
                await Invoking(
                    async () =>
                    {
                        IAsyncEnumerable<DummyParent> parents = null;

                        await parents.SelectManyToArrayAsync((parent, token) => Task.FromResult(parent.Children.AsEnumerable()));
                    })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public async Task Should_Throw_When_Selector_Null()
            {
                await Invoking(
                    async () =>
                    {
                        await _parents.SelectManyToArrayAsync((Func<DummyParent, CancellationToken, Task<IEnumerable<DummyChild>>>) null);
                    })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
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
                    .Should()
                    .ThrowAsync<OperationCanceledException>();
            }

            [Fact]
            public async Task Should_Project_To_Array()
            {
                var actual = await _parents.SelectManyToArrayAsync((parent, token) => Task.FromResult(parent.Children.AsEnumerable()));

                actual.Should().BeOfType<DummyChild[]>();
            }

            [Fact]
            public async Task Should_Iterate_Collection()
            {
                var actual = await _parents.SelectManyToArrayAsync((parent, token) => Task.FromResult(parent.Children.AsEnumerable()));

                var expected = _parentsArray.SelectMany(parent => parent.Children);

                expected.Should().BeEquivalentTo(actual);
            }
        }

        public class SelectManyToListAsync : AsyncEnumerableExtensionsFixture
        {
            [Fact]
            public async Task Should_Throw_When_Items_Null()
            {
                await Invoking(
                    async () =>
                    {
                        IAsyncEnumerable<DummyParent> parents = null;

                        await parents.SelectManyToListAsync((parent, token) => Task.FromResult(parent.Children.AsEnumerable()));
                    })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public async Task Should_Throw_When_Selector_Null()
            {
                await Invoking(
                    async () =>
                    {
                        await _parents.SelectManyToListAsync((Func<DummyParent, CancellationToken, Task<IEnumerable<DummyChild>>>) null);
                    })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
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
                    .Should()
                    .ThrowAsync<OperationCanceledException>();
            }

            [Fact]
            public async Task Should_Project_To_List()
            {
                var actual = await _parents.SelectManyToListAsync((parent, token) => Task.FromResult(parent.Children.AsEnumerable()));

                actual.Should().BeOfType<List<DummyChild>>();
            }

            [Fact]
            public async Task Should_Iterate_Collection()
            {
                var actual = await _parents.SelectManyToListAsync((parent, token) => Task.FromResult(parent.Children.AsEnumerable()));

                var expected = _parentsArray.SelectMany(parent => parent.Children);

                expected.Should().BeEquivalentTo(actual);
            }
        }

        public class SelectManyToReadOnlyCollectionAsync : AsyncEnumerableExtensionsFixture
        {
            [Fact]
            public async Task Should_Throw_When_Items_Null()
            {
                await Invoking(
                    async () =>
                    {
                        IAsyncEnumerable<DummyParent> parents = null;

                        await parents.SelectManyToReadOnlyCollectionAsync((parent, token) => Task.FromResult(parent.Children.AsEnumerable()));
                    })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public async Task Should_Throw_When_Selector_Null()
            {
                await Invoking(
                    async () =>
                    {
                        await _parents.SelectManyToReadOnlyCollectionAsync((Func<DummyParent, CancellationToken, Task<IEnumerable<DummyChild>>>) null);
                    })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
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
                    .Should()
                    .ThrowAsync<OperationCanceledException>();
            }

            [Fact]
            public async Task Should_Project_To_Collection()
            {
                var actual = await _parents.SelectManyToReadOnlyCollectionAsync((parent, token) => Task.FromResult(parent.Children.AsEnumerable()));

                actual.Should().BeOfType<ReadOnlyCollection<DummyChild>>();
            }

            [Fact]
            public async Task Should_Iterate_Collection()
            {
                var actual = await _parents.SelectManyToReadOnlyCollectionAsync((parent, token) => Task.FromResult(parent.Children.AsEnumerable()));

                var expected = _parentsArray.SelectMany(parent => parent.Children);

                expected.Should().BeEquivalentTo(actual);
            }
        }

        public class ToArrayAsync : AsyncEnumerableExtensionsFixture
        {
            [Fact]
            public async Task Should_Throw_When_Null()
            {
                await Invoking(
                        async () =>
                        {
                            IAsyncEnumerable<bool> items = null;

                            await items.ToArrayAsync();
                        })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public async Task Should_Convert_To_Array()
            {
                var expected = CreateMany<string>();

                var actual = await GetStrings(expected).ToArrayAsync();

                actual.Should().BeOfType<string[]>();

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public async Task Should_Throw_When_Cancelled()
            {
                var cancellationTokenSource = new CancellationTokenSource();
                cancellationTokenSource.Cancel();

                await Invoking(async () =>
                    {
                        await GetStrings(CreateMany<string>()).ToArrayAsync(cancellationTokenSource.Token);
                    })
                    .Should()
                    .ThrowAsync<OperationCanceledException>();
            }
        }

        public class ToListAsync : AsyncEnumerableExtensionsFixture
        {
            [Fact]
            public async Task Should_Throw_When_Null()
            {
                await Invoking(
                        async () =>
                        {
                            IAsyncEnumerable<bool> items = null;

                            await items.ToListAsync();
                        })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public async Task Should_Convert_To_List()
            {
                var expected = CreateMany<string>();

                var actual = await GetStrings(expected).ToListAsync();

                actual.Should().BeOfType<List<string>>();

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public async Task Should_Throw_When_Cancelled()
            {
                var cancellationTokenSource = new CancellationTokenSource();
                cancellationTokenSource.Cancel();

                await Invoking(async () =>
                {
                    await GetStrings(CreateMany<string>()).ToListAsync(cancellationTokenSource.Token);
                })
                    .Should()
                    .ThrowAsync<OperationCanceledException>();
            }
        }

        public class ToReadOnlyCollectionAsync : AsyncEnumerableExtensionsFixture
        {
            [Fact]
            public async Task Should_Throw_When_Null()
            {
                await Invoking(
                    async () =>
                    {
                        IAsyncEnumerable<bool> items = null;

                        await items.ToReadOnlyCollectionAsync();
                    })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public async Task Should_Convert_To_ReadOnlyCollection()
            {
                var expected = CreateMany<string>();

                var actual = await GetStrings(expected).ToReadOnlyCollectionAsync();

                actual.Should().BeOfType<ReadOnlyCollection<string>>();

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public async Task Should_Throw_When_Cancelled()
            {
                var cancellationTokenSource = new CancellationTokenSource();
                cancellationTokenSource.Cancel();

                await Invoking(async () =>
                {
                    await GetStrings(CreateMany<string>()).ToReadOnlyCollectionAsync(cancellationTokenSource.Token);
                })
                    .Should()
                    .ThrowAsync<OperationCanceledException>();
            }
        }

        public class SelectToArrayAsync : AsyncEnumerableExtensionsFixture
        {
            [Fact]
            public async Task Should_Throw_When_Items_Null()
            {
                await Invoking(async () =>
                {
                    _ = await AsyncEnumerableExtensions.SelectToArrayAsync<int, int>((IAsyncEnumerable<int>) null, (item, token) => Task.FromResult(Create<int>()));
                })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public async Task Should_Convert_To_Array()
            {
                var items = CreateMany<string>();
                var expected = items.ToDictionary(item => item, _ => Create<string>());

                var actual = await GetStrings(items).SelectToArrayAsync((item, token) => Task.FromResult(expected[item]));

                actual.Should().BeOfType<string[]>();

                expected.Values.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public async Task Should_Throw_When_Cancelled()
            {
                var cancellationTokenSource = new CancellationTokenSource();
                cancellationTokenSource.Cancel();

                await Invoking(async () =>
                {
                    await GetStrings(CreateMany<string>()).SelectToArrayAsync((item, token) => Task.FromResult(item), cancellationTokenSource.Token);
                })
                    .Should()
                    .ThrowAsync<OperationCanceledException>();
            }
        }

        public class SelectToListAsync : AsyncEnumerableExtensionsFixture
        {
            [Fact]
            public async Task Should_Throw_When_Items_Null()
            {
                await Invoking(async () =>
                {
                    _ = await AsyncEnumerableExtensions.SelectToListAsync<int, int>((IAsyncEnumerable<int>) null, (item, token) => Task.FromResult(Create<int>()));
                })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public async Task Should_Convert_To_List()
            {
                var items = CreateMany<string>();
                var expected = items.ToDictionary(item => item, _ => Create<string>());

                var actual = await GetStrings(items).SelectToListAsync((item, token) => Task.FromResult(expected[item]));

                actual.Should().BeOfType<List<string>>();

                expected.Values.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public async Task Should_Throw_When_Cancelled()
            {
                var cancellationTokenSource = new CancellationTokenSource();
                cancellationTokenSource.Cancel();

                await Invoking(async () =>
                {
                    await GetStrings(CreateMany<string>()).SelectToListAsync((item, token) => Task.FromResult(item), cancellationTokenSource.Token);
                })
                    .Should()
                    .ThrowAsync<OperationCanceledException>();
            }
        }

        public class SelectToReadOnlyCollectionAsync : AsyncEnumerableExtensionsFixture
        {
            [Fact]
            public async Task Should_Throw_When_Items_Null()
            {
                await Invoking(async () =>
                {
                    _ = await AsyncEnumerableExtensions.SelectToReadOnlyCollectionAsync<int, int>((IAsyncEnumerable<int>) null, (item, token) => Task.FromResult(Create<int>()));
                })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public async Task Should_Convert_To_ReadOnlyCollection()
            {
                var items = CreateMany<string>();
                var expected = items.ToDictionary(item => item, _ => Create<string>());

                var actual = await GetStrings(items).SelectToReadOnlyCollectionAsync((item, token) => Task.FromResult(expected[item]));

                actual.Should().BeOfType<ReadOnlyCollection<string>>();

                expected.Values.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public async Task Should_Throw_When_Cancelled()
            {
                var cancellationTokenSource = new CancellationTokenSource();
                cancellationTokenSource.Cancel();

                await Invoking(async () =>
                {
                    await GetStrings(CreateMany<string>()).SelectToReadOnlyCollectionAsync((item, token) => Task.FromResult(item), cancellationTokenSource.Token);
                })
                    .Should()
                    .ThrowAsync<OperationCanceledException>();
            }
        }

        public class SelectAsListAsync : AsyncEnumerableExtensionsFixture
        {
            [Fact]
            public async Task Should_Convert_To_List()
            {
                var items = CreateMany<string>();
                var expected = items.ToDictionary(item => item, _ => Create<string>());

                var actual = await GetStrings(items)
                    .SelectAsListAsync(item => Task.FromResult(expected[item]));

                expected.Values.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public async Task Should_Return_As_List()
            {
                var items = CreateMany<string>();
                var expected = items.ToDictionary(item => item, _ => Create<string>());

                var actual = await GetStrings(items)
                    .SelectAsListAsync(item => Task.FromResult(expected[item]));

                actual.Should().BeAssignableTo(typeof(IList<string>));
            }

            [Fact]
            public async Task Should_Throw_When_Cancelled()
            {
                var cancellationTokenSource = new CancellationTokenSource();
                cancellationTokenSource.Cancel();

                await Invoking(async () =>
                    {
                        await GetStrings(CreateMany<string>())
                            .SelectAsListAsync(item => Task.FromResult(item), cancellationTokenSource.Token);
                    })
                    .Should()
                    .ThrowAsync<OperationCanceledException>();
            }
        }

        public class SelectAsReadOnlyCollectionAsync : AsyncEnumerableExtensionsFixture
        {
            [Fact]
            public async Task Should_Convert_To_List()
            {
                var items = CreateMany<string>();
                var expected = items.ToDictionary(item => item, _ => Create<string>());

                var actual = await GetStrings(items)
                    .SelectAsReadOnlyCollectionAsync(item => Task.FromResult(expected[item]));

                expected.Values.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public async Task Should_Return_As_List()
            {
                var items = CreateMany<string>();
                var expected = items.ToDictionary(item => item, _ => Create<string>());

                var actual = await GetStrings(items)
                    .SelectAsReadOnlyCollectionAsync(item => Task.FromResult(expected[item]));

                actual.Should().BeAssignableTo(typeof(IReadOnlyCollection<string>));
            }

            [Fact]
            public async Task Should_Throw_When_Cancelled()
            {
                var cancellationTokenSource = new CancellationTokenSource();
                cancellationTokenSource.Cancel();

                await Invoking(async () =>
                    {
                        await GetStrings(CreateMany<string>())
                            .SelectAsReadOnlyCollectionAsync(item => Task.FromResult(item), cancellationTokenSource.Token);
                    })
                    .Should()
                    .ThrowAsync<OperationCanceledException>();
            }
        }

        public class SelectAsReadOnlyListAsync : AsyncEnumerableExtensionsFixture
        {
            [Fact]
            public async Task Should_Convert_To_List()
            {
                var items = CreateMany<string>();
                var expected = items.ToDictionary(item => item, _ => Create<string>());

                var actual = await GetStrings(items)
                    .SelectAsReadOnlyListAsync(item => Task.FromResult(expected[item]));

                expected.Values.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public async Task Should_Return_As_List()
            {
                var items = CreateMany<string>();
                var expected = items.ToDictionary(item => item, _ => Create<string>());

                var actual = await GetStrings(items)
                    .SelectAsReadOnlyListAsync(item => Task.FromResult(expected[item]));

                actual.Should().BeAssignableTo(typeof(IReadOnlyList<string>));
            }

            [Fact]
            public async Task Should_Throw_When_Cancelled()
            {
                var cancellationTokenSource = new CancellationTokenSource();
                cancellationTokenSource.Cancel();

                await Invoking(async () =>
                    {
                        await GetStrings(CreateMany<string>())
                            .SelectAsReadOnlyListAsync(item => Task.FromResult(item), cancellationTokenSource.Token);
                    })
                    .Should()
                    .ThrowAsync<OperationCanceledException>();
            }
        }

        public class ForEachAsync_Action : EnumerableExtensionsFixture
        {
            [Fact]
            public async Task Should_Throw_When_Null()
            {
                await Invoking(async () =>
                {
                    IAsyncEnumerable<object> items = null;

                    await items.ForEachAsync((_, _) => { });
                })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public async Task Should_Throw_When_Cancelled()
            {
                var cancelledAtIndex = 0;

                await Invoking(async () =>
                {
                    var cts = new CancellationTokenSource();

                    var values = Create<string>();

                    await AsAsyncEnumerable(values).ForEachAsync((item, index) =>
                    {
                        if (index > 1)
                        {
                            cancelledAtIndex = index;
                            cts.Cancel();
                        }
                    }, cts.Token);
                })
                    .Should()
                    .ThrowAsync<OperationCanceledException>();

                cancelledAtIndex.Should().Be(2);
            }

            [Fact]
            public async Task Should_Iterate_Items_With_Index()
            {
                var values = Create<string>();
                var count = 0;

                await AsAsyncEnumerable(values).ForEachAsync((item, index) =>
                {
                    item.Should().Be(values.ElementAt(index));
                    count++;
                });

                count.Should().Be(values.Length);
            }
        }

        public class ForEachAsync_Func : EnumerableExtensionsFixture
        {
            [Fact]
            public async Task Should_Throw_When_Null()
            {
                await Invoking(async () =>
                {
                    IAsyncEnumerable<object> items = null;

                    await items.ForEachAsync((_, _) => Task.CompletedTask);
                })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public async Task Should_Throw_When_Cancelled()
            {
                var cancelledAtIndex = 0;

                await Invoking(async () =>
                {
                    var cts = new CancellationTokenSource();

                    var values = Create<string>();

                    await AsAsyncEnumerable(values).ForEachAsync((item, index) =>
                    {
                        if (index > 1)
                        {
                            cancelledAtIndex = index;
                            cts.Cancel();
                        }

                        return Task.CompletedTask;
                    }, cts.Token);
                })
                    .Should()
                    .ThrowAsync<OperationCanceledException>();

                cancelledAtIndex.Should().Be(2);
            }

            [Fact]
            public async Task Should_Iterate_Items_With_Index()
            {
                var values = Create<string>();
                var count = 0;

                await AsAsyncEnumerable(values).ForEachAsync((item, index) =>
                {
                    item.Should().Be(values.ElementAt(index));
                    count++;

                    return Task.CompletedTask;
                });

                count.Should().Be(values.Length);
            }
        }

        public class WithIndex : EnumerableExtensionsFixture
        {
            [Fact]
            public async Task Should_Throw_When_Null()
            {
                await Invoking(
                        async () =>
                        {
                            IAsyncEnumerable<object> items = null;

                            // AsListAsync() is required to invoke the method
                            _ = await items.WithIndexAsync().ToListAsync();
                        })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("items");
            }

            [Fact]
            public async Task Should_Throw_When_Cancelled()
            {
                var index = 0;

                await Invoking(async () =>
                {
                    var cts = new CancellationTokenSource();

                    var values = Create<string>();
                    var expectedValues = values.Select((item, index) => (item, index)).AsReadOnlyCollection();

                    await foreach (var (value, idx) in AsAsyncEnumerable(values).WithIndexAsync(cts.Token))
                    {
                        index++;

                        if (index > 1)
                        {
                            cts.Cancel();
                        }
                    }
                })
                    .Should()
                    .ThrowAsync<OperationCanceledException>();

                index.Should().Be(2);
            }

            [Fact]
            public async Task Should_Provide_Item_Index()
            {
                var values = Create<string>();
                var expectedValues = values.Select((item, index) => (item, index)).AsReadOnlyCollection();

                var index = 0;

                await foreach (var (value, idx) in AsAsyncEnumerable(values).WithIndexAsync())
                {
                    var expected = expectedValues.ElementAt(index++);

                    expected
                        .Should()
                        .BeEquivalentTo((value, idx));
                }
            }
        }

        private static async IAsyncEnumerable<TType> AsAsyncEnumerable<TType>(IEnumerable<TType> items)
        {
            foreach (var item in items)
            {
                yield return item;
            }

            await Task.CompletedTask;
        }

        private static async IAsyncEnumerable<string> GetStrings(IEnumerable<string> strings)
        {
            foreach (var item in strings)
            {
                await Task.Yield();

                yield return item;
            }
        }

        private static async IAsyncEnumerable<DummyParent> AsAsyncEnumerable(IEnumerable<DummyParent> items)
        {
            foreach (var item in items)
            {
                yield return item;
            }

            await Task.CompletedTask;
        }
    }
}
