using AllOverIt.Diagnostics.Breadcrumbs;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace AllOverIt.Tests.Diagnostics.Breadcrumbs
{
    public class BreadcrumbsFixture : FixtureBase
    {
        private readonly AllOverIt.Diagnostics.Breadcrumbs.Breadcrumbs _breadcrumbs = new();

        public class Constructor : BreadcrumbsFixture
        {
            [Fact]
            public void Should_Create_Default_Options()
            {
                var actual = _breadcrumbs.Options;

                var expected = new BreadcrumbsOptions();

                expected.Should().BeEquivalentTo(actual);
            }
        }

        public class GetEnumerable : BreadcrumbsFixture
        {
            [Fact]
            public void Should_Return_Empty_When_No_Breadcrumbs()
            {
                var actual = _breadcrumbs.ToList();

                actual.Should().BeEmpty();
            }

            [Fact]
            public void Should_Get_Breadcrumbs()
            {
                var breadcrumb1 = new BreadcrumbData
                {
                    CallerName = string.Empty,
                    FilePath = string.Empty,
                    LineNumber = 0,
                    Message = Create<string>(),
                    Metadata = new { Value = Create<int>() }
                };

                var breadcrumb2 = new BreadcrumbData
                {
                    CallerName = null,
                    FilePath = null,
                    LineNumber = 0,
                    Message = Create<string>()
                };

                var breadcrumb3 = new BreadcrumbData
                {
                    CallerName = Create<string>(),
                    FilePath = Create<string>(),
                    LineNumber = Create<int>(),
                    Message = Create<string>(),
                    Metadata = Create<int>()
                };

                _breadcrumbs.Add(breadcrumb1);
                _breadcrumbs.Add(breadcrumb2);
                _breadcrumbs.Add(breadcrumb3);

                var actual = _breadcrumbs.ToList();

                var expected = new[]
                {
                    new
                    {
                        breadcrumb1.CallerName,
                        breadcrumb1.FilePath,
                        breadcrumb1.LineNumber,
                        breadcrumb1.Message,
                        breadcrumb1.Metadata
                    },
                    new
                    {
                        breadcrumb2.CallerName,
                        breadcrumb2.FilePath,
                        breadcrumb2.LineNumber,
                        breadcrumb2.Message,
                        breadcrumb2.Metadata
                    },
                    new
                    {
                        breadcrumb3.CallerName,
                        breadcrumb3.FilePath,
                        breadcrumb3.LineNumber,
                        breadcrumb3.Message,
                        breadcrumb3.Metadata
                    }
                };

                expected
                    .Should()
                    .BeEquivalentTo(
                        actual,
                        options => options
                            .Excluding(model => model.Timestamp)
                            .Excluding(model => model.TimestampUtc));
            }
        }

        public class Add : BreadcrumbsFixture
        {
            [Fact]
            public void Should_Throw_Null_When_Breadcrumb_Null()
            {
                Invoking(() =>
                {
                    _ = _breadcrumbs.Add(null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("breadcrumb");
            }

            [Fact]
            public void Should_Return_Same_Instance()
            {
                var breadcrumb = Create<BreadcrumbData>();

                var actual = _breadcrumbs.Add(breadcrumb);

                actual.Should().BeSameAs(_breadcrumbs);
            }

            [Fact]
            public void Should_Add_Breadcrumb()
            {
                var breadcrumb = Create<BreadcrumbData>();

                _ = _breadcrumbs.Add(breadcrumb);

                var actual = _breadcrumbs.ToList();

                actual.Single().Should().BeSameAs(breadcrumb);
            }

            [Fact]
            public void Should_Limit_Number_Of_Breadcrumbs()
            {
                var options = new BreadcrumbsOptions
                {
                    MaxCapacity = 5
                };

                var breadcrumbs = new AllOverIt.Diagnostics.Breadcrumbs.Breadcrumbs(options);

                var items = CreateMany<BreadcrumbData>(10);

                foreach (var item in items)
                {
                    breadcrumbs.Add(item);
                }

                var actual = breadcrumbs.ToList();

                actual.Should().HaveCount(5);

                var expected = items.Skip(5).Take(5);

                actual.Should().Contain(expected);
            }
        }
    }
}
