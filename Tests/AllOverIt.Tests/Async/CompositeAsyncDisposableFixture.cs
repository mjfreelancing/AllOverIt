using AllOverIt.Async;
using AllOverIt.Fixture;
using AllOverIt.Fixture.FakeItEasy;
using FluentAssertions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AllOverIt.Tests.Async
{
#if !NETSTANDARD2_0
    public class CompositeAsyncDisposableFixture : FixtureBase
    {
        public CompositeAsyncDisposableFixture()
        {
            this.UseFakeItEasy();
        }

        public class Constructor : CompositeAsyncDisposableFixture
        {
            [Fact]
            public void Should_Not_Throw()
            {
                Invoking(() =>
                    {
                        _ = new CompositeAsyncDisposable();
                    })
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Add_Disposables()
            {
                var count = 0;
                var disposableFakes = this.CreateManyFakes<IAsyncDisposable>();

                foreach (var fake in disposableFakes)
                {
                    fake
                        .CallsTo(item => item.DisposeAsync())
                        .Invokes(_ => Interlocked.Increment(ref count));
                }

                var disposables = disposableFakes.Select(item => item.FakedObject).ToArray();

                using (new CompositeAsyncDisposable(disposables))
                {
                }

                disposableFakes.Count.Should().NotBe(0);
                count.Should().Be(disposableFakes.Count);
            }
        }

        public class Dispose : CompositeAsyncDisposableFixture
        {
            [Fact]
            public void Should_DisposeAsync_Using_Dispose()
            {
                var count = 0;
                var disposableFakes = this.CreateManyFakes<IAsyncDisposable>();

                foreach (var fake in disposableFakes)
                {
                    fake
                        .CallsTo(item => item.DisposeAsync())
                        .Invokes(_ => Interlocked.Increment(ref count));
                }

                var disposables = disposableFakes.Select(item => item.FakedObject).ToArray();

                var sut = new CompositeAsyncDisposable(disposables);
                sut.Dispose();

                disposableFakes.Count.Should().NotBe(0);
                count.Should().Be(disposableFakes.Count);
            }
        }

        public class DisposeAsync : CompositeAsyncDisposableFixture
        {
            [Fact]
            public async Task Should_DisposeAsync_Using_DisposeAsync()
            {
                var count = 0;
                var disposableFakes = this.CreateManyFakes<IAsyncDisposable>();

                foreach (var fake in disposableFakes)
                {
                    fake
                        .CallsTo(item => item.DisposeAsync())
                        .Invokes(_ => Interlocked.Increment(ref count));
                }

                var disposables = disposableFakes.Select(item => item.FakedObject).ToArray();

                var sut = new CompositeAsyncDisposable(disposables);
                await sut.DisposeAsync();

                disposableFakes.Count.Should().NotBe(0);
                count.Should().Be(disposableFakes.Count);
            }
        }
    }
#endif
}