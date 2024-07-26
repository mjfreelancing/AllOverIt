using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Disposables;

namespace AllOverIt.ReactiveUI.Tests
{
    public class ActivatableViewModelFixture : FixtureBase
    {
        private class ViewModelDummy : ActivatableViewModel
        {
            private readonly Action _activated;
            private readonly Action _deactivated;

            public ViewModelDummy(Action activated, Action deactivated)
            {
                _activated = activated;
                _deactivated = deactivated;
            }

            protected override void OnActivated(CompositeDisposable disposables)
            {
                _activated?.Invoke();
            }

            protected override void OnDeactivated()
            {
                base.OnDeactivated();

                _deactivated?.Invoke();
            }

            public CancellationTokenSource DoCreateAutoCancellingTokenSource()
            {
                return CreateAutoCancellingTokenSource();
            }

            public CancellationTokenSource DoCreateAutoCancellingTokenSource(CancellationToken cancellationToken)
            {
                return CreateAutoCancellingTokenSource(cancellationToken);
            }

            public ReactiveCommand<Unit, Unit> DoCreateAutoCancellingCommand(Func<CancellationToken, Task> action)
            {
                return CreateAutoCancellingCommand(action);
            }

            public ReactiveCommand<Unit, TResult> DoCreateAutoCancellingCommand<TResult>(Func<CancellationToken, Task<TResult>> action)
            {
                return CreateAutoCancellingCommand<TResult>(action);
            }

            public ReactiveCommand<TParam, Unit> DoCreateAutoCancellingCommand<TParam>(Func<TParam, CancellationToken, Task> action)
            {
                return CreateAutoCancellingCommand<TParam>(action);
            }

            public ReactiveCommand<TParam, TResult> DoCreateAutoCancellingCommand<TParam, TResult>(Func<TParam, CancellationToken, Task<TResult>> action)
            {
                return CreateAutoCancellingCommand<TParam, TResult>(action);
            }
        }

        public class OnActivated : ActivatableViewModelFixture
        {
            [Fact]
            public void Should_Activate()
            {
                var isActivated = false;

                var viewModel = new ViewModelDummy(() => { isActivated = true; }, null);

                viewModel.Activator.Activate();

                isActivated.Should().BeTrue();
            }
        }

        public class OnDeactivated : ActivatableViewModelFixture
        {
            [Fact]
            public void Should_Deactivate()
            {
                var isDeactivated = false;

                var viewModel = new ViewModelDummy(null, () => { isDeactivated = true; });

                using (viewModel.Activator.Activate())
                {
                }

                isDeactivated.Should().BeTrue();
            }
        }

        public class CreateAutoCancellingTokenSource : ActivatableViewModelFixture
        {
            [Fact]
            public void Should_Throw_Before_Activation()
            {
                var viewModel = new ViewModelDummy(null, null);

                Invoking(() =>
                {
                    _ = viewModel.DoCreateAutoCancellingTokenSource();
                })
                .Should()
                .Throw<InvalidOperationException>()
                .WithMessage("Cannot create an auto-cancelling token source before activation or after deactivation.");
            }

            [Fact]
            public void Should_Throw_After_Deactivation()
            {
                var viewModel = new ViewModelDummy(null, null);

                using (viewModel.Activator.Activate())
                {
                }

                Invoking(() =>
                {
                    _ = viewModel.DoCreateAutoCancellingTokenSource();
                })
                .Should()
                .Throw<InvalidOperationException>()
                .WithMessage("Cannot create an auto-cancelling token source before activation or after deactivation.");
            }

