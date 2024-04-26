using AllOverIt.Fixture;
using AllOverIt.Threading;
using FluentAssertions;
using System;
using System.Threading;

namespace AllOverIt.Tests.Threading
{
    public class ReadWriteLockFixture : FixtureBase
    {
        public class Constructor_Default : ReadWriteLockFixture
        {
            [Fact]
            public void Should_Throw_When_Thread_Is_Reentrant()
            {
                var @lock = new ReadWriteLock();

                @lock.EnterReadLock(false);

                Invoking(() =>
                {
                    @lock.EnterReadLock(false);
                })
                    .Should()
                    .Throw<LockRecursionException>()
                    .WithMessage("Recursive read lock acquisitions not allowed in this mode.");

                @lock.ExitReadLock();
            }
        }

        public class Constructor_Reentrant_Policy : ReadWriteLockFixture
        {
            [Fact]
            public void Should_Throw_When_Thread_Is_Reentrant()
            {
                var @lock = new ReadWriteLock(LockRecursionPolicy.NoRecursion);

                @lock.EnterReadLock(false);

                Invoking(() =>
                {
                    @lock.EnterReadLock(false);
                })
                    .Should()
                    .Throw<LockRecursionException>()
                    .WithMessage("Recursive read lock acquisitions not allowed in this mode.");
                @lock.ExitReadLock();
            }

            [Fact]
            public void Should_Not_Throw_When_Thread_Is_Reentrant()
            {
                var @lock = new ReadWriteLock(LockRecursionPolicy.SupportsRecursion);

                Invoking(() =>
                {
                    @lock.EnterReadLock(false);
                    @lock.EnterReadLock(false);
                })
                    .Should()
                    .NotThrow();

                @lock.ExitReadLock();
                @lock.ExitReadLock();
            }
        }

        public class EnterReadLock : ReadWriteLockFixture
        {
            [Fact]
            public void Should_Enter_Read_Lock()
            {
                var @lock = new ReadWriteLock(LockRecursionPolicy.NoRecursion);

                @lock.EnterReadLock(false);

                // proves the lock was obtained
                Invoking(() =>
                {
                    @lock.EnterReadLock(false);
                })
                    .Should()
                    .Throw<LockRecursionException>()
                    .WithMessage("Recursive read lock acquisitions not allowed in this mode.");

                @lock.ExitReadLock();
            }

            [Fact]
            public void Should_Be_Upgradeable_Read_Lock()
            {
                var @lock = new ReadWriteLock(LockRecursionPolicy.NoRecursion);

                @lock.EnterReadLock(true);
                @lock.EnterWriteLock();

                @lock.ExitWriteLock();
                @lock.ExitReadLock();
            }

            [Fact]
            public void Should_Throw_When_Not_Upgradeable_Read_Lock()
            {
                var @lock = new ReadWriteLock(LockRecursionPolicy.NoRecursion);

                @lock.EnterReadLock(false);

                Invoking(() =>
                {
                    @lock.EnterWriteLock();
                })
                    .Should()
                    .Throw<LockRecursionException>()
                    .WithMessage("Write lock may not be acquired with read lock held.*");

                @lock.ExitReadLock();
            }
        }

        public class TryEnterReadLock_Milliseconds : ReadWriteLockFixture
        {
            private readonly int _timeout = 10;

            [Fact]
            public void Should_Enter_Read_Lock()
            {
                var @lock = new ReadWriteLock(LockRecursionPolicy.NoRecursion);

                var success = @lock.TryEnterReadLock(false, _timeout);
                success.Should().BeTrue();

                @lock.ExitReadLock();
            }

            [Fact]
            public void Should_Be_Upgradeable_Read_Lock()
            {
                var @lock = new ReadWriteLock(LockRecursionPolicy.NoRecursion);

                var success = @lock.TryEnterReadLock(true, _timeout);
                success.Should().BeTrue();

                success = @lock.TryEnterWriteLock(_timeout);
                success.Should().BeTrue();

                @lock.ExitWriteLock();
                @lock.ExitReadLock();
            }

            [Fact]
            public void Should_Not_Be_Upgradeable_Read_Lock()
            {
                var @lock = new ReadWriteLock(LockRecursionPolicy.NoRecursion);

                var success = @lock.TryEnterReadLock(false, _timeout);
                success.Should().BeTrue();

                Invoking(() =>
                {
                    @lock.EnterWriteLock();
                })
                    .Should()
                    .Throw<LockRecursionException>()
                    .WithMessage("Write lock may not be acquired with read lock held.*");

                @lock.ExitReadLock();
            }
        }

        public class TryEnterReadLock_TimeSpan : ReadWriteLockFixture
        {
            private readonly TimeSpan _timeout = TimeSpan.FromMilliseconds(10);

