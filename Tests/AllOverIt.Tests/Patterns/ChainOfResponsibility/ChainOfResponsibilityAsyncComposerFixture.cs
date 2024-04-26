using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Patterns.ChainOfResponsibility;
using FluentAssertions;

namespace AllOverIt.Tests.Patterns.ChainOfResponsibility
{
    public class ChainOfResponsibilityAsyncComposerFixture : FixtureBase
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

        public class Constructor : ChainOfResponsibilityAsyncComposerFixture
        {
            [Fact]
            public void Should_Throw_When_Handlers_Null()
            {
                Invoking(() =>
                    {
                        IEnumerable<IChainOfResponsibilityHandlerAsync<DummyState, DummyState>> handlers = null;

                        _ = new ChainOfResponsibilityAsyncComposer<DummyState, DummyState>(handlers);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("handlers");
            }

            [Fact]
            public void Should_Throw_When_Handlers_Empty()
            {
                Invoking(() =>
                    {
                        var handlers = new List<IChainOfResponsibilityHandlerAsync<DummyState, DummyState>>();

                        _ = new ChainOfResponsibilityAsyncComposer<DummyState, DummyState>(handlers);
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("handlers");
            }

            [Fact]
            public async Task Should_Begin_With_First_Handler()
            {
                var handlers = new IChainOfResponsibilityHandlerAsync<DummyState, DummyState>[]
                {
                    new DummyChainOfResponsibility1(),
                    new DummyChainOfResponsibility2()
                };

                var composer = new ChainOfResponsibilityAsyncComposer<DummyState, DummyState>(handlers);

                var state = new DummyState
                {
                    Value = 3
                };

                state = await composer.HandleAsync(state, CancellationToken.None);

                state.ProcessedValue.Should().Be(9);
            }

            [Fact]
            public async Task Should_Return_Expected_State()
            {
                var handlers = new IChainOfResponsibilityHandlerAsync<DummyState, DummyState>[]
                {
                    new DummyChainOfResponsibility1(),
                    new DummyChainOfResponsibility2()
                };

                var composer = new ChainOfResponsibilityAsyncComposer<DummyState, DummyState>(handlers);

                var state = new DummyState
                {
                    Value = 2
                };

                state = await composer.HandleAsync(state, CancellationToken.None);

                state.ProcessedValue.Should().Be(4);
            }

            [Fact]
            public async Task Should_Return_Default_State_When_Unhandled()
            {
                var handlers = new IChainOfResponsibilityHandlerAsync<DummyState, DummyState>[]
                {
                    new DummyChainOfResponsibility1(),
                    new DummyChainOfResponsibility2()
                };

                var composer = new ChainOfResponsibilityAsyncComposer<DummyState, DummyState>(handlers);

                var state = new DummyState
                {
                    Value = 5
                };

                state = await composer.HandleAsync(state, CancellationToken.None);

                state.Should().Be(default);
            }

            [Fact]
            public async Task Should_Throw_When_Cancelled()
            {
                var handlers = new IChainOfResponsibilityHandlerAsync<DummyState, DummyState>[]
                {
                    new DummyChainOfResponsibility1(),
                    new DummyChainOfResponsibility2()
                };

                var composer = new ChainOfResponsibilityAsyncComposer<DummyState, DummyState>(handlers);

                var state = new DummyState
                {
                    Value = 2
                };

                using var cts = new CancellationTokenSource();
                cts.Cancel();

                await Invoking(() => composer.HandleAsync(state, cts.Token))
                    .Should()
                    .ThrowExactlyAsync<OperationCanceledException>();
            }
        }
    }
}