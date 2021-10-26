using AllOverIt.Extensions;
using AllOverIt.Fixture;
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
        public class AsListAsync : AsyncEnumerableExtensionsFixture
        {
            [Fact]
            public async Task Should_Convert_To_List()
            {
                var expected = CreateMany<string>();
                
                var actual = await GetStrings(expected)
                    .AsListAsync()
                    .ConfigureAwait(false);

                actual.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public async Task Should_Return_As_List()
            {
                var expected = CreateMany<string>();

                var actual = await GetStrings(expected)
                    .AsListAsync()
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
                            .AsListAsync(cancellationTokenSource.Token)
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

                actual.Should().BeEquivalentTo(expected.Values);
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

                actual.Should().BeEquivalentTo(expected.Values);
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

                actual.Should().BeEquivalentTo(expected.Values);
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