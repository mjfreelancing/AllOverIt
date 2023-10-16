using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using AllOverIt.ReactiveUI.Factories;
using AllOverIt.ReactiveUI.ViewRegistry;
using AllOverIt.ReactiveUI.ViewRegistry.Events;
using FakeItEasy;
using FluentAssertions;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Xunit;

namespace AllOverIt.ReactiveUI.Tests.ViewRegistry
{
    public class ViewRegistryFixture : FixtureBase
    {
        private class DummyViewModel1
        {
        }

        private class DummyViewModel2 : DummyViewModel1
        {
        }

        private sealed class DummyView : IViewFor<DummyViewModel1>
        {
            public DummyViewModel1 ViewModel { get; set; }
            object IViewFor.ViewModel { get; set; }

            public EventHandler ClosedEventHandler;

            public void Close()
            {
                ClosedEventHandler.Invoke(this, EventArgs.Empty);
            }
        }

        private sealed class DummyViewHandler : IViewHandler
        {
            public bool CloseView { get; set; }

            public IList<IViewFor> ActivatedViews { get; } = new List<IViewFor>();
            public IList<IViewFor> ShownViews { get; } = new List<IViewFor>();
            public IList<IViewFor> ClosedViews { get; } = new List<IViewFor>();

            public void Activate(IViewFor view)
            {
                ActivatedViews.Add(view);
            }

            public void Show(IViewFor view)
            {
                ShownViews.Add(view);
            }

            public void Close(IViewFor view)
            {
                ClosedViews.Add(view);

                if (CloseView)
                {
                    (view as DummyView).Close();
                }
            }

            public void SetOnActivatedHandler(IViewFor view, EventHandler eventHandler, bool register)
            {
                throw new NotImplementedException();
            }

            public void SetOnClosedHandler(IViewFor view, EventHandler eventHandler, bool register)
            {
                if (register)
                {
                    (view as DummyView).ClosedEventHandler += eventHandler;
                }
                else
                {
                    (view as DummyView).ClosedEventHandler -= eventHandler;
                }
            }

            public void SetOnDeactivatedHandler(IViewFor view, EventHandler eventHandler, bool register)
            {
                throw new NotImplementedException();
            }
        }

        private readonly Fake<IViewFactory> _viewFactoryFake;
        private readonly IViewHandler _viewHandler = new DummyViewHandler();
        private readonly DummyViewModel1[] _dummyViewModels;
        private readonly DummyView[] _dummyViews;
        private readonly ViewRegistry<int> _viewRegistry;

        public ViewRegistryFixture()
        {
            _viewFactoryFake = this.CreateFake<IViewFactory>();

            _dummyViewModels = CreateMany<DummyViewModel1>(5).ToArray();
            _dummyViews = CreateMany<DummyView>(5).ToArray();

            _dummyViewModels
                .Zip(_dummyViews)
                .ForEach((context, index) =>
                {
                    // context is view model, view
                    context.Second.ViewModel = context.First;

                    _viewFactoryFake
                        .CallsTo(fake => fake.CreateViewFor<DummyViewModel1>())
                        .ReturnsNextFromSequence(_dummyViews);
                });

            _viewRegistry = new ViewRegistry<int>(_viewFactoryFake.FakedObject, _viewHandler);
        }

        public class OnUpdate : ViewRegistryFixture
        {
            private readonly int _expected;
            private int _actual;
            private ViewItemUpdateType? _updateType;

            [Fact]
            public void Should_Raise_OnUpdate_When_View_Created()
            {
                _viewRegistry.OnUpdate += ViewRegistryOnUpdate;

                try
                {
                    _viewRegistry.CreateOrActivateFor<DummyViewModel1>(Create<int>(), viewItems => { return Create<int>(); });
                }
                finally
                {
                    _viewRegistry.OnUpdate -= ViewRegistryOnUpdate;
                }

                _actual.Should().Be(_expected);
                _updateType.Value.Should().Be(ViewItemUpdateType.Add);
            }

            [Fact]
            public void Should_Raise_OnUpdate_When_View_Closed()
            {
                _viewRegistry.OnUpdate += ViewRegistryOnUpdate;

                try
                {
                    _viewRegistry.CreateOrActivateFor<DummyViewModel1>(Create<int>(), viewItems => { return Create<int>(); });

                    (_dummyViews[0] as DummyView).Close();
                }
                finally
                {
                    _viewRegistry.OnUpdate -= ViewRegistryOnUpdate;
                }

                _actual.Should().Be(_expected);
                _updateType.Value.Should().Be(ViewItemUpdateType.Remove);
            }

            private void ViewRegistryOnUpdate(object sender, ViewRegistryEventArgs eventArgs)
            {
                _actual = _expected;
                _updateType = eventArgs.UpdateType;
            }
        }

