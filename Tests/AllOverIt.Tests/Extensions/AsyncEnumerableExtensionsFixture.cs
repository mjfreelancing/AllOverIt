#if !NETSTANDARD2_0

using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

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

            [Fact]
            public async Task Should_Iterate_Collection()
            {
                var values = CreateMany<bool>();
                var expected = values.SelectAsReadOnlyCollection(item => !item);

                var actual = await AsAsyncEnumerable(values).SelectAsync(item => Task.FromResult(!item)).ToListAsync();

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
                
                var actual = await GetStrings(expected)
                    .ToListAsync()
                    .ConfigureAwait(false);

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public async Task Should_Return_As_List()
            {
                var expected = CreateMany<string>();

                var actual = await GetStrings(expected)
                    .ToListAsync()
                    .ConfigureAwait(false);

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
                            .ToListAsync(cancellationTokenSource.Token)
                            .ConfigureAwait(false);
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
                    .SelectAsListAsync(item => Task.FromResult(expected[item]))
                    .ConfigureAwait(false);

                expected.Values.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public async Task Should_Return_As_List()
            {
                var items = CreateMany<string>();
                var expected = items.ToDictionary(item => item, _ => Create<string>());

                var actual = await GetStrings(items)
                    .SelectAsListAsync(item => Task.FromResult(expected[item]))
                    .ConfigureAwait(false);

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
                            .SelectAsListAsync(item => Task.FromResult(item), cancellationTokenSource.Token)
                            .ConfigureAwait(false);
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
                    .SelectAsReadOnlyCollectionAsync(item => Task.FromResult(expected[item]))
                    .ConfigureAwait(false);

                expected.Values.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public async Task Should_Return_As_List()
            {
                var items = CreateMany<string>();
                var expected = items.ToDictionary(item => item, _ => Create<string>());

                var actual = await GetStrings(items)
                    .SelectAsReadOnlyCollectionAsync(item => Task.FromResult(expected[item]))
                    .ConfigureAwait(false);

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
                            .SelectAsReadOnlyCollectionAsync(item => Task.FromResult(item), cancellationTokenSource.Token)
                            .ConfigureAwait(false);
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
                    .SelectAsReadOnlyListAsync(item => Task.FromResult(expected[item]))
                    .ConfigureAwait(false);

                expected.Values.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public async Task Should_Return_As_List()
            {
                var items = CreateMany<string>();
                var expected = items.ToDictionary(item => item, _ => Create<string>());

                var actual = await GetStrings(items)
                    .SelectAsReadOnlyListAsync(item => Task.FromResult(expected[item]))
                    .ConfigureAwait(false);

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
                            .SelectAsReadOnlyListAsync(item => Task.FromResult(item), cancellationTokenSource.Token)
                            .ConfigureAwait(false);
                    })
                    .Should()
                    .ThrowAsync<OperationCanceledException>();
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
                            IAsyncEnumerable<object> items = null;

                            await items.ForEachAsync((_, _) => Task.CompletedTask);
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

                await AsAsyncEnumerable(values).ForEachAsync(async (item, index) =>
                {
                    await Task.CompletedTask;

                    item.Should().Be(values.ElementAt(index));
                    count++;
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
#endif
