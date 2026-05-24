using AllOverIt.Diagnostics.Breadcrumbs;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Shouldly.Extensions;

namespace AllOverIt.Tests.Diagnostics.Breadcrumbs
{
    public class BreadcrumbsFactoryFixture : FixtureBase
    {
        [Fact]
        public void Should_Use_Default_Options()
        {
            var factory = new BreadcrumbsFactory();

            var breacrumbs = factory.CreateBreadcrumbs() as AllOverIt.Diagnostics.Breadcrumbs.Breadcrumbs;

            breacrumbs.Options.ShouldBeEquivalentTo(new BreadcrumbsOptions());
        }

        [Fact]
        public void Should_Throw_When_Options_Null()
        {
            Invoking(() =>
            {
                _ = new BreadcrumbsFactory(null);
            })
            .ShouldThrow<ArgumentNullException>()
            .WithNamedMessageWhenNull("options");
        }

        [Fact]
        public void Should_Use_Custom_Options()
        {
            var options = new BreadcrumbsOptions();
            var factory = new BreadcrumbsFactory(options);

            var breacrumbs = factory.CreateBreadcrumbs() as AllOverIt.Diagnostics.Breadcrumbs.Breadcrumbs;

            breacrumbs.Options.ShouldBeSameAs(options);
        }

        [Fact]
        public void Should_Return_Breadcrumbs_Instance()
        {
            var factory = new BreadcrumbsFactory();

            var breacrumbs = factory.CreateBreadcrumbs();

            breacrumbs.ShouldBeOfType<AllOverIt.Diagnostics.Breadcrumbs.Breadcrumbs>();
        }
    }
}








