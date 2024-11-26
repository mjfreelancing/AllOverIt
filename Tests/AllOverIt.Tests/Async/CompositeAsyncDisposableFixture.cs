using AllOverIt.Async;
using AllOverIt.Fixture;
using AllOverIt.Fixture.FakeItEasy;
using FluentAssertions;

namespace AllOverIt.Tests.Async
{
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
                // We could look at the Disposables property but better to actually make sure
                // the disposables added are disosed of, and count them.

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

        public class Disposables : CompositeAsyncDisposableFixture
        {
            [Fact]
            public void Should_Contain_All_Disposables()
            {
                var disposableFakes = this.CreateManyFakes<IAsyncDisposable>();
                var expected = disposableFakes.Select(item => item.FakedObject).ToArray();

                var composite = new CompositeAsyncDisposable(expected);

                composite.Disposables.Should().BeEquivalentTo(expected);
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

            [Fact]
            public void Should_Not_Throw_When_Disposed_More_Than_Once()
            {
                var disposableFakes = this.CreateManyFakes<IAsyncDisposable>();
                var disposables = disposableFakes.Select(item => item.FakedObject).ToArray();
                var sut = new CompositeAsyncDisposable(disposables);

                sut.Dispose();

                Invoking(() =>
                {
                    sut.Dispose();
                })
                .Should()
                .NotThrow();
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

            [Fact]
            public async Task Should_Not_Throw_When_Disposed_More_Than_Once()
            {
                var disposableFakes = this.CreateManyFakes<IAsyncDisposable>();
                var disposables = disposableFakes.Select(item => item.FakedObject).ToArray();
                var sut = new CompositeAsyncDisposable(disposables);

                await sut.DisposeAsync();

                await Invoking(async () =>
                {
                    await sut.DisposeAsync();
                })
                .Should()
                .NotThrowAsync();
            }
        }
    }
}