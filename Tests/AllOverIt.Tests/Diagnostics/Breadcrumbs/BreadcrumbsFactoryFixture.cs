using AllOverIt.Diagnostics.Breadcrumbs;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System;

namespace AllOverIt.Tests.Diagnostics.Breadcrumbs
{
    public class BreadcrumbsFactoryFixture : FixtureBase
    {
        [Fact]
        public void Should_Use_Default_Options()
        {
            var factory = new BreadcrumbsFactory();

            var breacrumbs = factory.CreateBreadcrumbs() as AllOverIt.Diagnostics.Breadcrumbs.Breadcrumbs;

            breacrumbs.Options.Should().BeEquivalentTo(new BreadcrumbsOptions());
        }

        [Fact]
        public void Should_Throw_When_Options_Null()
        {
            Invoking(() =>
            {
                _ = new BreadcrumbsFactory(null);
            })
            .Should()
            .Throw<ArgumentNullException>()
            .WithNamedMessageWhenNull("options");
        }

        [Fact]
        public void Should_Use_Custom_Options()
        {
            var options = new BreadcrumbsOptions();
            var factory = new BreadcrumbsFactory(options);

            var breacrumbs = factory.CreateBreadcrumbs() as AllOverIt.Diagnostics.Breadcrumbs.Breadcrumbs;

            breacrumbs.Options.Should().BeSameAs(options);
        }

        [Fact]
        public void Should_Return_Breadcrumbs_Instance()
        {
            var factory = new BreadcrumbsFactory();

            var breacrumbs = factory.CreateBreadcrumbs();

            breacrumbs.Should().BeOfType<AllOverIt.Diagnostics.Breadcrumbs.Breadcrumbs>();
        }
    }
}
