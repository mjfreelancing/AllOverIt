using AllOverIt.Fixture;
using AllOverIt.Threading;
using AllOverIt.Threading.Extensions;

namespace AllOverIt.Tests.Threading.Extensions
{
    public class ReadWriteLockExtensionsFixture : FixtureBase
    {
        public class GetReadLock : ReadWriteLockExtensionsFixture
        {
            [Fact]
            public void Should_Get_Non_Upgradeable_Read_Lock()
            {
                var @lock = new ReadWriteLock();

                using (ReadWriteLockExtensions.GetReadLock(@lock, false))
                {
                    Invoking(() =>
                    {
                        @lock.EnterWriteLock();
                    })
                   .ShouldThrow<LockRecursionException>()
                   .WithMessage("Write lock may not be acquired with read lock held.*");
                }
            }

            [Fact]
            public void Should_Get_Upgradeable_Read_Lock()
            {
                var @lock = new ReadWriteLock();

                using (ReadWriteLockExtensions.GetReadLock(@lock, true))
                {
                    Invoking(() =>
                    {
                        @lock.EnterWriteLock();
                        @lock.ExitWriteLock();
                    })
                   .ShouldNotThrow();
                }
            }

            [Fact]
            public void Should_Exit_Read_Lock()
            {
                var @lock = new ReadWriteLock();

                using (ReadWriteLockExtensions.GetReadLock(@lock, false))
                {
                }

                Invoking(() =>
                {
                    @lock.ExitReadLock();
                })
                    .ShouldThrow<SynchronizationLockException>()
                    .WithMessage("The read lock is being released without being held.*");
            }
        }

        public class GetWriteLock : ReadWriteLockExtensionsFixture
        {
            [Fact]
            public void Should_Get_Write_Lock()
            {
                var @lock = new ReadWriteLock();

                using (ReadWriteLockExtensions.GetWriteLock(@lock))
                {
                    // proves the lock was obtained
                    Invoking(() =>
                    {
                        @lock.EnterReadLock(false);
                    })
                        .ShouldThrow<LockRecursionException>()
                        .WithMessage("A read lock may not be acquired with the write lock held in this mode.");
                }
            }

            [Fact]
            public void Should_Exit_Write_Lock()
            {
                var @lock = new ReadWriteLock();

                using (ReadWriteLockExtensions.GetWriteLock(@lock))
                {
                }

                Invoking(() =>
                {
                    @lock.ExitWriteLock();
                })
                    .ShouldThrow<SynchronizationLockException>()
                    .WithMessage("The write lock is being released without being held.*");
            }
        }
    }
}



