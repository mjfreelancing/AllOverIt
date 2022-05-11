﻿using AllOverIt.Diagnostics.Breadcrumbs;
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
                    Message = Create<string>(),
                    Metadata = new { Value = Create<int>() }
                };

                var breadcrumb2 = new BreadcrumbData
                {
                    CallerName = null,
                    Message = Create<string>()
                };

                var breadcrumb3 = new BreadcrumbData
                {
                    CallerName = Create<string>(),
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
                        breadcrumb1.Message,
                        breadcrumb1.Metadata
                    },
                    new
                    {
                        breadcrumb2.CallerName,
                        breadcrumb2.Message,
                        breadcrumb2.Metadata
                    },
                    new
                    {
                        breadcrumb3.CallerName,
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
        }
    }
}
