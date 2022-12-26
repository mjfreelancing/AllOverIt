﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Patterns.ChainOfResponsibility;
using AllOverIt.Patterns.ChainOfResponsibility.Extensions;
using FluentAssertions;
using Xunit;

namespace AllOverIt.Tests.Patterns.ChainOfResponsibility.Extensions
{
    public class ChainOfResponsibilityExtensionsFixture : FixtureBase
    {
        private sealed class DummyState
        {
            public int Value { get; set; }
            public int? ProcessedValue { get; set; }
        }

        public class Compose : ChainOfResponsibilityExtensionsFixture
        {
            private class ChainOfResponsibilityHandlerBase : ChainOfResponsibilityHandler<DummyState, DummyState>
            {
                // NOTE: Don't put state in the handlers - this is just for the unit tests
                public static int Sequence;
            }

            private class ChainOfResponsibilityDummy1 : ChainOfResponsibilityHandlerBase
            {
                public override DummyState Handle(DummyState state)
                {
                    Sequence++;

                    if (Sequence != 1)
                    {
                        throw new InvalidOperationException("Handler sequence is not in the expected order");
                    }

                    if (state.Value % 3 == 0)
                    {
                        state.ProcessedValue = state.Value * 3;
                        return state;
                    }

                    return base.Handle(state);
                }
            }

            private class ChainOfResponsibilityDummy2 : ChainOfResponsibilityHandlerBase
            {
                public override DummyState Handle(DummyState state)
                {
                    Sequence++;

                    if (Sequence != 2)
                    {
                        throw new InvalidOperationException("Handler sequence is not in the expected order");
                    }

                    if (state.Value % 2 == 0)
                    {
                        state.ProcessedValue = state.Value * 2;
                        return state;
                    }

                    return base.Handle(state);
                }
            }

            [Fact]
            public void Should_Throw_When_Handlers_Null()
            {
                Invoking(() =>
                {
                    IEnumerable<IChainOfResponsibilityHandler<DummyState, DummyState>> handlers = null;

                    ChainOfResponsibilityExtensions.Compose(handlers);
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

                    ChainOfResponsibilityExtensions.Compose(handlers);
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("handlers");
            }

            [Fact]
            public void Should_Return_First_Handler()
            {
                var handler1 = new ChainOfResponsibilityDummy1();
                var handler2 = new ChainOfResponsibilityDummy2();

                var handlers = new IChainOfResponsibilityHandler<DummyState, DummyState>[] { handler1, handler2 };

                var actual = ChainOfResponsibilityExtensions.Compose(handlers);

                actual.Should().BeSameAs(handler1);
            }

            [Fact]
            public void Should_Compose_Handlers_In_Sequence()
            {
                var handler1 = new ChainOfResponsibilityDummy1();
                var handler2 = new ChainOfResponsibilityDummy2();

                var handlers = new IChainOfResponsibilityHandler<DummyState, DummyState>[] { handler1, handler2 };

                Invoking(() =>
                {
                    var state = new DummyState
                    {
                        Value = 1
                    };

                    _ = ChainOfResponsibilityExtensions
                        .Compose(handlers)
                        .Handle(state);
                })
                    .Should()
                    .NotThrow();

                ChainOfResponsibilityHandlerBase.Sequence.Should().Be(2);
            }
        }

        public class ComposeAsync : ChainOfResponsibilityExtensionsFixture
        {
            private class ChainOfResponsibilityHandlerAsyncBase : ChainOfResponsibilityHandlerAsync<DummyState, DummyState>
            {
                // NOTE: Don't put state in the handlers - this is just for the unit tests
                public static int Sequence;
            }

            private class ChainOfResponsibilityAsyncDummy1 : ChainOfResponsibilityHandlerAsyncBase
            {
                public override Task<DummyState> HandleAsync(DummyState state)
                {
                    Sequence++;

                    if (Sequence != 1)
                    {
                        throw new InvalidOperationException("Handler sequence is not in the expected order");
                    }

                    if (state.Value % 3 == 0)
                    {
                        state.ProcessedValue = state.Value * 3;
                        return Task.FromResult(state);
                    }

                    return base.HandleAsync(state);
                }
            }

            private class ChainOfResponsibilityAsyncDummy2 : ChainOfResponsibilityHandlerAsyncBase
            {
                public override Task<DummyState> HandleAsync(DummyState state)
                {
                    Sequence++;

                    if (Sequence != 2)
                    {
                        throw new InvalidOperationException("Handler sequence is not in the expected order");
                    }

                    if (state.Value % 2 == 0)
                    {
                        state.ProcessedValue = state.Value * 2;
                        return Task.FromResult(state);
                    }

                    return base.HandleAsync(state);
                }


                [Fact]
                public void Should_Throw_When_Handlers_Null()
                {
                    Invoking(() =>
                    {
                        IEnumerable<IChainOfResponsibilityHandlerAsync<DummyState, DummyState>> handlers = null;

                        _ = ChainOfResponsibilityExtensions.Compose(handlers);
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

                        _ = ChainOfResponsibilityExtensions.Compose(handlers);
                    })
                        .Should()
                        .Throw<ArgumentException>()
                        .WithNamedMessageWhenEmpty("handlers");
                }

                [Fact]
                public void Should_Return_First_Handler()
                {
                    var handler1 = new ChainOfResponsibilityAsyncDummy1();
                    var handler2 = new ChainOfResponsibilityAsyncDummy2();

                    var handlers = new IChainOfResponsibilityHandlerAsync<DummyState, DummyState>[] { handler1, handler2 };

                    var actual = ChainOfResponsibilityExtensions.Compose(handlers);

                    actual.Should().BeSameAs(handler1);
                }

                [Fact]
                public async Task Should_Compose_Handlers_In_Sequence()
                {
                    var handler1 = new ChainOfResponsibilityAsyncDummy1();
                    var handler2 = new ChainOfResponsibilityAsyncDummy2();

                    var handlers = new IChainOfResponsibilityHandlerAsync<DummyState, DummyState>[] { handler1, handler2 };

                    await Invoking(async () =>
                    {
                        var state = new DummyState
                        {
                            Value = 1
                        };

                        _ = await ChainOfResponsibilityExtensions
                            .Compose(handlers)
                            .HandleAsync(state);
                    })
                        .Should()
                        .NotThrowAsync();

                    ChainOfResponsibilityHandlerAsyncBase.Sequence.Should().Be(2);
                }
            }
        }
    }
}