using AllOverIt.Fixture;
using AllOverIt.Fixture.FakeItEasy;
using AllOverIt.Helpers;
using FluentAssertions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using AggregateException = System.AggregateException;

namespace AllOverIt.Tests.Helpers
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

            [Fact]
            public void Should_Report_Exceptions()
            {
                var faultyFakes = this.CreateManyFakes<IAsyncDisposable>(3);

                foreach (var fake in faultyFakes)
                {
                    fake
                        .CallsTo(item => item.DisposeAsync())
                        .Invokes(_ => throw new Exception());
                }
                var goodFakes = this.CreateManyFakes<IAsyncDisposable>(3);

                var disposables = faultyFakes
                    .Concat(goodFakes)
                    .Select(item => item.FakedObject)
                    .ToArray();

                var innerExceptionCount = 0;

                try
                {
                    var sut = new CompositeAsyncDisposable(disposables);
                    sut.Dispose();
                }
                catch (Exception exception)
                {
                    if (exception is AggregateException aggregateException)
                    {
                        innerExceptionCount = aggregateException.InnerExceptions.Count;
                    }
                }

                innerExceptionCount.Should().Be(3);
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
            public async Task Should_Report_Exceptions()
            {
                var faultyFakes = this.CreateManyFakes<IAsyncDisposable>(3);

                foreach (var fake in faultyFakes)
                {
                    fake
                        .CallsTo(item => item.DisposeAsync())
                        .Invokes(_ => throw new Exception());
                }
                var goodFakes = this.CreateManyFakes<IAsyncDisposable>(3);

                var disposables = faultyFakes
                    .Concat(goodFakes)
                    .Select(item => item.FakedObject)
                    .ToArray();

                var innerExceptionCount = 0;

                try
                {
                    var sut = new CompositeAsyncDisposable(disposables);
                    await sut.DisposeAsync();
                }
                catch (Exception exception)
                {
                    if (exception is AggregateException aggregateException)
                    {
                        innerExceptionCount = aggregateException.InnerExceptions.Count;
                    }
                }

                innerExceptionCount.Should().Be(3);
            }
        }
    }
}