using System;
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
            public int Sequence { get; set; }
            public int Value { get; set; }
            public int? ProcessedValue { get; set; }
        }

        private class DummyChainOfResponsibility1 : ChainOfResponsibilityHandler<DummyState, DummyState>
        {
            public override DummyState Handle(DummyState state)
            {
                state.Sequence++;

                if (state.Sequence != 1)
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

        private class DummyChainOfResponsibility2 : ChainOfResponsibilityHandler<DummyState, DummyState>
        {
            public override DummyState Handle(DummyState state)
            {
                state.Sequence++;

                if (state.Sequence != 2)
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

        private class DummyChainOfResponsibility3 : ChainOfResponsibilityHandler<DummyState, DummyState>
        {
            public override DummyState Handle(DummyState state)
            {
                state.Sequence++;

                if (state.Sequence != 3)
                {
                    throw new InvalidOperationException("Handler sequence is not in the expected order");
                }

                state.ProcessedValue = state.Value + 1;
                return state;
            }
        }

        private class DummyChainOfResponsibilityAsync1 : ChainOfResponsibilityHandlerAsync<DummyState, DummyState>
        {
            public override Task<DummyState> HandleAsync(DummyState state)
            {
                state.Sequence++;

                if (state.Sequence != 1)
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

        private class DummyChainOfResponsibilityAsync2 : ChainOfResponsibilityHandlerAsync<DummyState, DummyState>
        {
            public override Task<DummyState> HandleAsync(DummyState state)
            {
                state.Sequence++;

                if (state.Sequence != 2)
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
        }

        private class DummyChainOfResponsibilityAsync3 : ChainOfResponsibilityHandlerAsync<DummyState, DummyState>
        {
            public override Task<DummyState> HandleAsync(DummyState state)
            {
                state.Sequence++;

                if (state.Sequence != 3)
                {
                    throw new InvalidOperationException("Handler sequence is not in the expected order");
                }

                state.ProcessedValue = state.Value + 1;
                return Task.FromResult(state);
            }
        }

        public class Compose : ChainOfResponsibilityExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Handlers_Null()
            {
                Invoking(() =>
                {
                    IEnumerable<IChainOfResponsibilityHandler<DummyState, DummyState>> handlers = null;

                    ChainOfResponsibilityHandlerExtensions.Compose(handlers);
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

                    ChainOfResponsibilityHandlerExtensions.Compose(handlers);
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("handlers");
            }

            [Fact]
            public void Should_Return_First_Handler()
            {
                var handler1 = new DummyChainOfResponsibility1();
                var handler2 = new DummyChainOfResponsibility2();

                var handlers = new IChainOfResponsibilityHandler<DummyState, DummyState>[] { handler1, handler2 };

                var actual = ChainOfResponsibilityHandlerExtensions.Compose(handlers);

                actual.Should().BeSameAs(handler1);
            }

            [Fact]
            public void Should_Compose_Handlers_In_Sequence()
            {
                var handler1 = new DummyChainOfResponsibility1();
                var handler2 = new DummyChainOfResponsibility2();

                var handlers = new IChainOfResponsibilityHandler<DummyState, DummyState>[] { handler1, handler2 };

                var state = new DummyState
                {
                    Value = 1
                };

                Invoking(() =>
                {
                    _ = ChainOfResponsibilityHandlerExtensions
                        .Compose(handlers)
                        .Handle(state);
                })
                    .Should()
                    .NotThrow();

                state.Sequence.Should().Be(2);
            }
        }

        public class ComposeAsync : ChainOfResponsibilityExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Handlers_Null()
            {
                Invoking(() =>
                {
                    IEnumerable<IChainOfResponsibilityHandlerAsync<DummyState, DummyState>> handlers = null;

                    _ = ChainOfResponsibilityHandlerExtensions.Compose(handlers);
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

                    _ = ChainOfResponsibilityHandlerExtensions.Compose(handlers);
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("handlers");
            }

            [Fact]
            public void Should_Return_First_Handler()
            {
                var handler1 = new DummyChainOfResponsibilityAsync1();
                var handler2 = new DummyChainOfResponsibilityAsync2();

                var handlers = new IChainOfResponsibilityHandlerAsync<DummyState, DummyState>[] { handler1, handler2 };

                var actual = ChainOfResponsibilityHandlerExtensions.Compose(handlers);

                actual.Should().BeSameAs(handler1);
            }

            [Fact]
            public async Task Should_Compose_Handlers_In_Sequence()
            {
                var handler1 = new DummyChainOfResponsibilityAsync1();
                var handler2 = new DummyChainOfResponsibilityAsync2();

                var handlers = new IChainOfResponsibilityHandlerAsync<DummyState, DummyState>[] { handler1, handler2 };

                var state = new DummyState
                {
                    Value = 1
                };

                await Invoking(async () =>
                {
                    _ = await ChainOfResponsibilityHandlerExtensions
                        .Compose(handlers)
                        .HandleAsync(state);
                })
                    .Should()
                    .NotThrowAsync();

                state.Sequence.Should().Be(2);
            }
        }

        public class Then : ChainOfResponsibilityExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_First_Null()
            {
                Invoking(() =>
                {
                    _ = ChainOfResponsibilityHandlerExtensions.Then<DummyState, DummyState>(null, new DummyChainOfResponsibility1());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("first");
            }

            [Fact]
            public void Should_Throw_When_Next_Null()
            {
                Invoking(() =>
                {
                    _ = ChainOfResponsibilityHandlerExtensions.Then<DummyState, DummyState>(new DummyChainOfResponsibility1(), null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("next");
            }

            [Fact]
            public void Should_Compose_Handlers_Return_First()
            {
                var handler1 = new DummyChainOfResponsibility1();
                var handler2 = new DummyChainOfResponsibility2();

                var composed = ChainOfResponsibilityHandlerExtensions.Then(handler1, handler2);

                var state = new DummyState
                {
                    Value = 3
                };

                var actual = composed.Handle(state);

                actual.ProcessedValue.Should().Be(9);
            }

            [Fact]
            public void Should_Compose_Handlers_Return_Second()
            {
                var handler1 = new DummyChainOfResponsibility1();
                var handler2 = new DummyChainOfResponsibility2();

                var composed = ChainOfResponsibilityHandlerExtensions.Then(handler1, handler2);

                var state = new DummyState
                {
                    Value = 2
                };

                var actual = composed.Handle(state);

                actual.ProcessedValue.Should().Be(4);
            }

            [Fact]
            public void Should_Compose_Handlers_Return_Third_Approach_1()
            {
                var handler1 = new DummyChainOfResponsibility1();
                var handler2 = new DummyChainOfResponsibility2();
                var handler3 = new DummyChainOfResponsibility3();

                var composed = ChainOfResponsibilityHandlerExtensions.Then(handler1, handler2).Then(handler3);

                var state = new DummyState
                {
                    Value = 5
                };

                var actual = composed.Handle(state);

                actual.ProcessedValue.Should().Be(6);
            }

            [Fact]
            public void Should_Compose_Handlers_Return_Third_Approach_2()
            {
                var handler1 = new DummyChainOfResponsibility1();
                var handler2 = new DummyChainOfResponsibility2();
                var handler3 = new DummyChainOfResponsibility3();

                var composed = handler1.Then(handler2).Then(handler3);

                var state = new DummyState
                {
                    Value = 5
                };

                var actual = composed.Handle(state);

                actual.ProcessedValue.Should().Be(6);
            }
        }

        public class ThenAsync : ChainOfResponsibilityExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_First_Null()
            {
                Invoking(() =>
                {
                    _ = ChainOfResponsibilityHandlerExtensions.Then<DummyState, DummyState>(null, new DummyChainOfResponsibilityAsync1());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("first");
            }

            [Fact]
            public void Should_Throw_When_Next_Null()
            {
                Invoking(() =>
                {
                    _ = ChainOfResponsibilityHandlerExtensions.Then<DummyState, DummyState>(new DummyChainOfResponsibilityAsync1(), null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("next");
            }

            [Fact]
            public async Task Should_Compose_Handlers_Return_First()
            {
                var handler1 = new DummyChainOfResponsibilityAsync1();
                var handler2 = new DummyChainOfResponsibilityAsync2();

                var composed = ChainOfResponsibilityHandlerExtensions.Then(handler1, handler2);

                var state = new DummyState
                {
                    Value = 3
                };

                var actual = await composed.HandleAsync(state);

                actual.ProcessedValue.Should().Be(9);
            }

            [Fact]
            public async Task Should_Compose_Handlers_Return_Second()
            {
                var handler1 = new DummyChainOfResponsibilityAsync1();
                var handler2 = new DummyChainOfResponsibilityAsync2();

                var composed = ChainOfResponsibilityHandlerExtensions.Then(handler1, handler2);

                var state = new DummyState
                {
                    Value = 2
                };

                var actual = await composed.HandleAsync(state);

                actual.ProcessedValue.Should().Be(4);
            }

            [Fact]
            public async Task Should_Compose_Handlers_Return_Third_Approach_1()
            {
                var handler1 = new DummyChainOfResponsibilityAsync1();
                var handler2 = new DummyChainOfResponsibilityAsync2();
                var handler3 = new DummyChainOfResponsibilityAsync3();

                var composed = ChainOfResponsibilityHandlerExtensions.Then(handler1, handler2).Then(handler3);

                var state = new DummyState
                {
                    Value = 5
                };

                var actual = await composed.HandleAsync(state);

                actual.ProcessedValue.Should().Be(6);
            }

            [Fact]
            public async Task Should_Compose_Handlers_Return_Third_Approach_2()
            {
                var handler1 = new DummyChainOfResponsibilityAsync1();
                var handler2 = new DummyChainOfResponsibilityAsync2();
                var handler3 = new DummyChainOfResponsibilityAsync3();

                var composed = handler1.Then(handler2).Then(handler3);

                var state = new DummyState
                {
                    Value = 5
                };

                var actual = await composed.HandleAsync(state);

                actual.ProcessedValue.Should().Be(6);
            }
        }
    }
}