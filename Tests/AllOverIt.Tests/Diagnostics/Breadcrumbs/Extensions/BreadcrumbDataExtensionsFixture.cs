using AllOverIt.Diagnostics.Breadcrumbs;
using AllOverIt.Diagnostics.Breadcrumbs.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;

namespace AllOverIt.Tests.Diagnostics.Breadcrumbs.Extensions
{
    public class BreadcrumbDataExtensionsFixture : FixtureBase
    {
        public class AsMutable : BreadcrumbDataExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_BreadcrumbData_Null()
            {
                Invoking(() =>
                {
                    _ = BreadcrumbDataExtensions.AsMutable(null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("breadcrumbData");
            }

            [Fact]
            public void Should_Return_BreadcrumbDataMutator()
            {
                BreadcrumbDataExtensions
                    .AsMutable(Create<BreadcrumbData>())
                    .Should()
                    .BeOfType<BreadcrumbDataMutator>();
            }
        }
    }
}