            [Fact]
            public void Should_Enter_Read_Lock()
            {
                var @lock = new ReadWriteLock(LockRecursionPolicy.NoRecursion);

                var success = @lock.TryEnterReadLock(false, _timeout);
                success.Should().BeTrue();

                @lock.ExitReadLock();
            }

            [Fact]
            public void Should_Be_Upgradeable_Read_Lock()
            {
                var @lock = new ReadWriteLock(LockRecursionPolicy.NoRecursion);

                var success = @lock.TryEnterReadLock(true, _timeout);
                success.Should().BeTrue();

                success = @lock.TryEnterWriteLock(_timeout);
                success.Should().BeTrue();

                @lock.ExitWriteLock();
                @lock.ExitReadLock();
            }

            [Fact]
            public void Should_Not_Be_Upgradeable_Read_Lock()
            {
                var @lock = new ReadWriteLock(LockRecursionPolicy.NoRecursion);

                var success = @lock.TryEnterReadLock(false, _timeout);
                success.Should().BeTrue();

                Invoking(() =>
                {
                    @lock.EnterWriteLock();
                })
                    .Should()
                    .Throw<LockRecursionException>()
                    .WithMessage("Write lock may not be acquired with read lock held.*");

                @lock.ExitReadLock();
            }
        }

        public class TryEnterUpgradeableReadLock_Milliseconds : ReadWriteLockFixture
        {
            private readonly int _timeout = 10;

            [Fact]
            public void Should_Enter_Read_Lock()
            {
                var @lock = new ReadWriteLock(LockRecursionPolicy.NoRecursion);

                var success = @lock.TryEnterUpgradeableReadLock(_timeout);
                success.Should().BeTrue();

                @lock.ExitReadLock();
            }

            [Fact]
            public void Should_Be_Upgradeable_Read_Lock()
            {
                var @lock = new ReadWriteLock(LockRecursionPolicy.NoRecursion);

                var success = @lock.TryEnterUpgradeableReadLock(_timeout);
                success.Should().BeTrue();

                success = @lock.TryEnterWriteLock(_timeout);
                success.Should().BeTrue();

                @lock.ExitWriteLock();
                @lock.ExitReadLock();
            }
        }

        public class TryEnterUpgradeableReadLock_TimeSpan : ReadWriteLockFixture
        {
            private readonly TimeSpan _timeout = TimeSpan.FromMilliseconds(10);

            [Fact]
            public void Should_Enter_Read_Lock()
            {
                var @lock = new ReadWriteLock(LockRecursionPolicy.NoRecursion);

                var success = @lock.TryEnterUpgradeableReadLock(_timeout);
                success.Should().BeTrue();

                @lock.ExitReadLock();
            }

            [Fact]
            public void Should_Be_Upgradeable_Read_Lock()
            {
                var @lock = new ReadWriteLock(LockRecursionPolicy.NoRecursion);

                var success = @lock.TryEnterUpgradeableReadLock(_timeout);
                success.Should().BeTrue();

                success = @lock.TryEnterWriteLock(_timeout);
                success.Should().BeTrue();

                @lock.ExitWriteLock();
                @lock.ExitReadLock();
            }
        }

        public class ExitReadLock : ReadWriteLockFixture
        {
            [Fact]
            public void Should_Throw_When_Lock_Not_Held()
            {
                var @lock = new ReadWriteLock();

                Invoking(() =>
                {
                    @lock.ExitReadLock();
                })
                    .Should()
                    .Throw<SynchronizationLockException>()
                    .WithMessage("The read lock is being released without being held.*");
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Exit_Read_Lock(bool upgradeable)
            {
                var @lock = new ReadWriteLock();

                Invoking(() =>
                {
                    @lock.EnterReadLock(upgradeable);
                    @lock.ExitReadLock();
                })
                    .Should()
                    .NotThrow();
            }
        }

        public class EnterWriteLock : ReadWriteLockFixture
        {
            [Fact]
            public void Should_Enter_Write_Lock()
            {
                var @lock = new ReadWriteLock();

                Invoking(() =>
                {
                    @lock.EnterWriteLock();
                    @lock.ExitWriteLock();
                })
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Throw_When_Thread_Is_Reentrant()
            {
                var @lock = new ReadWriteLock();

                @lock.EnterWriteLock();

                Invoking(() =>
                {
                    @lock.EnterWriteLock();
                })
                      .Should()
                      .Throw<LockRecursionException>()
                      .WithMessage("Recursive write lock acquisitions not allowed in this mode.");

                @lock.ExitWriteLock();
            }

            [Fact]
            public void Should_Not_Throw_When_Thread_Is_Reentrant()
            {
                var @lock = new ReadWriteLock(LockRecursionPolicy.SupportsRecursion);

                Invoking(() =>
                {
                    @lock.EnterWriteLock();
                    @lock.EnterWriteLock();
                })
                    .Should()
                    .NotThrow();

                @lock.ExitWriteLock();
                @lock.ExitWriteLock();
            }

            [Fact]
            public void Should_Throw_When_Not_Upgradeable()
            {
                var @lock = new ReadWriteLock();

                @lock.EnterReadLock(false);

                Invoking(() =>
                {
                    @lock.EnterWriteLock();
                })
                    .Should()
                    .Throw<LockRecursionException>()
                    .WithMessage("Write lock may not be acquired with read lock held.*");

                @lock.ExitReadLock();
            }

            [Fact]
            public void Should_Not_Throw_When_Upgradeable()
            {
                var @lock = new ReadWriteLock();

                Invoking(() =>
                {
                    @lock.EnterReadLock(true);
                    @lock.EnterWriteLock();
                })
                    .Should()
                    .NotThrow();

                @lock.ExitWriteLock();
                @lock.ExitReadLock();
            }
        }

