using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Tests.Extensions
{
    public class AsyncEnumerableExtensionsFixture : FixtureBase
    {
        public class SelectAsync : EnumerableExtensionsFixture
        {
            [Fact]
            public async Task Should_Throw_When_Null()
            {
                await Invoking(
                        async () =>
                        {
                            IAsyncEnumerable<bool> items = null;

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
                            var items = AsAsyncEnumerable(new[] { true });

                            await items.SelectAsync(item => Task.FromResult(item), cts.Token).ToListAsync();
                        })
                    .Should()
                    .ThrowAsync<OperationCanceledException>();
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public async Task Should_Iterate_Collection(bool useCancellationToken)
            {
                var values = CreateMany<bool>();
                var expected = values.SelectAsReadOnlyCollection(item => !item);

                IList<bool> actual;

                if (useCancellationToken)
                {
                    using (var cts = new CancellationTokenSource())
                    {
                        actual = await AsAsyncEnumerable(values).SelectAsync(item => Task.FromResult(!item), cts.Token).ToListAsync();
                    }
                }
                else
                {
                    actual = await AsAsyncEnumerable(values).SelectAsync(item => Task.FromResult(!item)).ToListAsync();
                }

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

        public class SelectManyAsync : EnumerableExtensionsFixture
        {
            [Fact]
            public async Task Should_Throw_When_Null()
            {
                await Invoking(
                        async () =>
                        {
                            IAsyncEnumerable<IEnumerable<IEnumerable<bool>>> items = null;

                            await items.SelectManyAsync(item => item).ToListAsync();
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
                            var items = AsAsyncEnumerable(new[] { new[] { true } });

                            await items.SelectManyAsync(item => item, cts.Token).ToListAsync();
                        })
                    .Should()
                    .ThrowAsync<OperationCanceledException>();
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public async Task Should_Iterate_Collection(bool useCancellationToken)
            {
                var values = new[]
                {
                    CreateMany<bool>(),
                    CreateMany<bool>(),
                    CreateMany<bool>()
                };

                var expected = values
                    .SelectMany(item => item)
                    .AsReadOnlyCollection();

                IList<bool> actual;

                if (useCancellationToken)
                {
                    using (var cts = new CancellationTokenSource())
                    {
                        actual = await AsAsyncEnumerable(values).SelectManyAsync(item => item, cts.Token).ToListAsync();
                    }
                }
                else
                {
                    actual = await AsAsyncEnumerable(values).SelectManyAsync(item => item).ToListAsync();
                }

                expected.Should().BeEquivalentTo(actual);
            }

            private static async IAsyncEnumerable<IEnumerable<bool>> AsAsyncEnumerable(IEnumerable<IEnumerable<bool>> items)
            {
                foreach (var item in items)
                {
                    yield return item;
                }

                await Task.CompletedTask;
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

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public async Task Should_Return_As_List()
            {
                var expected = CreateMany<string>();

                var actual = await GetStrings(expected).ToListAsync();

                actual.Should().BeAssignableTo(typeof(IList<string>));
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

        public class SelectToArrayAsync : AsyncEnumerableExtensionsFixture
        {
            [Fact]
            public async Task Should_Convert_To_Array()
            {
                var items = CreateMany<string>();
                var expected = items.ToDictionary(item => item, _ => Create<string>());

                var actual = await GetStrings(items)
                    .SelectToArrayAsync(item => Task.FromResult(expected[item]));

                expected.Values.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public async Task Should_Return_As_Array()
            {
                var items = CreateMany<string>();
                var expected = items.ToDictionary(item => item, _ => Create<string>());

                var actual = await GetStrings(items)
                    .SelectToArrayAsync(item => Task.FromResult(expected[item]));

                actual.Should().BeAssignableTo(typeof(string[]));
            }

            [Fact]
            public async Task Should_Throw_When_Cancelled()
            {
                var cancellationTokenSource = new CancellationTokenSource();
                cancellationTokenSource.Cancel();

                await Invoking(async () =>
                {
                    await GetStrings(CreateMany<string>())
                        .SelectToArrayAsync(item => Task.FromResult(item), cancellationTokenSource.Token);
                })
                    .Should()
                    .ThrowAsync<OperationCanceledException>();
            }
        }

        public class SelectToListAsync : AsyncEnumerableExtensionsFixture
        {
            [Fact]
            public async Task Should_Convert_To_List()
            {
                var items = CreateMany<string>();
                var expected = items.ToDictionary(item => item, _ => Create<string>());

                var actual = await GetStrings(items)
                    .SelectToListAsync(item => Task.FromResult(expected[item]));

                expected.Values.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public async Task Should_Return_As_List()
            {
                var items = CreateMany<string>();
                var expected = items.ToDictionary(item => item, _ => Create<string>());

                var actual = await GetStrings(items)
                    .SelectToListAsync(item => Task.FromResult(expected[item]));

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
                        .SelectToListAsync(item => Task.FromResult(item), cancellationTokenSource.Token);
                })
                    .Should()
                    .ThrowAsync<OperationCanceledException>();
            }
        }

        public class SelectToReadOnlyCollectionAsync : AsyncEnumerableExtensionsFixture
        {
            [Fact]
            public async Task Should_Convert_To_List()
            {
                var items = CreateMany<string>();
                var expected = items.ToDictionary(item => item, _ => Create<string>());

                var actual = await GetStrings(items)
                    .SelectToReadOnlyCollectionAsync(item => Task.FromResult(expected[item]));

                expected.Values.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public async Task Should_Return_As_List()
            {
                var items = CreateMany<string>();
                var expected = items.ToDictionary(item => item, _ => Create<string>());

                var actual = await GetStrings(items)
                    .SelectToReadOnlyCollectionAsync(item => Task.FromResult(expected[item]));

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
                        .SelectToReadOnlyCollectionAsync(item => Task.FromResult(item), cancellationTokenSource.Token);
                })
                    .Should()
                    .ThrowAsync<OperationCanceledException>();
            }
        }

        public class SelectToReadOnlyListAsync : AsyncEnumerableExtensionsFixture
        {
            [Fact]
            public async Task Should_Convert_To_List()
            {
                var items = CreateMany<string>();
                var expected = items.ToDictionary(item => item, _ => Create<string>());

                var actual = await GetStrings(items)
                    .SelectToReadOnlyListAsync(item => Task.FromResult(expected[item]));

                expected.Values.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public async Task Should_Return_As_List()
            {
                var items = CreateMany<string>();
                var expected = items.ToDictionary(item => item, _ => Create<string>());

                var actual = await GetStrings(items)
                    .SelectToReadOnlyListAsync(item => Task.FromResult(expected[item]));

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
                        .SelectToReadOnlyListAsync(item => Task.FromResult(item), cancellationTokenSource.Token);
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
    }
}
