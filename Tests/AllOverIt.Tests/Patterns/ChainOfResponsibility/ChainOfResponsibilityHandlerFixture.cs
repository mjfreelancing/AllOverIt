using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Patterns.ChainOfResponsibility;
using AllOverIt.Patterns.ChainOfResponsibility.Extensions;
using FluentAssertions;

namespace AllOverIt.Tests.Patterns.ChainOfResponsibility
{
    public class ChainOfResponsibilityHandlerFixture : FixtureBase
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

        public class SetNext : ChainOfResponsibilityHandlerFixture
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

        public class Handle : ChainOfResponsibilityHandlerFixture
        {
            [Fact]
            public void Should_Return_Default_When_End_Of_Chain()
            {
                var sut = new DummyChainOfResponsibility1();

                var state = new DummyState
                {
                    Value = 5
                };

                var actual = sut.Handle(state);

                actual.Should().Be(default);
            }

            [Fact]
            public void Should_Be_Handled_By_First_Handler()
            {
                var handler = new ChainOfResponsibilityHandler<DummyState, DummyState>[]
                {
                    new DummyChainOfResponsibility1(), new DummyChainOfResponsibility2()
                }.Compose();

                var state = new DummyState
                {
                    Value = 3
                };

                state = handler.Handle(state);

                state.ProcessedValue.Should().Be(9);
            }

            [Fact]
            public void Should_Be_Handled_By_Second_Handler()
            {
                var handler = new ChainOfResponsibilityHandler<DummyState, DummyState>[]
                {
                    new DummyChainOfResponsibility1(), new DummyChainOfResponsibility2()
                }.Compose();

                var state = new DummyState
                {
                    Value = 2
                };

                state = handler.Handle(state);

                state.ProcessedValue.Should().Be(4);
            }

            [Fact]
            public void Should_Not_Be_Handled()
            {
                var handler = new ChainOfResponsibilityHandler<DummyState, DummyState>[]
                {
                    new DummyChainOfResponsibility1(), new DummyChainOfResponsibility2()
                }.Compose();

                var state = new DummyState
                {
                    Value = 5
                };

                state = handler.Handle(state);

                state.Should().Be(null);
            }
        }
    }
}