            [Fact]
            public void Should_Auto_Cancel_Token_Source()
            {
                var viewModel = new ViewModelDummy(null, null);

                var cts = new CancellationTokenSource[3];

                using (viewModel.Activator.Activate())
                {
                    cts[0] = viewModel.DoCreateAutoCancellingTokenSource();
                    cts[1] = viewModel.DoCreateAutoCancellingTokenSource();
                    cts[2] = viewModel.DoCreateAutoCancellingTokenSource();

                    cts[0].IsCancellationRequested.Should().BeFalse();
                    cts[1].IsCancellationRequested.Should().BeFalse();
                    cts[2].IsCancellationRequested.Should().BeFalse();
                }

                cts[0].IsCancellationRequested.Should().BeTrue();
                cts[1].IsCancellationRequested.Should().BeTrue();
                cts[2].IsCancellationRequested.Should().BeTrue();

                cts[0].Dispose();
                cts[1].Dispose();
                cts[2].Dispose();
            }
        }

        public class CreateAutoCancellingTokenSource_CancellingToken : ActivatableViewModelFixture
        {
            [Fact]
            public void Should_Throw_Before_Activation()
            {
                var viewModel = new ViewModelDummy(null, null);

                Invoking(() =>
                {
                    _ = viewModel.DoCreateAutoCancellingTokenSource(CancellationToken.None);
                })
                .Should()
                .Throw<InvalidOperationException>()
                .WithMessage("Cannot create an auto-cancelling token source before activation or after deactivation.");
            }

            [Fact]
            public void Should_Throw_After_Deactivation()
            {
                var viewModel = new ViewModelDummy(null, null);

                using (viewModel.Activator.Activate())
                {
                }

                Invoking(() =>
                {
                    _ = viewModel.DoCreateAutoCancellingTokenSource(CancellationToken.None);
                })
                .Should()
                .Throw<InvalidOperationException>()
                .WithMessage("Cannot create an auto-cancelling token source before activation or after deactivation.");
            }

            [Fact]
            public void Should_Auto_Cancel_Token_Source()
            {
                var viewModel = new ViewModelDummy(null, null);

                using var otherCts1 = new CancellationTokenSource();
                using var otherCts2 = new CancellationTokenSource();
                using var otherCts3 = new CancellationTokenSource();

                var cts = new CancellationTokenSource[3];

                using (viewModel.Activator.Activate())
                {
                    cts[0] = viewModel.DoCreateAutoCancellingTokenSource(otherCts1.Token);
                    cts[1] = viewModel.DoCreateAutoCancellingTokenSource(otherCts2.Token);
                    cts[2] = viewModel.DoCreateAutoCancellingTokenSource(otherCts3.Token);

                    otherCts1.IsCancellationRequested.Should().BeFalse();
                    otherCts2.IsCancellationRequested.Should().BeFalse();
                    otherCts3.IsCancellationRequested.Should().BeFalse();

                    cts[0].IsCancellationRequested.Should().BeFalse();
                    cts[1].IsCancellationRequested.Should().BeFalse();
                    cts[2].IsCancellationRequested.Should().BeFalse();
                }

                // These will still be non-cancelled
                otherCts1.IsCancellationRequested.Should().BeFalse();
                otherCts2.IsCancellationRequested.Should().BeFalse();
                otherCts3.IsCancellationRequested.Should().BeFalse();

                // But the linked tokens will be cancelled
                cts[0].IsCancellationRequested.Should().BeTrue();
                cts[1].IsCancellationRequested.Should().BeTrue();
                cts[2].IsCancellationRequested.Should().BeTrue();

                cts[0].Dispose();
                cts[1].Dispose();
                cts[2].Dispose();
            }
        }

