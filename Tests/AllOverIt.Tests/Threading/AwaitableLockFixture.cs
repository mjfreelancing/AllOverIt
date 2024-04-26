using AllOverIt.Fixture;
using AllOverIt.Threading;
using FluentAssertions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Tests.Threading
{
    public class AwaitableLockFixture : FixtureBase
    {
        private readonly AwaitableLock _lock = new();

        public class EnterLockAsync : AwaitableLockFixture
        {
            [Fact]
            public async Task Should_Get_Lock()
            {
                await _lock.EnterLockAsync(CancellationToken.None);

                var success = await _lock.TryEnterLockAsync(10, CancellationToken.None);

                success.Should().BeFalse();

                _lock.ExitLock();
            }

            [Fact]
            public async Task Should_Cancel_Get_Lock()
            {
                var cts = new CancellationTokenSource();
                cts.Cancel();

                await Invoking(async () =>
                {
                    await _lock.EnterLockAsync(cts.Token);
                })
                    .Should()
                    .ThrowAsync<TaskCanceledException>();
            }
        }

        public class TryEnterLockAsync_Milliseconds : AwaitableLockFixture
        {
            private readonly int _timeout = 10;

            [Fact]
            public async Task Should_Try_Get_Lock()
            {
                var success = await _lock.TryEnterLockAsync(_timeout, CancellationToken.None);

                success.Should().BeTrue();

                _lock.ExitLock();
            }

            [Fact]
            public async Task Should_Not_Try_Get_Lock()
            {
                await _lock.EnterLockAsync(CancellationToken.None);

                var success = await _lock.TryEnterLockAsync(_timeout, CancellationToken.None);

                success.Should().BeFalse();

                _lock.ExitLock();
            }

            [Fact]
            public async Task Should_Cancel_Try_Get_Lock()
            {
                var cts = new CancellationTokenSource();
                cts.Cancel();

                await Invoking(async () =>
                {
                    await _lock.TryEnterLockAsync(_timeout, cts.Token);
                })
                    .Should()
                    .ThrowAsync<TaskCanceledException>();
            }
        }

        public class TryEnterLockAsync_TimeSpan : AwaitableLockFixture
        {
            private readonly TimeSpan _timeout = TimeSpan.FromMilliseconds(10);

            [Fact]
            public async Task Should_Try_Get_Lock()
            {
                var success = await _lock.TryEnterLockAsync(_timeout, CancellationToken.None);

                success.Should().BeTrue();

                _lock.ExitLock();
            }

            [Fact]
            public async Task Should_Not_Try_Get_Lock()
            {
                await _lock.EnterLockAsync(CancellationToken.None);

                var success = await _lock.TryEnterLockAsync(_timeout, CancellationToken.None);

                success.Should().BeFalse();

                _lock.ExitLock();
            }

            [Fact]
            public async Task Should_Cancel_Try_Get_Lock()
            {
                var cts = new CancellationTokenSource();
                cts.Cancel();

                await Invoking(async () =>
                {
                    await _lock.TryEnterLockAsync(_timeout, cts.Token);
                })
                    .Should()
                    .ThrowAsync<TaskCanceledException>();
            }
        }

        public class ExitLock : AwaitableLockFixture
        {
            [Fact]
            public async Task Should_Not_Throw_When_Locked()
            {
                await Invoking(async () =>
                {
                    await _lock.EnterLockAsync(CancellationToken.None);

                    _lock.ExitLock();
                })
                  .Should()
                  .NotThrowAsync();
            }

            [Fact]
            public void Should_Throw_When_Not_Locked()
            {
                Invoking(() =>
                {
                    _lock.ExitLock();
                })
                  .Should()
                  .Throw<SynchronizationLockException>()
                  .WithMessage("The lock is not currently held.");
            }
        }

        public class Disose : AwaitableLockFixture
        {
            [Fact]
            public void Should_Not_Throw_When_Disposed_Twice()
            {
                Invoking(() =>
                {
                    _lock.Dispose();
                    _lock.Dispose();
                })
                .Should()
                .NotThrow();
            }
        }
    }
}