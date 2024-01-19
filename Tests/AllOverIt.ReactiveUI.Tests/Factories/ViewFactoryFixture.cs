using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using AllOverIt.ReactiveUI.Factories;
using FakeItEasy;
using FluentAssertions;
using ReactiveUI;
using System;
using Xunit;

namespace AllOverIt.ReactiveUI.Tests.Factories
{
    public class ViewFactoryFixture : FixtureBase
    {
        public class Constructor : ViewFactoryFixture
        {
            [Fact]
            public void Should_Throw_When_Provider_Null()
            {
                Invoking(() =>
                {
                    _ = new ViewFactory(null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("provider");
            }
        }

        public class CreateViewFor : ViewFactoryFixture
        {
            private class DummyViewModel
            {
            }

            private class DummyView : IViewFor<DummyViewModel>
            {
                public DummyViewModel ViewModel { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
                object IViewFor.ViewModel { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            }

            [Fact]
            public void Should_Create_View()
            {
                var expected = new DummyView();

                var providerFake = this.CreateFake<IServiceProvider>();

                providerFake
                    .CallsTo(fake => fake.GetService(typeof(IViewFor<DummyViewModel>)))
                    .Returns(expected);

                var factory = new ViewFactory(providerFake.FakedObject);

                var actual = factory.CreateViewFor<DummyViewModel>();

                actual.Should().BeSameAs(expected);
            }
        }
    }
}