        public class TryEnterWriteLock_Milliseconds : ReadWriteLockFixture
        {
            private readonly int _timeout = 10;

            [Fact]
            public void Should_Enter_Write_Lock()
            {
                var @lock = new ReadWriteLock();

                var success = @lock.TryEnterWriteLock(_timeout);
                success.Should().BeTrue();

                @lock.ExitWriteLock();
            }

            [Fact]
            public void Should_Not_Enter_Write_Lock_When_Reentrant()
            {
                var @lock = new ReadWriteLock(LockRecursionPolicy.NoRecursion);

                var success = @lock.TryEnterWriteLock(_timeout);
                success.Should().BeTrue();

                Invoking(() =>
                {
                    @lock.TryEnterWriteLock(_timeout);
                })
                            .Should()
                            .Throw<LockRecursionException>()
                            .WithMessage("Recursive write lock acquisitions not allowed in this mode.");

                @lock.ExitWriteLock();
            }

            [Fact]
            public void Should_Enter_Write_Lock_When_Reentrant()
            {
                var @lock = new ReadWriteLock(LockRecursionPolicy.SupportsRecursion);

                var success = @lock.TryEnterWriteLock(_timeout);
                success.Should().BeTrue();

                success = @lock.TryEnterWriteLock(_timeout);
                success.Should().BeTrue();

                @lock.ExitWriteLock();
                @lock.ExitWriteLock();
            }
        }

        public class TryEnterWriteLock_TimeSpan : ReadWriteLockFixture
        {
            private readonly TimeSpan _timeout = TimeSpan.FromMilliseconds(10);

            [Fact]
            public void Should_Enter_Write_Lock()
            {
                var @lock = new ReadWriteLock();

                var success = @lock.TryEnterWriteLock(_timeout);
                success.Should().BeTrue();

                @lock.ExitWriteLock();
            }

            [Fact]
            public void Should_Not_Enter_Write_Lock_When_Reentrant()
            {
                var @lock = new ReadWriteLock(LockRecursionPolicy.NoRecursion);

                var success = @lock.TryEnterWriteLock(_timeout);
                success.Should().BeTrue();

                Invoking(() =>
                {
                    @lock.TryEnterWriteLock(_timeout);
                })
                            .Should()
                            .Throw<LockRecursionException>()
                            .WithMessage("Recursive write lock acquisitions not allowed in this mode.");

                @lock.ExitWriteLock();
            }

            [Fact]
            public void Should_Enter_Write_Lock_When_Reentrant()
            {
                var @lock = new ReadWriteLock(LockRecursionPolicy.SupportsRecursion);

                var success = @lock.TryEnterWriteLock(_timeout);
                success.Should().BeTrue();

                success = @lock.TryEnterWriteLock(_timeout);
                success.Should().BeTrue();

                @lock.ExitWriteLock();
                @lock.ExitWriteLock();
            }
        }

        public class ExitWriteLock : ReadWriteLockFixture
        {
            [Fact]
            public void Should_Throw_When_Lock_Not_Held()
            {
                var @lock = new ReadWriteLock();

                Invoking(() =>
                {
                    @lock.ExitWriteLock();
                })
                    .Should()
                    .Throw<SynchronizationLockException>()
                    .WithMessage("The write lock is being released without being held.*");
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Exit_Write_Lock(bool upgradeable)
            {
                var @lock = new ReadWriteLock();

                Invoking(() =>
                {
                    @lock.EnterWriteLock();
                    @lock.ExitWriteLock();
                })
                    .Should()
                    .NotThrow();
            }
        }

        public class Disose : ReadWriteLockFixture
        {
            [Fact]
            public void Should_Not_Throw_When_Disposed_Twice()
            {
                var @lock = new ReadWriteLock();

                Invoking(() =>
                {
                    @lock.Dispose();
                    @lock.Dispose();
                })
                .Should()
                .NotThrow();
            }
        }
    }
}