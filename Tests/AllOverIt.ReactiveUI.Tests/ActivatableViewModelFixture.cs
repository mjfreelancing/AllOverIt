using AllOverIt.Fixture;
using FluentAssertions;
using System;
using System.Reactive.Disposables;
using Xunit;

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
        }

        [Fact]
        public void Should_Activate()
        {
            var isActivated = false;

            var viewModel = new ViewModelDummy(() => { isActivated = true; }, null);

            viewModel.Activator.Activate();

            isActivated.Should().BeTrue();
        }

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
}