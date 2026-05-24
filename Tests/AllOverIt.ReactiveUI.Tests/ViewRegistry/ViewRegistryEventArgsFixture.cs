using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.ReactiveUI.ViewRegistry.Events;
using AllOverIt.Shouldly.Extensions;
using FakeItEasy;
using ReactiveUI;

namespace AllOverIt.ReactiveUI.Tests.ViewRegistry
{
    public class ViewRegistryEventArgsFixture : FixtureBase
    {
        public class Constructor : ViewRegistryEventArgsFixture
        {
            [Fact]
            public void Should_Throw_When_ViewModelType_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                {
                    _ = new ViewRegistryEventArgs(null, A.Fake<IViewFor>(), Create<ViewItemUpdateType>());
                })
                .WithNamedMessageWhenNull("viewModelType");
            }

            [Fact]
            public void Should_Throw_When_View_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                {
                    _ = new ViewRegistryEventArgs(typeof(object), null, Create<ViewItemUpdateType>());
                })
                .WithNamedMessageWhenNull("view");
            }

            [Fact]
            public void Should_Set_Properties()
            {
                var viewModelType = typeof(object);
                var view = A.Fake<IViewFor>();
                var updateType = Create<ViewItemUpdateType>();

                var actual = new ViewRegistryEventArgs(viewModelType, view, updateType);

                var expected = new
                {
                    ViewModelType = viewModelType,
                    View = view,
                    UpdateType = updateType
                };

                actual.ShouldBeEquivalentTo(expected);
            }
        }
    }
}







