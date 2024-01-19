using AllOverIt.Collections;
using AllOverIt.Diagnostics.Breadcrumbs;
using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AllOverIt.Tests.Diagnostics.Breadcrumbs
{
    public class BreadcrumbsFixture : FixtureBase
    {
        public class Constructor : BreadcrumbsFixture
        {
            [Fact]
            public void Should_Create_Default_Options()
            {
                var actual = CreateBreadcrumbs().Options;

                var expected = new BreadcrumbsOptions();

                expected.Should().BeEquivalentTo(actual);
            }
        }

        public class Enabled : BreadcrumbsFixture
        {
            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Add_Breadcrumb(bool threadSafe)
            {
                var breadcrumbs = CreateBreadcrumbs(threadSafe);
                var breadcrumb = Create<BreadcrumbData>();

                breadcrumbs.Add(breadcrumb);

                var actual = breadcrumbs.ToList();

                actual.Single().Should().BeSameAs(breadcrumb);
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Not_Add_Breadcrumb(bool threadSafe)
            {
                var breadcrumbs = CreateBreadcrumbs(threadSafe, startEnabled: false);

                var breadcrumb = Create<BreadcrumbData>();

                breadcrumbs.Add(breadcrumb);

                var actual = breadcrumbs.ToList();

                actual.Should().BeEmpty();
            }
        }

        public class Add : BreadcrumbsFixture
        {
            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Throw_Null_When_Breadcrumb_Null(bool threadSafe)
            {
                var breadcrumbs = CreateBreadcrumbs(threadSafe);

                Invoking(() =>
                {
                    breadcrumbs.Add(null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("breadcrumb");
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public async Task Should_Add_Breadcrumbs(bool threadSafe)
            {
                var breadcrumbs = CreateBreadcrumbs(threadSafe);

                var breadcrumbData = CreateMany<BreadcrumbData>(100);

                if (threadSafe)
                {
                    var tasks = Enumerable.Range(0, 100).Select(index =>
                    {
                        return Task.Factory.StartNew(() =>
                        {
                            breadcrumbs.Add(breadcrumbData[index]);
                        });
                    });

                    await Task.WhenAll(tasks);
                }
                else
                {
                    Enumerable.Range(0, 100).ForEach((_, index) =>
                    {
                        breadcrumbs.Add(breadcrumbData[index]);
                    });
                }

                breadcrumbData.Should().BeEquivalentTo(breadcrumbs);
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public async Task Should_Limit_Number_Of_Breadcrumbs(bool threadSafe)
            {
                var breadcrumbs = CreateBreadcrumbs(threadSafe, 10);

                var breadcrumbData = CreateMany<BreadcrumbData>(100);

                if (threadSafe)
                {
                    var tasks = Enumerable.Range(0, 100).Select(index =>
                    {
                        return Task.Factory.StartNew(() =>
                        {
                            breadcrumbs.Add(breadcrumbData[index]);
                        });
                    });

                    await Task.WhenAll(tasks);
                }
                else
                {
                    Enumerable.Range(0, 100).ForEach((_, index) =>
                    {
                        breadcrumbs.Add(breadcrumbData[index]);
                    });
                }

                var expected = breadcrumbData.Skip(90);
                var actual = breadcrumbs.ToList();

                actual.Count.Should().Be(10);
                expected.Should().BeEquivalentTo(actual);
            }
        }

        public class Clear : BreadcrumbsFixture
        {
            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Clear_Breadcrumbs(bool threadSafe)
            {
                var breadcrumbs = CreateBreadcrumbs(threadSafe);

                var breadcrumb = Create<BreadcrumbData>();

                breadcrumbs.Add(breadcrumb);

                breadcrumbs.Should().HaveCount(1);

                breadcrumbs.Clear();

                breadcrumbs.Should().BeEmpty();
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public async Task Should_Not_Reset_StartTimestamp(bool threadSafe)
            {
                var breadcrumbs = CreateBreadcrumbs(threadSafe);
                var originalTimestamp = breadcrumbs.StartTimestamp;

                await Task.Delay(100);

                breadcrumbs.Clear();
                breadcrumbs.StartTimestamp.Should().Be(originalTimestamp);
            }
        }

        public class Reset : BreadcrumbsFixture
        {
            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Clear_Breadcrumbs(bool threadSafe)
            {
                var breadcrumbs = CreateBreadcrumbs(threadSafe);
                var breadcrumb = Create<BreadcrumbData>();

                breadcrumbs.Add(breadcrumb);

                breadcrumbs.Should().HaveCount(1);

                breadcrumbs.Reset();

                breadcrumbs.Should().BeEmpty();
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public async Task Should_Reset_StartTimestamp(bool threadSafe)
            {
                var breadcrumbs = CreateBreadcrumbs(threadSafe);
                var originalTimestamp = breadcrumbs.StartTimestamp;

                await Task.Delay(100);

                breadcrumbs.Reset();
                breadcrumbs.StartTimestamp.Should().NotBe(originalTimestamp);
            }
        }

        public class GetEnumerator : BreadcrumbsFixture
        {
            [Fact]
            public void Should_Return_Empty_When_No_Breadcrumbs()
            {
                var enumerator = CreateBreadcrumbs().GetEnumerator();

                var count = 0;

                while (enumerator.MoveNext())
                {
                    count++;
                }

                count.Should().Be(0);
            }

            [Fact]
            public void Should_Get_Breadcrumbs()
            {
                var breadcrumbs = CreateBreadcrumbs();

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

                breadcrumbs.Add(breadcrumb1);
                breadcrumbs.Add(breadcrumb2);
                breadcrumbs.Add(breadcrumb3);

                var actual = new List<BreadcrumbData>();

                var enumerator = breadcrumbs.GetEnumerator();

                while (enumerator.MoveNext())
                {
                    actual.Add(enumerator.Current);
                }

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

        public class GetEnumerator_Explicit : BreadcrumbsFixture
        {
            [Fact]
            public void Should_Return_Empty_When_No_Breadcrumbs()
            {
                var breadcrumbs = CreateBreadcrumbs();

                var enumerator = ((IEnumerable)breadcrumbs).GetEnumerator();

                var count = 0;

                while (enumerator.MoveNext())
                {
                    count++;
                }

                count.Should().Be(0);
            }

            [Fact]
            public void Should_Get_Breadcrumbs()
            {
                var breadcrumbs = CreateBreadcrumbs();

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

                breadcrumbs.Add(breadcrumb1);
                breadcrumbs.Add(breadcrumb2);
                breadcrumbs.Add(breadcrumb3);

                var actual = new List<BreadcrumbData>();

                var enumerator = ((IEnumerable) breadcrumbs).GetEnumerator();

                while (enumerator.MoveNext())
                {
                    var value = (BreadcrumbData)enumerator.Current;

                    actual.Add(value);
                }

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

        private AllOverIt.Diagnostics.Breadcrumbs.Breadcrumbs CreateBreadcrumbs(bool threadSafe = false, int maxCapacity = -1, bool startEnabled = true)
        {
            var options = new BreadcrumbsOptions
            {
                ThreadSafe = threadSafe,
                MaxCapacity = maxCapacity,
                StartEnabled = startEnabled
            };

            return new AllOverIt.Diagnostics.Breadcrumbs.Breadcrumbs(options);
        }
    }
}
