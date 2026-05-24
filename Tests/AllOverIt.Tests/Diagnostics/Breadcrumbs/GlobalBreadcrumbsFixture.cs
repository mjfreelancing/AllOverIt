using AllOverIt.Diagnostics.Breadcrumbs;
using AllOverIt.Fixture;

namespace AllOverIt.Tests.Diagnostics.Breadcrumbs
{
    public class GlobalBreadcrumbsFixture : FixtureBase
    {
        [Fact]
        public void Should_Create_Instance_With_Default_Options()
        {
            var instance = GlobalBreadcrumbs.Instance;

            instance.ShouldBeAssignableTo<IBreadcrumbs>();
            instance.Enabled.ShouldBeTrue();
        }

        [Fact]
        public void Should_Create_With_Options()
        {
            var options = new BreadcrumbsOptions
            {
                StartEnabled = false
            };

            var instance = GlobalBreadcrumbs.Create(options);

            instance.Enabled.ShouldBeFalse();
        }

        [Fact]
        public void Should_Return_Same_Instance()
        {
            var instance1 = GlobalBreadcrumbs.Instance;
            var instance2 = GlobalBreadcrumbs.Instance;

            instance1.ShouldBeSameAs(instance2);
        }

        [Fact]
        public void Should_Create_New_Instance_After_Destroy()
        {
            var instance1 = GlobalBreadcrumbs.Instance;

            GlobalBreadcrumbs.Destroy();

            var instance2 = GlobalBreadcrumbs.Instance;

            instance1.ShouldNotBeSameAs(instance2);
        }

        [Fact]
        public void Should_Not_Throw_When_Destroyed_Without_Creation()
        {
            Invoking(() =>
            {
                GlobalBreadcrumbs.Destroy();
            })
            .ShouldNotThrow();
        }

        [Fact]
        public void Should_Not_Throw_When_Destroyed_Twice()
        {
            Invoking(() =>
            {
                _ = GlobalBreadcrumbs.Instance;

                GlobalBreadcrumbs.Destroy();
                GlobalBreadcrumbs.Destroy();
            })
            .ShouldNotThrow();
        }
    }
}




