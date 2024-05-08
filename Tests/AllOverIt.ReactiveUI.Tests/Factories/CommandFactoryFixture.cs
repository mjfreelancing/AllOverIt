using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.ReactiveUI.Factories;
using FluentAssertions;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace AllOverIt.ReactiveUI.Tests.Factories
{
    public class CommandFactoryFixture : FixtureBase
    {
        public class CreateCancellableCommand_Unit_Unit : CommandFactoryFixture
        {
            [Fact]
            public void Should_Throw_When_Action_Null()
            {
                Invoking(() =>
                {
                    _ = CommandFactory.CreateCancellableCommand((Func<CancellationToken, Task<Unit>>) null, () => null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("action");
            }

            [Fact]
            public void Should_Throw_When_CancelCommand_Null()
            {
                Invoking(() =>
                {
                    _ = CommandFactory.CreateCancellableCommand(token => Task.FromResult(Unit.Default), null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("cancelObservable");
            }

            [Fact]
            public void Should_Create_CreateCancellableCommand()
            {
                var command = CommandFactory.CreateCancellableCommand(token => Task.FromResult(Unit.Default), () => ReactiveCommand.Create(() => { }));

                command.Should().BeOfType<ReactiveCommand<Unit, Unit>>();
            }

            [Fact]
            public async Task Should_Cancel_Cancellable_Command()
            {
                var resetEvent = new ManualResetEventSlim(false);
                var actual = false;

                ReactiveCommand<Unit, Unit> cancelCommand = default;

                var cancellableCommand = CommandFactory
                    .CreateCancellableCommand(async cancellationToken =>
                    {
                        try
                        {
                            await Task.Delay(-1, cancellationToken);
                        }
                        catch (OperationCanceledException)
                        {
                        }

                        actual = true;
                        resetEvent.Set();

                        return Unit.Default;
                    },
                    () => cancelCommand);

                var canCancelStates = new List<bool>();

                cancellableCommand.CanExecute
                    .Subscribe(canCancel =>
                    {
                        canCancelStates.Add(canCancel);
                    });

                cancelCommand = CommandFactory.CreateCancelCommand(cancellableCommand);

                cancellableCommand.Execute().Subscribe();

                await cancelCommand.Execute();

                resetEvent.Wait(TimeSpan.FromMilliseconds(100));

                actual.Should().BeTrue();
                canCancelStates.Should().BeEquivalentTo([true, false, true]);
            }
        }

        public class CreateCancellableCommand_Unit_Result : CommandFactoryFixture
        {
            [Fact]
            public void Should_Throw_When_Action_Null()
            {
                Invoking(() =>
                {
                    _ = CommandFactory.CreateCancellableCommand<int>((Func<CancellationToken, Task<int>>) null, () => null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("action");
            }

            [Fact]
            public void Should_Throw_When_CancelCommand_Null()
            {
                Invoking(() =>
                {
                    _ = CommandFactory.CreateCancellableCommand<int>(token => Task.FromResult(Create<int>()), null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("cancelObservable");
            }

            [Fact]
            public void Should_Create_CreateCancellableCommand()
            {
                var command = CommandFactory.CreateCancellableCommand<int>(token => Task.FromResult(Create<int>()), () => ReactiveCommand.Create(() => { }));

                command.Should().BeOfType<ReactiveCommand<Unit, int>>();
            }

            [Fact]
            public async Task Should_Return_Result()
            {
                var expected = Create<int>();

                var command = CommandFactory.CreateCancellableCommand<int>(token => Task.FromResult(expected), () => ReactiveCommand.Create(() => { }));

                var actual = await command.Execute();

                actual.Should().Be(expected);
            }

            [Fact]
            public async Task Should_Cancel_Cancellable_Command()
            {
                var resetEvent = new ManualResetEventSlim(false);
                var actual = false;

                ReactiveCommand<Unit, Unit> cancelCommand = default;

                var cancellableCommand = CommandFactory
                    .CreateCancellableCommand<int>(async cancellationToken =>
                    {
                        try
                        {
                            await Task.Delay(-1, cancellationToken);
                        }
                        catch (OperationCanceledException)
                        {
                        }

                        actual = true;
                        resetEvent.Set();

                        return Create<int>();
                    },
                    () => cancelCommand);

                var canCancelStates = new List<bool>();

                cancellableCommand.CanExecute
                    .Subscribe(canCancel =>
                    {
                        canCancelStates.Add(canCancel);
                    });

                cancelCommand = CommandFactory.CreateCancelCommand(cancellableCommand);

                cancellableCommand.Execute().Subscribe();

                await cancelCommand.Execute();

                resetEvent.Wait(TimeSpan.FromMilliseconds(100));

                actual.Should().BeTrue();
                canCancelStates.Should().BeEquivalentTo([true, false, true]);
            }
        }

        public class CreateCancellableCommand_Param_Result : CommandFactoryFixture
        {
            [Fact]
            public void Should_Throw_When_Action_Null()
            {
                Invoking(() =>
                {
                    _ = CommandFactory.CreateCancellableCommand<bool, int>((Func<bool, CancellationToken, Task<int>>) null, () => null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("action");
            }

            [Fact]
            public void Should_Throw_When_CancelCommand_Null()
            {
                Invoking(() =>
                {
                    _ = CommandFactory.CreateCancellableCommand<bool, int>((param, token) => Task.FromResult(Create<int>()), null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("cancelObservable");
            }

            [Fact]
            public void Should_Create_CreateCancellableCommand()
            {
                var command = CommandFactory.CreateCancellableCommand<bool, int>((param, token) => Task.FromResult(Create<int>()), () => ReactiveCommand.Create(() => { }));

                command.Should().BeOfType<ReactiveCommand<bool, int>>();
            }

            [Fact]
            public async Task Should_Return_Result()
            {
                var param = Create<bool>();
                var expected = Create<int>();

                var command = CommandFactory.CreateCancellableCommand<bool, int>((param, token) => Task.FromResult(param ? expected : -expected), () => ReactiveCommand.Create(() => { }));

                var actual = await command.Execute(param);

                actual.Should().Be(param ? expected : -expected);
            }

            [Fact]
            public async Task Should_Cancel_Cancellable_Command()
            {
                var resetEvent = new ManualResetEventSlim(false);
                var actual = false;

                ReactiveCommand<Unit, Unit> cancelCommand = default;

                var cancellableCommand = CommandFactory
                    .CreateCancellableCommand<bool, int>(async (_, cancellationToken) =>
                    {
                        try
                        {
                            await Task.Delay(-1, cancellationToken);
                        }
                        catch (OperationCanceledException)
                        {
                        }

                        actual = true;
                        resetEvent.Set();

                        return Create<int>();
                    },
                    () => cancelCommand);

                var canCancelStates = new List<bool>();

                cancellableCommand.CanExecute
                    .Subscribe(canCancel =>
                    {
                        canCancelStates.Add(canCancel);
                    });

                cancelCommand = CommandFactory.CreateCancelCommand(cancellableCommand);

                cancellableCommand.Execute().Subscribe();

                await cancelCommand.Execute();

                resetEvent.Wait(TimeSpan.FromMilliseconds(100));

                actual.Should().BeTrue();
                canCancelStates.Should().BeEquivalentTo([true, false, true]);
            }
        }

        public class CreateCancellableCommand_Param_Result_CancelResult : CommandFactoryFixture
        {
            [Fact]
            public void Should_Throw_When_Action_Null()
            {
                Invoking(() =>
                {
                    _ = CommandFactory.CreateCancellableCommand<bool, int, string>((Func<bool, CancellationToken, Task<int>>) null, () => null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("action");
            }

            [Fact]
            public void Should_Throw_When_CancelCommand_Null()
            {
                Invoking(() =>
                {
                    _ = CommandFactory.CreateCancellableCommand<bool, int, string>((param, token) => Task.FromResult(Create<int>()), null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("cancelObservable");
            }

            [Fact]
            public void Should_Create_CreateCancellableCommand()
            {
                var command = CommandFactory.CreateCancellableCommand<bool, int, string>((param, token) => Task.FromResult(Create<int>()), () => ReactiveCommand.Create<Unit, string>(_ => string.Empty));

                command.Should().BeOfType<ReactiveCommand<bool, int>>();
            }

            [Fact]
            public async Task Should_Return_Result()
            {
                var param = Create<bool>();
                var expected = Create<int>();

                var command = CommandFactory.CreateCancellableCommand<bool, int, string>((param, token) => Task.FromResult(param ? expected : -expected), () => ReactiveCommand.Create<Unit, string>(_ => string.Empty));

                var actual = await command.Execute(param);

                actual.Should().Be(param ? expected : -expected);
            }

            [Fact]
            public void Should_Cancel_Cancellable_Command_Via_Observable()
            {
                var resetEvent = new ManualResetEventSlim(false);
                var actual = false;

                // Using a Subject to represent a 'cancel command'
                Subject<string> cancelCommand = new();

                var cancellableCommand = CommandFactory
                    .CreateCancellableCommand<bool, int, string>(async (_, cancellationToken) =>
                    {
                        try
                        {
                            await Task.Delay(-1, cancellationToken);
                        }
                        catch (OperationCanceledException)
                        {
                        }

                        actual = true;
                        resetEvent.Set();

                        return Create<int>();
                    },
                    () => cancelCommand);

                var canCancelStates = new List<bool>();

                cancellableCommand.CanExecute
                    .Subscribe(canCancel =>
                    {
                        canCancelStates.Add(canCancel);
                    });

                cancellableCommand.Execute().Subscribe();

                cancelCommand.OnNext(string.Empty);

                resetEvent.Wait(TimeSpan.FromMilliseconds(100));

                actual.Should().BeTrue();
                canCancelStates.Should().BeEquivalentTo([true, false, true]);
            }
        }

        public class CreateCancelCommand_Observables : CommandFactoryFixture
        {
            [Fact]
            public void Should_Throw_When_CanExecutes_Null()
            {
                Invoking(() =>
                {
                    _ = CommandFactory.CreateCancelCommand((IObservable<bool>[]) null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithMessage("At least one observable is required. (Parameter 'observables')");
            }

            [Fact]
            public void Should_Throw_When_CanExecutes_Empty()
            {
                Invoking(() =>
                {
                    _ = CommandFactory.CreateCancelCommand(Array.Empty<IObservable<bool>>());
                })
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("At least one observable is required. (Parameter 'observables')");
            }

            [Theory]
            [InlineData(1)]
            [InlineData(2)]
            [InlineData(3)]
            [InlineData(4)]
            [InlineData(5)]
            [InlineData(6)]
            [InlineData(7)]
            [InlineData(8)]
            [InlineData(9)]
            [InlineData(10)]
            [InlineData(11)]
            [InlineData(12)]
            [InlineData(13)]
            [InlineData(14)]
            [InlineData(15)]
            [InlineData(16)]
            public void Should_Set_CanExecute(int subjectCount)
            {
                var subjects = Enumerable.Range(1, subjectCount).SelectToArray(_ => new Subject<bool>());

                var command = CommandFactory.CreateCancelCommand(subjects);

                bool? actual = default;

                using var subscription = command.CanExecute
                    .Subscribe(result =>
                    {
                        actual = result;
                    });

                actual.Should().BeFalse();

                for (var i = 0; i < subjectCount; i++)
                {
                    subjects[i].OnNext(true);

                    actual.Should().BeTrue($"Subject with index {i} (where count = {subjectCount}) emit True");

                    subjects[i].OnNext(false);

                    actual.Should().BeFalse($"Subject with index {i} (where count = {subjectCount}) emit False");
                }
            }

            [Theory]
            [InlineData(1)]
            [InlineData(2)]
            [InlineData(3)]
            [InlineData(4)]
            [InlineData(5)]
            [InlineData(6)]
            [InlineData(7)]
            [InlineData(8)]
            [InlineData(9)]
            [InlineData(10)]
            [InlineData(11)]
            [InlineData(12)]
            [InlineData(13)]
            [InlineData(14)]
            [InlineData(15)]
            [InlineData(16)]
            public async Task Should_Create_Command_That_Cancels_Cancellable_Command(int subjectCount)
            {
                var subjects = Enumerable.Range(1, subjectCount).SelectToArray(_ => new Subject<bool>());

                var command = CommandFactory.CreateCancelCommand(subjects);

                var actual = new List<bool>(2);

                using var subscription = command.IsExecuting
                   .Subscribe(result =>
                   {
                       actual.Add(result);
                   });

                actual.Should().BeEquivalentTo([false]);

                for (var i = 0; i < subjectCount; i++)
                {
                    actual.Clear();

                    subjects[i].OnNext(true);

                    await command.Execute();

                    actual.Should().BeEquivalentTo([true, false]);
                }
            }

            [Fact]
            public void Should_Throw_When_Too_Many_Observables()
            {
                Invoking(() =>
                {
                    var subjects = Enumerable.Range(1, 17).SelectToArray(_ => new Subject<bool>());

                    _ = CommandFactory.CreateCancelCommand(subjects);
                })
                    .Should()
                    .Throw<ArgumentOutOfRangeException>()
                    .WithMessage("A maximum of 16 observables is supported. (Parameter 'observables')");
            }
        }

        public class CreateCancelCommand_ReactiveCommand : CommandFactoryFixture
        {
            [Fact]
            public void Should_Throw_When_CanExecutes_Null()
            {
                Invoking(() =>
                {
                    _ = CommandFactory.CreateCancelCommand((IReactiveCommand[]) null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithMessage("At least one cancellable command is required. (Parameter 'cancellableCommands')");
            }

            [Fact]
            public void Should_Throw_When_CanExecutes_Empty()
            {
                Invoking(() =>
                {
                    _ = CommandFactory.CreateCancelCommand(Array.Empty<IReactiveCommand>());
                })
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("At least one cancellable command is required. (Parameter 'cancellableCommands')");
            }

            [Fact]
            public async Task Should_Cancel_Cancellable_Command()
            {
                var resetEvent = new ManualResetEventSlim(false);
                var actual = false;

                ReactiveCommand<Unit, Unit> cancelCommand = default;

                var cancellableCommand = CommandFactory
                    .CreateCancellableCommand(async cancellationToken =>
                    {
                        try
                        {
                            await Task.Delay(-1, cancellationToken);
                        }
                        catch (OperationCanceledException)
                        {
                        }

                        actual = true;
                        resetEvent.Set();

                        return Unit.Default;
                    },
                    () => cancelCommand);

                cancelCommand = CommandFactory.CreateCancelCommand(cancellableCommand);

                cancellableCommand.Execute().Subscribe();

                await cancelCommand.Execute();

                resetEvent.Wait(TimeSpan.FromMilliseconds(100));

                actual.Should().BeTrue();
            }

            [Fact]
            public async Task Should_Cancel_Multiple_Cancellable_Commands()
            {
                var resetEvent1 = new ManualResetEventSlim(false);
                var resetEvent2 = new ManualResetEventSlim(false);

                var actual1 = false;
                var actual2 = false;

                ReactiveCommand<Unit, Unit> cancelCommand = default;

                var cancellableCommand1 = CommandFactory
                    .CreateCancellableCommand(async cancellationToken =>
                    {
                        try
                        {
                            await Task.Delay(-1, cancellationToken);
                        }
                        catch (OperationCanceledException)
                        {
                        }

                        actual1 = true;
                        resetEvent1.Set();

                        return Unit.Default;
                    },
                    () => cancelCommand);

                var cancellableCommand2 = CommandFactory
                    .CreateCancellableCommand(async cancellationToken =>
                    {
                        try
                        {
                            await Task.Delay(-1, cancellationToken);
                        }
                        catch (OperationCanceledException)
                        {
                        }

                        actual2 = true;
                        resetEvent2.Set();

                        return Unit.Default;
                    },
                    () => cancelCommand);

                cancelCommand = CommandFactory.CreateCancelCommand(cancellableCommand1, cancellableCommand2);

                cancellableCommand1.Execute().Subscribe();
                cancellableCommand2.Execute().Subscribe();

                await cancelCommand.Execute();

                resetEvent1.Wait(TimeSpan.FromMilliseconds(100));
                resetEvent2.Wait(TimeSpan.FromMilliseconds(100));

                actual1.Should().BeTrue();
                actual2.Should().BeTrue();
            }
        }
    }
}