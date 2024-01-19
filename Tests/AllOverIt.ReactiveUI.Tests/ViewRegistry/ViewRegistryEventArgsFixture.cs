using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.ReactiveUI.ViewRegistry.Events;
using FakeItEasy;
using FluentAssertions;
using ReactiveUI;
using System;
using Xunit;

namespace AllOverIt.ReactiveUI.Tests.ViewRegistry
{
    public class ViewRegistryEventArgsFixture : FixtureBase
    {
        public class Constructor : ViewRegistryEventArgsFixture
        {
            [Fact]
            public void Should_Throw_When_ViewModelType_Null()
            {
                Invoking(() =>
                {
                    _ = new ViewRegistryEventArgs(null, A.Fake<IViewFor>(), Create<ViewItemUpdateType>());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("viewModelType");
            }

            [Fact]
            public void Should_Throw_When_View_Null()
            {
                Invoking(() =>
                {
                    _ = new ViewRegistryEventArgs(typeof(object), null, Create<ViewItemUpdateType>());
                })
                .Should()
                .Throw<ArgumentNullException>()
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

                expected.Should().BeEquivalentTo(actual);
            }
        }
    }
}