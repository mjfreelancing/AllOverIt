using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Patterns.ChainOfResponsibility;
using FluentAssertions;

namespace AllOverIt.Tests.Patterns.ChainOfResponsibility
{
    public class ChainOfResponsibilityComposerFixture : FixtureBase
    {
        private sealed class DummyState
        {
            public int Value { get; set; }
            public int? ProcessedValue { get; set; }
        }

        private class DummyChainOfResponsibility1 : ChainOfResponsibilityHandler<DummyState, DummyState>
        {
            public override DummyState Handle(DummyState state)
            {
                if (state.Value % 3 == 0)
                {
                    state.ProcessedValue = state.Value * 3;
                    return state;
                }

                return base.Handle(state);
            }
        }

        private class DummyChainOfResponsibility2 : ChainOfResponsibilityHandler<DummyState, DummyState>
        {
            public override DummyState Handle(DummyState state)
            {
                if (state.Value % 2 == 0)
                {
                    state.ProcessedValue = state.Value * 2;
                    return state;
                }

                return base.Handle(state);
            }
        }

        public class Constructor : ChainOfResponsibilityComposerFixture
        {
            [Fact]
            public void Should_Throw_When_Handlers_Null()
            {
                Invoking(() =>
                    {
                        IEnumerable<IChainOfResponsibilityHandler<DummyState, DummyState>> handlers = null;

                        _ = new ChainOfResponsibilityComposer<DummyState, DummyState>(handlers);
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
                        var handlers = new List<IChainOfResponsibilityHandler<DummyState, DummyState>>();

                        _ = new ChainOfResponsibilityComposer<DummyState, DummyState>(handlers);
                    })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("handlers");
            }

            [Fact]
            public void Should_Begin_With_First_Handler()
            {
                var handlers = new IChainOfResponsibilityHandler<DummyState, DummyState>[]
                {
                    new DummyChainOfResponsibility1(),
                    new DummyChainOfResponsibility2()
                };

                var composer = new ChainOfResponsibilityComposer<DummyState, DummyState>(handlers);

                var state = new DummyState
                {
                    Value = 3
                };

                state = composer.Handle(state);

                state.ProcessedValue.Should().Be(9);
            }

            [Fact]
            public void Should_Return_Expected_State()
            {
                var handlers = new IChainOfResponsibilityHandler<DummyState, DummyState>[]
                {
                    new DummyChainOfResponsibility1(),
                    new DummyChainOfResponsibility2()
                };

                var composer = new ChainOfResponsibilityComposer<DummyState, DummyState>(handlers);

                var state = new DummyState
                {
                    Value = 2
                };

                state = composer.Handle(state);

                state.ProcessedValue.Should().Be(4);
            }

            [Fact]
            public void Should_Return_Default_State_When_Unhandled()
            {
                var handlers = new IChainOfResponsibilityHandler<DummyState, DummyState>[]
                {
                    new DummyChainOfResponsibility1(),
                    new DummyChainOfResponsibility2()
                };

                var composer = new ChainOfResponsibilityComposer<DummyState, DummyState>(handlers);

                var state = new DummyState
                {
                    Value = 5
                };

                state = composer.Handle(state);

                state.Should().Be(default);
            }
        }
    }
}