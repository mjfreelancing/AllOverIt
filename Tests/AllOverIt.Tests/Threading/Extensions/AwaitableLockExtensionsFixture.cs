using AllOverIt.Fixture;
using AllOverIt.Threading;
using AllOverIt.Threading.Extensions;

namespace AllOverIt.Tests.Threading.Extensions
{
    public class AwaitableLockExtensionsFixture : FixtureBase
    {
        public class GetLockAsync : AwaitableLockExtensionsFixture
        {
            [Fact]
            public async Task Should_Get_Lock()
            {
                var @lock = new AwaitableLock();

                using (await AwaitableLockExtensions.GetLockAsync(@lock))
                {
                    var success = await @lock.TryEnterLockAsync(10, CancellationToken.None);

                    success.ShouldBeFalse();
                }
            }

            [Fact]
            public async Task Should_Cancel_Get_Lock()
            {
                var cts = new CancellationTokenSource();
                cts.Cancel();

                var @lock = new AwaitableLock();

                await Invoking(async () =>
                {
                    await AwaitableLockExtensions.GetLockAsync(@lock, cts.Token);
                })
                    .ShouldThrowAsync<TaskCanceledException>();
            }

            [Fact]
            public async Task Should_Release_Lock()
            {
                var @lock = new AwaitableLock();

                using (await AwaitableLockExtensions.GetLockAsync(@lock))
                {
                }

                var success = await @lock.TryEnterLockAsync(10, CancellationToken.None);

                success.ShouldBeTrue();

                @lock.ExitLock();
            }
        }
    }
}



