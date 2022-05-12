using AllOverIt.Diagnostics.Breadcrumbs.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace AllOverIt.Tests.Diagnostics.Breadcrumbs.Extensions
{
    public class BreadcrumbsExtensionsFixture : FixtureBase
    {
        private readonly AllOverIt.Diagnostics.Breadcrumbs.Breadcrumbs _breadcrumbs = new();

        public class Add_Message : BreadcrumbsExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Breadcrumbs_Null()
            {
                Invoking(() => BreadcrumbsExtensions.Add(null, Create<string>()))
                  .Should()
                  .Throw<ArgumentNullException>()
                  .WithNamedMessageWhenNull("breadcrumbs");
            }

            [Fact]
            public void Should_Throw_When_Message_Null()
            {
                Invoking(() => BreadcrumbsExtensions.Add(_breadcrumbs, null))
                  .Should()
                  .Throw<ArgumentNullException>()
                  .WithNamedMessageWhenNull("message");
            }

            [Fact]
            public void Should_Throw_When_Message_Empty()
            {
                Invoking(() => BreadcrumbsExtensions.Add(_breadcrumbs, string.Empty))
                  .Should()
                  .Throw<ArgumentException>()
                  .WithNamedMessageWhenEmpty("message");
            }

            [Fact]
            public void Should_Throw_When_Message_Whitespace()
            {
                Invoking(() => BreadcrumbsExtensions.Add(_breadcrumbs, "  "))
                  .Should()
                  .Throw<ArgumentException>()
                  .WithNamedMessageWhenEmpty("message");
            }

            [Fact]
            public void Should_Add_Breadcrumb()
            {
                var message = Create<string>();

                var _ = BreadcrumbsExtensions.Add(_breadcrumbs, message);

                var actual = _breadcrumbs.ToList();

                var expected = new[]
                {
                    new
                    {
                        CallerName = (string)null,
                        FilePath = (string)null,
                        LineNumber = 0,
                        Message = message,
                        Metadata = (object)null
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

            [Fact]
            public void Should_Return_Same_Instance()
            {
                var message = Create<string>();

                var actual = BreadcrumbsExtensions.Add(_breadcrumbs, message);

                actual.Should().BeSameAs(_breadcrumbs);
            }
        }

        public class Add_Message_Metadata : BreadcrumbsExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Breadcrumbs_Null()
            {
                Invoking(() => BreadcrumbsExtensions.Add(null, Create<string>(), Create<int>()))
                  .Should()
                  .Throw<ArgumentNullException>()
                  .WithNamedMessageWhenNull("breadcrumbs");
            }

            [Fact]
            public void Should_Throw_When_Message_Null()
            {
                Invoking(() => BreadcrumbsExtensions.Add(_breadcrumbs, null, Create<int>()))
                  .Should()
                  .Throw<ArgumentNullException>()
                  .WithNamedMessageWhenNull("message");
            }

            [Fact]
            public void Should_Throw_When_Message_Empty()
            {
                Invoking(() => BreadcrumbsExtensions.Add(_breadcrumbs, string.Empty, Create<int>()))
                  .Should()
                  .Throw<ArgumentException>()
                  .WithNamedMessageWhenEmpty("message");
            }

            [Fact]
            public void Should_Throw_When_Message_Whitespace()
            {
                Invoking(() => BreadcrumbsExtensions.Add(_breadcrumbs, "  ", Create<int>()))
                  .Should()
                  .Throw<ArgumentException>()
                  .WithNamedMessageWhenEmpty("message");
            }

            [Fact]
            public void Should_Throw_When_Metadata_Null()
            {
                Invoking(() => BreadcrumbsExtensions.Add(_breadcrumbs, Create<string>(), null))
                  .Should()
                  .Throw<ArgumentNullException>()
                  .WithNamedMessageWhenNull("metadata");
            }

            [Fact]
            public void Should_Add_Breadcrumb()
            {
                var message = Create<string>();
                var metadata = Create<int>();

                var _ = BreadcrumbsExtensions.Add(_breadcrumbs, message, metadata);

                var actual = _breadcrumbs.ToList();

                var expected = new[]
                {
                    new
                    {
                        CallerName = (string)null,
                        FilePath = (string)null,
                        LineNumber = 0,
                        Message = message,
                        Metadata = metadata
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

            [Fact]
            public void Should_Return_Same_Instance()
            {
                var message = Create<string>();
                var metadata = new { Value = Create<int>() };

                var actual = BreadcrumbsExtensions.Add(_breadcrumbs, message, metadata);

                actual.Should().BeSameAs(_breadcrumbs);
            }
        }

        public class Add_Message_With_Caller : BreadcrumbsExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Breadcrumbs_Null()
            {
                Invoking(() => BreadcrumbsExtensions.Add(null, this, Create<string>()))
                  .Should()
                  .Throw<ArgumentNullException>()
                  .WithNamedMessageWhenNull("breadcrumbs");
            }

            [Fact]
            public void Should_Throw_When_Caller_Null()
            {
                Invoking(() => BreadcrumbsExtensions.Add(_breadcrumbs, (object)null, Create<string>()))
                  .Should()
                  .Throw<ArgumentNullException>()
                  .WithNamedMessageWhenNull("caller");
            }

            [Fact]
            public void Should_Throw_When_Message_Null()
            {
                Invoking(() => BreadcrumbsExtensions.Add(_breadcrumbs, this, null))
                  .Should()
                  .Throw<ArgumentNullException>()
                  .WithNamedMessageWhenNull("message");
            }

            [Fact]
            public void Should_Throw_When_Message_Empty()
            {
                Invoking(() => BreadcrumbsExtensions.Add(_breadcrumbs, this, string.Empty))
                  .Should()
                  .Throw<ArgumentException>()
                  .WithNamedMessageWhenEmpty("message");
            }

            [Fact]
            public void Should_Throw_When_Message_Whitespace()
            {
                Invoking(() => BreadcrumbsExtensions.Add(_breadcrumbs, this, "  "))
                  .Should()
                  .Throw<ArgumentException>()
                  .WithNamedMessageWhenEmpty("message");
            }

            [Fact]
            public void Should_Add_Breadcrumb()
            {
                var message = Create<string>();

                var _ = BreadcrumbsExtensions.Add(_breadcrumbs, this, message);

                var actual = _breadcrumbs.ToList();

                var expected = new[]
                {
                    new
                    {
                        CallerName = $"AllOverIt.Tests.Diagnostics.Breadcrumbs.Extensions.BreadcrumbsExtensionsFixture+{nameof(Add_Message_With_Caller)}.{nameof(Should_Add_Breadcrumb)}",
                        Message = message,
                        Metadata = (object)null
                    }
                };

                actual.First().FilePath.Should().NotBeNullOrEmpty();
                actual.First().LineNumber.Should().BeGreaterThan(0);

                expected
                    .Should()
                    .BeEquivalentTo(
                        actual,
                        options => options
                            .Excluding(model => model.FilePath)
                            .Excluding(model => model.LineNumber)
                            .Excluding(model => model.Timestamp)
                            .Excluding(model => model.TimestampUtc));
            }

            [Fact]
            public void Should_Add_Breadcrumb_Empty_CallerName()
            {
                var message = Create<string>();

                var _ = BreadcrumbsExtensions.Add(_breadcrumbs, this, message, string.Empty);

                var actual = _breadcrumbs.ToList();

                var expected = new[]
                {
                    new
                    {
                        CallerName = $"AllOverIt.Tests.Diagnostics.Breadcrumbs.Extensions.BreadcrumbsExtensionsFixture+{nameof(Add_Message_With_Caller)}",
                        Message = message,
                        Metadata = (object)null
                    }
                };

                actual.First().FilePath.Should().NotBeNullOrEmpty();
                actual.First().LineNumber.Should().BeGreaterThan(0);

                expected
                    .Should()
                    .BeEquivalentTo(
                        actual,
                        options => options
                            .Excluding(model => model.FilePath)
                            .Excluding(model => model.LineNumber)
                            .Excluding(model => model.Timestamp)
                            .Excluding(model => model.TimestampUtc));
            }

            [Fact]
            public void Should_Add_Breadcrumb_Custom_CallerName()
            {
                var message = Create<string>();
                var callerName = Create<string>();

                var _ = BreadcrumbsExtensions.Add(_breadcrumbs, this, message, callerName);

                var actual = _breadcrumbs.ToList();

                var expected = new[]
                {
                    new
                    {
                        CallerName = $"AllOverIt.Tests.Diagnostics.Breadcrumbs.Extensions.BreadcrumbsExtensionsFixture+{nameof(Add_Message_With_Caller)}.{callerName}",
                        Message = message,
                        Metadata = (object)null
                    }
                };

                actual.First().FilePath.Should().NotBeNullOrEmpty();
                actual.First().LineNumber.Should().BeGreaterThan(0);

                expected
                    .Should()
                    .BeEquivalentTo(
                        actual,
                        options => options
                            .Excluding(model => model.FilePath)
                            .Excluding(model => model.LineNumber)
                            .Excluding(model => model.Timestamp)
                            .Excluding(model => model.TimestampUtc));
            }

            [Fact]
            public void Should_Return_Same_Instance()
            {
                var message = Create<string>();

                var actual = BreadcrumbsExtensions.Add(_breadcrumbs, this, message);

                actual.Should().BeSameAs(_breadcrumbs);
            }
        }
    }
}
