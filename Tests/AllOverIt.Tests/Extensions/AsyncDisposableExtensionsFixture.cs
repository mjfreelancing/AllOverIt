using AllOverIt.Async;
using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Patterns.ResourceInitialization;
using FluentAssertions;

namespace AllOverIt.Tests.Extensions
{
    public class AsyncDisposableExtensionsFixture : FixtureBase
    {
        public class DisposeWith : AsyncDisposableExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Null()
            {
                Invoking(() =>
                {
                    var disposable1 = new RaiiAsync(() => { }, () => { return Task.CompletedTask; });

                    _ = AsyncDisposableExtensions.DisposeWith(disposable1, null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("disposables");
            }

            [Fact]
            public async Task Should_Dispose_Disposabls()
            {
                var expected = Create<int>();
                var actual = 0;

                var disposables = new CompositeAsyncDisposable();
                var disposable1 = new RaiiAsync(() => { }, () => { actual = expected; return Task.CompletedTask; });

                // Normally used as: disposable1.DisposeWith(disposables);
                AsyncDisposableExtensions.DisposeWith(disposable1, disposables);

                await disposables.DisposeAsync();

                actual.Should().Be(expected);
            }

            [Fact]
            public async Task Should_Return_Same_Disposable()
            {
                var disposables = new CompositeAsyncDisposable();
                var disposable1 = new RaiiAsync(() => { }, () => { return Task.CompletedTask; });

                // Normally used as: disposable1.DisposeWith(disposables);
                var actual = AsyncDisposableExtensions.DisposeWith(disposable1, disposables);

                actual.Should().Be(disposable1);

                // cleanup only
                await disposables.DisposeAsync();
            }
        }

        public class DisposeAllAsync : AsyncDisposableExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Null()
            {
                Invoking(() =>
                {
                    _ = AsyncDisposableExtensions.DisposeAllAsync(null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("disposables");
            }

            [Fact]
            public async Task Should_Dispose_All_Disposables()
            {
                var expected = CreateMany<int>(3);
                var actual = new[] { 0, 0, 0 };

                var disposable1 = new RaiiAsync(() => { }, () => { actual[0] = expected[0]; return Task.CompletedTask; });
                var disposable2 = new RaiiAsync(() => { }, () => { actual[1] = expected[1]; return Task.CompletedTask; });
                var disposable3 = new RaiiAsync(() => { }, () => { actual[2] = expected[2]; return Task.CompletedTask; });

                var disposables = new[] { disposable1, disposable2, disposable3 };

                await AsyncDisposableExtensions.DisposeAllAsync(disposables);

                actual.Should().BeEquivalentTo(expected);
            }
        }
    }
}