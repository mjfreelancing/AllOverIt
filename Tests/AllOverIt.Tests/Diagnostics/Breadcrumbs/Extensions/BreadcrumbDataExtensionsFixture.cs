using AllOverIt.Diagnostics.Breadcrumbs;
using AllOverIt.Diagnostics.Breadcrumbs.Extensions;
using AllOverIt.Extensions;
using AllOverIt.Fixture;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AllOverIt.Tests.Events
{
    public class BreadcrumbDataExtensionsFixture : FixtureBase
    {
        private readonly Breadcrumbs _breadcrumbs = new();

        public class GetSerializedBreadcrumbs : BreadcrumbDataExtensionsFixture
        {
            [Fact]
            public void Should_Return_Empty_When_No_Breadcrumbs()
            {
                var actual = _breadcrumbs.WithSerializatedMetadata();

                actual.Should().BeEmpty();
            }

            [Fact]
            public void Should_Get_Serialized_Breadcrumbs()
            {
                var message1 = Create<string>();
                var metadata1 = new { Value = Create<int>() };

                var message2 = Create<string>();

                var message3 = Create<string>();
                var metadata3 = Create<int>();

                var message4 = Create<string>();
                var metadata4 = CreateMany<int>();

                _breadcrumbs.Add(message1, metadata1);
                _breadcrumbs.Add(message2);
                _breadcrumbs.Add(message3, metadata3);
                _breadcrumbs.Add(message4, metadata4);

                var actual = _breadcrumbs.WithSerializatedMetadata();

                var timestamps = _breadcrumbs
                    .Select(item => new
                    {
                        item.Timestamp,
                        item.TimestampUtc
                    })
                    .ToList();

                var expected = new[]
                {
                    new
                    {
                        timestamps[0].Timestamp,
                        timestamps[0].TimestampUtc,
                        Message = message1,
                        Metadata = new Dictionary<string, string>
                        {
                            { "Value", $"{metadata1.Value}"}
                        }
                    },
                    new
                    {
                        timestamps[1].Timestamp,
                        timestamps[1].TimestampUtc,
                        Message = message2,
                        Metadata = (Dictionary<string, string>)null
                    },
                    new
                    {
                        timestamps[2].Timestamp,
                        timestamps[2].TimestampUtc,
                        Message = message3,
                        Metadata = new Dictionary<string, string>
                        {
                            { "_", $"{metadata3}"}
                        }
                    },
                    new
                    {
                        timestamps[3].Timestamp,
                        timestamps[3].TimestampUtc,
                        Message = message4,
                        Metadata = new Dictionary<string, string>
                        {
                            { "[]", $"{string.Join(", ", metadata4)}"}
                        }
                    }
                };

                expected.Should().BeEquivalentTo(actual);
            }
        }
    }
}
