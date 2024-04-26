using AllOverIt.Diagnostics.Breadcrumbs;
using AllOverIt.Extensions;
using AllOverIt.Fixture;
using FluentAssertions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AllOverIt.Tests.Diagnostics.Breadcrumbs
{
    public class BreadcrumbDataFixture : FixtureBase
    {
        [Fact]
        public void Should_Have_Timestamp()
        {
            var data = new BreadcrumbData();

            data.Timestamp
                .Should()
                .BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(20));
        }

        [Fact]
        public void Should_Have_TimestampUtc()
        {
            var data = new BreadcrumbData();

            data.TimestampUtc
                .Should()
                .Be(data.Timestamp.ToUniversalTime());
        }

        [Fact]
        public void Should_Have_Unique_Identifiers_Single_Threaded()
        {
            var indexes = Enumerable.Range(1, 100).ToList();

            var data = indexes.SelectAsReadOnlyCollection(_ => new BreadcrumbData());

            var values = data.Select(item => item.Counter).Distinct();

            values.Should().HaveCount(100);
        }

        [Fact]
        public async Task Should_Have_Unique_Identifiers_Single_Multi_Threaded()
        {
            var indexes = Enumerable.Range(1, 100).ToList();

            var tasks = indexes.SelectAsReadOnlyCollection(_ =>
            {
                return Task.Run(() => new BreadcrumbData());
            });

            var data = await Task.WhenAll(tasks);

            var values = data.Select(item => item.Counter).Distinct();

            values.Should().HaveCount(100);
        }
    }
}