        public class CreateAutoCancellingCommand_Unit_Unit : ActivatableViewModelFixture
        {
            [Fact]
            public void Should_Throw_When_Action_Null()
            {
                var viewModel = new ViewModelDummy(null, null);

                Invoking(() =>
                {
                    _ = viewModel.DoCreateAutoCancellingCommand((Func<CancellationToken, Task>) null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("action");
            }

            [Fact]
            public void Should_Auto_Cancel_Command()
            {
                var viewModel = new ViewModelDummy(null, null);

                var command = viewModel.DoCreateAutoCancellingCommand(cancellationToken => Task.Delay(-1, cancellationToken));

                var isExecutingList = new List<bool>();

                var subscription1 = command.IsExecuting.Subscribe(isExecuting => isExecutingList.Add(isExecuting));

                using var subscription2 = command.Execute().Subscribe();

                using (viewModel.Activator.Activate())
                {
                    isExecutingList.Should().BeEquivalentTo([false, true], options => options.WithStrictOrdering());
                }

                isExecutingList.Should().BeEquivalentTo([false, true, false], options => options.WithStrictOrdering());
            }
        }

        public class CreateAutoCancellingCommand_Unit_Result : ActivatableViewModelFixture
        {
            [Fact]
            public void Should_Throw_When_Action_Null()
            {
                var viewModel = new ViewModelDummy(null, null);

                Invoking(() =>
                {
                    _ = viewModel.DoCreateAutoCancellingCommand((Func<CancellationToken, Task<string>>) null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("action");
            }

            [Fact]
            public void Should_Auto_Cancel_Command()
            {
                var viewModel = new ViewModelDummy(null, null);

                var command = viewModel.DoCreateAutoCancellingCommand<string>(async cancellationToken =>
                {
                    await Task.Delay(-1, cancellationToken);

                    return Create<string>();
                });

                var isExecutingList = new List<bool>();

                var subscription1 = command.IsExecuting.Subscribe(isExecuting => isExecutingList.Add(isExecuting));

                using var subscription2 = command.Execute().Subscribe();

                using (viewModel.Activator.Activate())
                {
                    isExecutingList.Should().BeEquivalentTo([false, true], options => options.WithStrictOrdering());
                }

                isExecutingList.Should().BeEquivalentTo([false, true, false], options => options.WithStrictOrdering());
            }
        }

        public class CreateAutoCancellingCommand_Param_Unit : ActivatableViewModelFixture
        {
            [Fact]
            public void Should_Throw_When_Action_Null()
            {
                var viewModel = new ViewModelDummy(null, null);

                Invoking(() =>
                {
                    _ = viewModel.DoCreateAutoCancellingCommand<string>((Func<string, CancellationToken, Task>) null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("action");
            }

            [Fact]
            public void Should_Auto_Cancel_Command()
            {
                var viewModel = new ViewModelDummy(null, null);

                var command = viewModel.DoCreateAutoCancellingCommand<string>((value, cancellationToken) => Task.Delay(-1, cancellationToken));

                var isExecutingList = new List<bool>();

                var subscription1 = command.IsExecuting.Subscribe(isExecuting => isExecutingList.Add(isExecuting));

                using var subscription2 = command.Execute().Subscribe();

                using (viewModel.Activator.Activate())
                {
                    isExecutingList.Should().BeEquivalentTo([false, true], options => options.WithStrictOrdering());
                }

                isExecutingList.Should().BeEquivalentTo([false, true, false], options => options.WithStrictOrdering());
            }
        }

        public class CreateAutoCancellingCommand_Param_Result : ActivatableViewModelFixture
        {
            [Fact]
            public void Should_Throw_When_Action_Null()
            {
                var viewModel = new ViewModelDummy(null, null);

                Invoking(() =>
                {
                    _ = viewModel.DoCreateAutoCancellingCommand<string, int>((Func<string, CancellationToken, Task<int>>) null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("action");
            }

            [Fact]
            public void Should_Auto_Cancel_Command()
            {
                var viewModel = new ViewModelDummy(null, null);

                var command = viewModel.DoCreateAutoCancellingCommand<string, int>(async (value, cancellationToken) =>
                {
                    await Task.Delay(-1, cancellationToken);

                    return Create<int>();
                });

                var isExecutingList = new List<bool>();

                var subscription1 = command.IsExecuting.Subscribe(isExecuting => isExecutingList.Add(isExecuting));

                using var subscription2 = command.Execute().Subscribe();

                using (viewModel.Activator.Activate())
                {
                    isExecutingList.Should().BeEquivalentTo([false, true], options => options.WithStrictOrdering());
                }

                isExecutingList.Should().BeEquivalentTo([false, true, false], options => options.WithStrictOrdering());
            }
        }
    }
}