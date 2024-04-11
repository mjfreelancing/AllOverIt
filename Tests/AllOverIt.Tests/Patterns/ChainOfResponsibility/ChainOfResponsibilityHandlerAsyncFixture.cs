using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Patterns.ChainOfResponsibility;
using AllOverIt.Patterns.ChainOfResponsibility.Extensions;
using FluentAssertions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Tests.Patterns.ChainOfResponsibility
{
    public class ChainOfResponsibilityHandlerAsyncFixture : FixtureBase
    {
        private sealed class DummyState
        {
            public int Value { get; set; }
            public int? ProcessedValue { get; set; }
        }

        private class DummyChainOfResponsibility1 : ChainOfResponsibilityHandlerAsync<DummyState, DummyState>
        {
            public override Task<DummyState> HandleAsync(DummyState state, CancellationToken cancellationToken)
            {
                if (state.Value % 3 == 0)
                {
                    state.ProcessedValue = state.Value * 3;
                    return Task.FromResult(state);
                }

                return base.HandleAsync(state, cancellationToken);
            }
        }

        private class DummyChainOfResponsibility2 : ChainOfResponsibilityHandlerAsync<DummyState, DummyState>
        {
            public override Task<DummyState> HandleAsync(DummyState state, CancellationToken cancellationToken)
            {
                if (state.Value % 2 == 0)
                {
                    state.ProcessedValue = state.Value * 2;
                    return Task.FromResult(state);
                }

                return base.HandleAsync(state, cancellationToken);
            }
        }

        public class SetNext : ChainOfResponsibilityHandlerAsyncFixture
        {
            [Fact]
            public void Should_Throw_When_Handler_Null()
            {
                Invoking(() =>
                    {
                        var sut = new DummyChainOfResponsibility1();

                        sut.SetNext(null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("handler");
            }

            [Fact]
            public void Should_Return_Same_Handler()
            {
                var sut1 = new DummyChainOfResponsibility1();
                var sut2 = new DummyChainOfResponsibility2();

                var actual = sut1.SetNext(sut2);

                actual.Should().Be(sut2);
            }
        }

        public class HandleAsync : ChainOfResponsibilityHandlerAsyncFixture
        {
            [Fact]
            public async Task Should_Return_Default_When_End_Of_Chain()
            {
                var sut = new DummyChainOfResponsibility1();

                var state = new DummyState
                {
                    Value = 5
                };

                var actual = await sut.HandleAsync(state, CancellationToken.None);

                actual.Should().Be(default);
            }

            [Fact]
            public async Task Should_Be_Handled_By_First_Handler()
            {
                var handler = new ChainOfResponsibilityHandlerAsync<DummyState, DummyState>[]
                {
                    new DummyChainOfResponsibility1(),
                    new DummyChainOfResponsibility2()
                }.Compose();

                var state = new DummyState
                {
                    Value = 3
                };

                state = await handler.HandleAsync(state, CancellationToken.None);

                state.ProcessedValue.Should().Be(9);
            }

            [Fact]
            public async Task Should_Be_Handled_By_Second_Handler()
            {
                var handler = new ChainOfResponsibilityHandlerAsync<DummyState, DummyState>[]
                {
                    new DummyChainOfResponsibility1(),
                    new DummyChainOfResponsibility2()
                }.Compose();

                var state = new DummyState
                {
                    Value = 2
                };

                state = await handler.HandleAsync(state, CancellationToken.None);

                state.ProcessedValue.Should().Be(4);
            }

            [Fact]
            public async Task Should_Not_Be_Handled()
            {
                var handler = new ChainOfResponsibilityHandlerAsync<DummyState, DummyState>[]
                {
                    new DummyChainOfResponsibility1(),
                    new DummyChainOfResponsibility2()
                }.Compose();

                var state = new DummyState
                {
                    Value = 5
                };

                state = await handler.HandleAsync(state, CancellationToken.None);

                state.Should().Be(null);
            }

            [Fact]
            public async Task Should_Throw_When_Cancelled()
            {
                var handler = new ChainOfResponsibilityHandlerAsync<DummyState, DummyState>[]
                {
                    new DummyChainOfResponsibility1(),
                    new DummyChainOfResponsibility2()
                }.Compose();

                var state = new DummyState
                {
                    Value = 2
                };

                using var cts = new CancellationTokenSource();
                cts.Cancel();

                await Invoking(() => handler.HandleAsync(state, cts.Token))
                    .Should()
                    .ThrowExactlyAsync<OperationCanceledException>();

            }
        }
    }
}