        public class Constructor : ViewRegistryFixture
        {
            [Fact]
            public void Should_Throw_When_ViewFactory_Null()
            {
                Invoking(() =>
                {
                    _ = new ViewRegistry<int>(null, _viewHandler);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("viewFactory");
            }

            [Fact]
            public void Should_Throw_When_ViewHandler_Null()
            {
                Invoking(() =>
                {
                    _ = new ViewRegistry<int>(_viewFactoryFake.FakedObject, null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("viewHandler");
            }
        }

        public class GetViewCountFor_Generic_Type : ViewRegistryFixture
        {
            [Fact]
            public void Should_Get_Zero()
            {
                var actual = _viewRegistry.GetViewCountFor<DummyViewModel1>();

                actual.Should().Be(0);
            }

            [Fact]
            public void Should_Get_Expected_Count()
            {
                var maxCount = 3 + Create<int>();

                var count = GetWithinRange(1, 5);

                for (var i = 0; i < count; i++)
                {
                    _viewRegistry.CreateOrActivateFor<DummyViewModel1>(maxCount, viewItems => Create<int>());
                }

                var actual1 = _viewRegistry.GetViewCountFor<DummyViewModel1>();
                actual1.Should().Be(count);

                var actual2 = _viewRegistry.GetViewCountFor<DummyViewModel2>();
                actual2.Should().Be(0);
            }
        }

        public class GetViewCountFor_Type : ViewRegistryFixture
        {
            [Fact]
            public void Should_Get_Zero()
            {
                var actual = _viewRegistry.GetViewCountFor(typeof(DummyViewModel1));

                actual.Should().Be(0);
            }

            [Fact]
            public void Should_Get_Expected_Count()
            {
                var maxCount = 3 + Create<int>();

                var count = GetWithinRange(1, 5);

                for (var i = 0; i < count; i++)
                {
                    _viewRegistry.CreateOrActivateFor<DummyViewModel1>(maxCount, viewItems => Create<int>());
                }

                var actual1 = _viewRegistry.GetViewCountFor(typeof(DummyViewModel1));
                actual1.Should().Be(count);

                var actual2 = _viewRegistry.GetViewCountFor(typeof(DummyViewModel2));
                actual2.Should().Be(0);
            }
        }

        public class GetViewModelTypes : ViewRegistryFixture
        {
            [Fact]
            public void Should_Return_Empty_Types()
            {
                var actual = _viewRegistry.GetViewModelTypes();

                actual.Should().BeEmpty();
            }

            [Fact]
            public void Should_Return_Types()
            {
                _viewRegistry.CreateOrActivateFor<DummyViewModel1>(Create<int>(), viewItems => Create<int>());

                var actual = _viewRegistry.GetViewModelTypes().Single();

                actual.Should().Be(typeof(DummyViewModel1));
            }
        }

        public class GetViewsFor_Generic_Type : ViewRegistryFixture
        {
            [Fact]
            public void Should_Return_Empty_Views()
            {
                var actual = _viewRegistry.GetViewsFor<DummyViewModel1>();

                actual.Should().BeEmpty();
            }

            [Fact]
            public void Should_Return_Views()
            {
                var ids = CreateMany<int>().ToArray();

                var maxCount = 3 + Create<int>();

                _viewRegistry.CreateOrActivateFor<DummyViewModel1>(maxCount, viewItems => ids[0]);
                _viewRegistry.CreateOrActivateFor<DummyViewModel1>(maxCount, viewItems => ids[1]);
                _viewRegistry.CreateOrActivateFor<DummyViewModel1>(maxCount, viewItems => ids[2]);

                var actual = _viewRegistry.GetViewsFor<DummyViewModel1>().ToArray();

                for (var i = 0; i < 3; i++)
                {
                    actual[i].Id.Should().Be(ids[i]);
                    actual[i].View.Should().Be(_dummyViews[i]);
                }
            }
        }

        public class GetViewsFor_Type : ViewRegistryFixture
        {
            [Fact]
            public void Should_Return_Empty_Views()
            {
                var actual = _viewRegistry.GetViewsFor(typeof(DummyViewModel1));

                actual.Should().BeEmpty();
            }

            [Fact]
            public void Should_Return_Views()
            {
                var maxCount = Create<int>();
                var ids = CreateMany<int>(3).ToArray();

                _viewRegistry.CreateOrActivateFor<DummyViewModel1>(maxCount, viewItems => ids[0]);
                _viewRegistry.CreateOrActivateFor<DummyViewModel1>(maxCount, viewItems => ids[1]);
                _viewRegistry.CreateOrActivateFor<DummyViewModel1>(maxCount, viewItems => ids[2]);

                var actual = _viewRegistry.GetViewsFor(typeof(DummyViewModel1)).ToArray();

                for (var i = 0; i < 3; i++)
                {
                    actual[i].Id.Should().Be(ids[i]);
                    actual[i].View.Should().Be(_dummyViews[i]);
                }
            }
        }

        public class CreateOrActivateFor : ViewRegistryFixture
        {
            [Fact]
            public void Should_Set_View_Id()
            {
                var expected = Create<int>();

                _viewRegistry.CreateOrActivateFor<DummyViewModel1>(Create<int>(), viewItems => expected);

                _viewRegistry.GetViewsFor<DummyViewModel1>().Single().Id.Should().Be(expected);
            }

            [Fact]
            public void Should_Configure_ViewModel()
            {
                var expected = Create<int>();
                var actual = -expected;

                _viewRegistry.CreateOrActivateFor<DummyViewModel1>(
                    Create<int>(),
                    viewItems => expected,
                    (vm, view, id) => { actual = id; });

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Activate_All_When_Views_At_Max()
            {
                _viewRegistry.CreateOrActivateFor<DummyViewModel1>(2, viewItems => { return Create<int>(); });
                _viewRegistry.CreateOrActivateFor<DummyViewModel1>(2, viewItems => { return Create<int>(); });

                _viewRegistry.CreateOrActivateFor<DummyViewModel1>(2, viewItems => { return Create<int>(); });

                var activatedViews = (_viewHandler as DummyViewHandler).ActivatedViews;

                foreach (var dummyView in _dummyViews.Take(2))
                {
                    activatedViews.Should().Contain(dummyView);
                }

                foreach (var dummyView in _dummyViews.Skip(2))
                {
                    activatedViews.Should().NotContain(dummyView);
                }
            }

            [Fact]
            public void Should_Unregister_View_When_Closed()
            {
                _viewRegistry.CreateOrActivateFor<DummyViewModel1>(Create<int>(), viewItems => { return Create<int>(); });

                _viewRegistry.IsEmpty.Should().BeFalse();

                (_dummyViews[0] as DummyView).Close();

                _viewRegistry.IsEmpty.Should().BeTrue();
            }

            [Fact]
            public void Should_Show_New_Views()
            {
                _viewRegistry.CreateOrActivateFor<DummyViewModel1>(3, viewItems => { return Create<int>(); });
                _viewRegistry.CreateOrActivateFor<DummyViewModel1>(3, viewItems => { return Create<int>(); });
                _viewRegistry.CreateOrActivateFor<DummyViewModel1>(3, viewItems => { return Create<int>(); });

                var shownViews = (_viewHandler as DummyViewHandler).ShownViews;

                foreach (var dummyView in _dummyViews.Take(3))
                {
                    shownViews.Should().Contain(dummyView);
                }
            }            
        }

        public class TryCloseAllViews : ViewRegistryFixture
        {
            [Fact]
            public void Should_Close_All_Views()
            {
                (_viewHandler as DummyViewHandler).CloseView = true;    // emulate a window closing itself

                _viewRegistry.IsEmpty.Should().BeTrue();

                var maxCount = 3 + Create<int>();

                _viewRegistry.CreateOrActivateFor<DummyViewModel1>(maxCount, viewItems => { return Create<int>(); });
                _viewRegistry.CreateOrActivateFor<DummyViewModel1>(maxCount, viewItems => { return Create<int>(); });
                _viewRegistry.CreateOrActivateFor<DummyViewModel1>(maxCount, viewItems => { return Create<int>(); });

                _viewRegistry.IsEmpty.Should().BeFalse();

                _viewRegistry.GetViewCountFor<DummyViewModel1>().Should().Be(3);

                // Returns true to indicate there are no more windows
                _viewRegistry.TryCloseAllViews().Should().BeTrue();

                var closedViews = (_viewHandler as DummyViewHandler).ClosedViews;

                foreach (var dummyView in _dummyViews.Take(3))
                {
                    closedViews.Should().Contain(dummyView);
                }

                _viewRegistry.IsEmpty.Should().BeTrue();
            }
        }

        public class GetEnumerator : ViewRegistryFixture
        {
            [Fact]
            public void Should_Enumerate()
            {
                var maxCount = 3 + Create<int>();

                _viewRegistry.CreateOrActivateFor<DummyViewModel1>(maxCount, viewItems => { return Create<int>(); });
                _viewRegistry.CreateOrActivateFor<DummyViewModel1>(maxCount, viewItems => { return Create<int>(); });
                _viewRegistry.CreateOrActivateFor<DummyViewModel1>(maxCount, viewItems => { return Create<int>(); });

                _viewRegistry.Count().Should().Be(3);           // also using the enumerator for this

                var index = 0;

                foreach (var viewItem in _viewRegistry)
                {
                    viewItem.View.Should().BeSameAs(_dummyViews[index++]);
                }
            }

            [Fact]
            public void Should_Enumerate_Explicit()
            {
                var maxCount = 3 + Create<int>();

                _viewRegistry.CreateOrActivateFor<DummyViewModel1>(maxCount, viewItems => { return Create<int>(); });
                _viewRegistry.CreateOrActivateFor<DummyViewModel1>(maxCount, viewItems => { return Create<int>(); });
                _viewRegistry.CreateOrActivateFor<DummyViewModel1>(maxCount, viewItems => { return Create<int>(); });

                _viewRegistry.Count().Should().Be(3);           // also using the enumerator for this

                var index = 0;

                foreach (var viewItem in (IEnumerable)_viewRegistry)
                {
                    ((ViewModelViewItem<int>)viewItem).View.Should().BeSameAs(_dummyViews[index++]);
                }
            }
        }
    }
}