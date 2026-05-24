using AllOverIt.Fixture;
using AllOverIt.Threading;

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
                    .ShouldThrow<LockRecursionException>()
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
                    .ShouldThrow<LockRecursionException>()
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
                    .ShouldNotThrow();

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
                    .ShouldThrow<LockRecursionException>()
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
                    .ShouldThrow<LockRecursionException>()
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
                success.ShouldBeTrue();

                @lock.ExitReadLock();
            }

            [Fact]
            public void Should_Be_Upgradeable_Read_Lock()
            {
                var @lock = new ReadWriteLock(LockRecursionPolicy.NoRecursion);

                var success = @lock.TryEnterReadLock(true, _timeout);
                success.ShouldBeTrue();

                success = @lock.TryEnterWriteLock(_timeout);
                success.ShouldBeTrue();

                @lock.ExitWriteLock();
                @lock.ExitReadLock();
            }

            [Fact]
            public void Should_Not_Be_Upgradeable_Read_Lock()
            {
                var @lock = new ReadWriteLock(LockRecursionPolicy.NoRecursion);

                var success = @lock.TryEnterReadLock(false, _timeout);
                success.ShouldBeTrue();

                Invoking(() =>
                {
                    @lock.EnterWriteLock();
                })
                    .ShouldThrow<LockRecursionException>()
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
                success.ShouldBeTrue();

                @lock.ExitReadLock();
            }

            [Fact]
            public void Should_Be_Upgradeable_Read_Lock()
            {
                var @lock = new ReadWriteLock(LockRecursionPolicy.NoRecursion);

                var success = @lock.TryEnterReadLock(true, _timeout);
                success.ShouldBeTrue();

                success = @lock.TryEnterWriteLock(_timeout);
                success.ShouldBeTrue();

                @lock.ExitWriteLock();
                @lock.ExitReadLock();
            }

            [Fact]
            public void Should_Not_Be_Upgradeable_Read_Lock()
            {
                var @lock = new ReadWriteLock(LockRecursionPolicy.NoRecursion);

                var success = @lock.TryEnterReadLock(false, _timeout);
                success.ShouldBeTrue();

                Invoking(() =>
                {
                    @lock.EnterWriteLock();
                })
                    .ShouldThrow<LockRecursionException>()
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
                success.ShouldBeTrue();

                @lock.ExitReadLock();
            }

            [Fact]
            public void Should_Be_Upgradeable_Read_Lock()
            {
                var @lock = new ReadWriteLock(LockRecursionPolicy.NoRecursion);

                var success = @lock.TryEnterUpgradeableReadLock(_timeout);
                success.ShouldBeTrue();

                success = @lock.TryEnterWriteLock(_timeout);
                success.ShouldBeTrue();

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
                success.ShouldBeTrue();

                @lock.ExitReadLock();
            }

            [Fact]
            public void Should_Be_Upgradeable_Read_Lock()
            {
                var @lock = new ReadWriteLock(LockRecursionPolicy.NoRecursion);

                var success = @lock.TryEnterUpgradeableReadLock(_timeout);
                success.ShouldBeTrue();

                success = @lock.TryEnterWriteLock(_timeout);
                success.ShouldBeTrue();

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
                    .ShouldThrow<SynchronizationLockException>()
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
                    .ShouldNotThrow();
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
                    .ShouldNotThrow();
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
                      .ShouldThrow<LockRecursionException>()
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
                    .ShouldNotThrow();

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
                    .ShouldThrow<LockRecursionException>()
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
                    .ShouldNotThrow();

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
                success.ShouldBeTrue();

                @lock.ExitWriteLock();
            }

            [Fact]
            public void Should_Not_Enter_Write_Lock_When_Reentrant()
            {
                var @lock = new ReadWriteLock(LockRecursionPolicy.NoRecursion);

                var success = @lock.TryEnterWriteLock(_timeout);
                success.ShouldBeTrue();

                Invoking(() =>
                {
                    @lock.TryEnterWriteLock(_timeout);
                })
                            .ShouldThrow<LockRecursionException>()
                            .WithMessage("Recursive write lock acquisitions not allowed in this mode.");

                @lock.ExitWriteLock();
            }

            [Fact]
            public void Should_Enter_Write_Lock_When_Reentrant()
            {
                var @lock = new ReadWriteLock(LockRecursionPolicy.SupportsRecursion);

                var success = @lock.TryEnterWriteLock(_timeout);
                success.ShouldBeTrue();

                success = @lock.TryEnterWriteLock(_timeout);
                success.ShouldBeTrue();

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
                success.ShouldBeTrue();

                @lock.ExitWriteLock();
            }

            [Fact]
            public void Should_Not_Enter_Write_Lock_When_Reentrant()
            {
                var @lock = new ReadWriteLock(LockRecursionPolicy.NoRecursion);

                var success = @lock.TryEnterWriteLock(_timeout);
                success.ShouldBeTrue();

                Invoking(() =>
                {
                    @lock.TryEnterWriteLock(_timeout);
                })
                            .ShouldThrow<LockRecursionException>()
                            .WithMessage("Recursive write lock acquisitions not allowed in this mode.");

                @lock.ExitWriteLock();
            }

            [Fact]
            public void Should_Enter_Write_Lock_When_Reentrant()
            {
                var @lock = new ReadWriteLock(LockRecursionPolicy.SupportsRecursion);

                var success = @lock.TryEnterWriteLock(_timeout);
                success.ShouldBeTrue();

                success = @lock.TryEnterWriteLock(_timeout);
                success.ShouldBeTrue();

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
                    .ShouldThrow<SynchronizationLockException>()
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
                    .ShouldNotThrow();
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
                .ShouldNotThrow();
            }
        }
    }
}



