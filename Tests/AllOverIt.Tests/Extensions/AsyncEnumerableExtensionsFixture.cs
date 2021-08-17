using AllOverIt.Extensions;
using AllOverIt.Fixture;
using FluentAssertions;
using System;
using System.Collections.Generic;
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
            public void Should_Convert_To_Empty_List_When_Cancelled()
            {
                var cancellationTokenSource = new CancellationTokenSource();
                cancellationTokenSource.Cancel();

                Invoking(async () =>
                    {
                        try
                        {
                            return await GetStrings(CreateMany<string>()).AsListAsync(cancellationTokenSource.Token)
                                .ConfigureAwait(false);
                        }
                        catch (OperationCanceledException)
                        {
                            return null;
                        }
                    })
                    .Should()
                    .BeNull();
            }

            [Fact]
            public void Should_Cancel_Conversion()
            {
                var cancellationTokenSource = new CancellationTokenSource();
                cancellationTokenSource.Cancel();

                Invoking(async () =>
                    {
                        await GetStrings(CreateMany<string>()).AsListAsync(cancellationTokenSource.Token);
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