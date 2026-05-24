using AllOverIt.Diagnostics.Breadcrumbs;
using AllOverIt.Diagnostics.Breadcrumbs.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using System.Runtime.CompilerServices;
using AllOverIt.Shouldly.Extensions;

namespace AllOverIt.Tests.Diagnostics.Breadcrumbs.Extensions
{
    public class BreadcrumbsExtensionsFixture : FixtureBase
    {
        private readonly AllOverIt.Diagnostics.Breadcrumbs.Breadcrumbs _breadcrumbs = new();

        public class AddCallSite : BreadcrumbsExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Breadcrumbs_Null()
            {
                Invoking(() => BreadcrumbsExtensions.AddCallSite(null, this))
                  .ShouldThrow<ArgumentNullException>()
                  .WithNamedMessageWhenNull("breadcrumbs");
            }

            [Fact]
            public void Should_Throw_When_Caller_Null()
            {
                Invoking(() => BreadcrumbsExtensions.AddCallSite(_breadcrumbs, null))
                  .ShouldThrow<ArgumentNullException>()
                  .WithNamedMessageWhenNull("caller");
            }

            [Fact]
            public void Should_Throw_When_CallerName_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue => BreadcrumbsExtensions.AddCallSite(_breadcrumbs, this, null, stringValue),
                    "callerName");
            }

            [Fact]
            public void Should_Not_Throw_When_Metadata_Null()
            {
                Invoking(() => BreadcrumbsExtensions.AddCallSite(_breadcrumbs, this, null))
                  .ShouldNotThrow();
            }

            [Fact]
            public void Should_Add_Breadcrumb_Message_Null_Metadata()
            {
                BreadcrumbsExtensions.AddCallSite(_breadcrumbs, this);

                var actual = _breadcrumbs.ToList();

                var fullCallerName = "AllOverIt.Tests.Diagnostics.Breadcrumbs.Extensions.BreadcrumbsExtensionsFixture+AddCallSite.Should_Add_Breadcrumb_Message_Null_Metadata";

                var expected = new[]
                {
                    new
                    {
                        CallerName = fullCallerName,
                        FilePath = (string)null,
                        LineNumber = 0,
                        Message = $"Call Site: {fullCallerName}()",
                        Metadata = (object)null,
                        Tags = Array.Empty<string>()
                    }
                };

                actual.ShouldBeEquivalentTo(expected, opts => opts.ExcludeMember(nameof(BreadcrumbData.Timestamp)).ExcludeMember(nameof(BreadcrumbData.TimestampUtc)));
            }

            [Fact]
            public void Should_Add_Breadcrumb_Message_With_Metadata()
            {
                var metadata = new
                {
                    Prop1 = Create<int>(),
                    Prop2 = Create<string>()
                };

                BreadcrumbsExtensions.AddCallSite(_breadcrumbs, this, metadata);

                var actual = _breadcrumbs.ToList();

                var fullCallerName = "AllOverIt.Tests.Diagnostics.Breadcrumbs.Extensions.BreadcrumbsExtensionsFixture+AddCallSite.Should_Add_Breadcrumb_Message_With_Metadata";

                var expected = new[]
                {
                    new
                    {
                        CallerName = fullCallerName,
                        FilePath = (string)null,
                        LineNumber = 0,
                        Message = $"Call Site: {fullCallerName}()",
                        Metadata = metadata,
                        Tags = Array.Empty<string>()
                    }
                };

                actual.ShouldBeEquivalentTo(expected, opts => opts.ExcludeMember(nameof(BreadcrumbData.Timestamp)).ExcludeMember(nameof(BreadcrumbData.TimestampUtc)));
            }

            [Fact]
            public void Should_Add_Breadcrumb_Message_With_Metadata_And_Caller()
            {
                var metadata = new
                {
                    Prop1 = Create<int>(),
                    Prop2 = Create<string>()
                };

                var callerName = Create<string>();

                BreadcrumbsExtensions.AddCallSite(_breadcrumbs, this, metadata, callerName);

                var actual = _breadcrumbs.ToList();

                var fullCallerName = $"AllOverIt.Tests.Diagnostics.Breadcrumbs.Extensions.BreadcrumbsExtensionsFixture+AddCallSite.{callerName}";

                var expected = new[]
                {
                    new
                    {
                        CallerName = fullCallerName,
                        FilePath = (string)null,
                        LineNumber = 0,
                        Message = $"Call Site: {fullCallerName}()",
                        Metadata = metadata,
                        Tags = Array.Empty<string>()
                    }
                };

                actual.ShouldBeEquivalentTo(expected, opts => opts.ExcludeMember(nameof(BreadcrumbData.Timestamp)).ExcludeMember(nameof(BreadcrumbData.TimestampUtc)));
            }
        }

        public class AddExtendedCallSite : BreadcrumbsExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Breadcrumbs_Null()
            {
                Invoking(() => BreadcrumbsExtensions.AddExtendedCallSite(null, this))
                  .ShouldThrow<ArgumentNullException>()
                  .WithNamedMessageWhenNull("breadcrumbs");
            }

            [Fact]
            public void Should_Throw_When_Caller_Null()
            {
                Invoking(() => BreadcrumbsExtensions.AddExtendedCallSite(_breadcrumbs, null))
                  .ShouldThrow<ArgumentNullException>()
                  .WithNamedMessageWhenNull("caller");
            }

            [Fact]
            public void Should_Throw_When_CallerName_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue => BreadcrumbsExtensions.AddExtendedCallSite(_breadcrumbs, this, null, stringValue),
                    "callerName");
            }

            [Fact]
            public void Should_Not_Throw_When_Metadata_Null()
            {
                Invoking(() => BreadcrumbsExtensions.AddExtendedCallSite(_breadcrumbs, this, null))
                  .ShouldNotThrow();
            }

            [Fact]
            public void Should_Add_Breadcrumb_Message()
            {
                BreadcrumbsExtensions.AddExtendedCallSite(_breadcrumbs, this);

                var actual = _breadcrumbs.ToList();

                var fullCallerName = $@"AllOverIt.Tests.Diagnostics.Breadcrumbs.Extensions.BreadcrumbsExtensionsFixture+AddExtendedCallSite.{nameof(Should_Add_Breadcrumb_Message)}";

                var expected = new[]
                {
                    new
                    {
                        CallerName = fullCallerName,
                        FilePath = GetCallerFilePath(),
                        LineNumber = GetCallerLineNumber() - 12,
                        Message = $@"Call Site: AllOverIt.Tests.Diagnostics.Breadcrumbs.Extensions.BreadcrumbsExtensionsFixture+AddExtendedCallSite.{nameof(Should_Add_Breadcrumb_Message)}(), at {GetCallerFilePath()}:{GetCallerLineNumber()-13}",
                        Metadata = (object)null,
                        Tags = Array.Empty<string>()
                    }
                };

                actual.ShouldBeEquivalentTo(expected, opts => opts.ExcludeMember(nameof(BreadcrumbData.Timestamp)).ExcludeMember(nameof(BreadcrumbData.TimestampUtc)));
            }
        }

        public class Add_Message : BreadcrumbsExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Breadcrumbs_Null()
            {
                Invoking(() => BreadcrumbsExtensions.Add(null, Create<string>()))
                  .ShouldThrow<ArgumentNullException>()
                  .WithNamedMessageWhenNull("breadcrumbs");
            }

            [Fact]
            public void Should_Throw_When_Message_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue => BreadcrumbsExtensions.Add(_breadcrumbs, stringValue),
                    "message");
            }

            [Fact]
            public void Should_Add_Breadcrumb()
            {
                var message = Create<string>();

                BreadcrumbsExtensions.Add(_breadcrumbs, message);

                var actual = _breadcrumbs.ToList();

                var expected = new[]
                {
                    new
                    {
                        CallerName = (string)null,
                        FilePath = (string)null,
                        LineNumber = 0,
                        Message = message,
                        Metadata = (object)null,
                        Tags = Array.Empty<string>()
                    }
                };

                actual.ShouldBeEquivalentTo(expected, opts => opts.ExcludeMember(nameof(BreadcrumbData.Timestamp)).ExcludeMember(nameof(BreadcrumbData.TimestampUtc)));
            }
        }

        public class Add_Message_Metadata : BreadcrumbsExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Breadcrumbs_Null()
            {
                Invoking(() => BreadcrumbsExtensions.Add(null, Create<string>(), Create<int>()))
                  .ShouldThrow<ArgumentNullException>()
                  .WithNamedMessageWhenNull("breadcrumbs");
            }

            [Fact]
            public void Should_Throw_When_Message_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue => BreadcrumbsExtensions.Add(_breadcrumbs, stringValue, Create<int>()),
                    "message");
            }

            [Fact]
            public void Should_Throw_When_Metadata_Null()
            {
                Invoking(() => BreadcrumbsExtensions.Add(_breadcrumbs, Create<string>(), (object) null))
                  .ShouldThrow<ArgumentNullException>()
                  .WithNamedMessageWhenNull("metadata");
            }

            [Fact]
            public void Should_Add_Breadcrumb()
            {
                var message = Create<string>();
                var metadata = Create<int>();

                BreadcrumbsExtensions.Add(_breadcrumbs, message, metadata);

                var actual = _breadcrumbs.ToList();

                var expected = new[]
                {
                    new
                    {
                        CallerName = (string)null,
                        FilePath = (string)null,
                        LineNumber = 0,
                        Message = message,
                        Metadata = metadata,
                        Tags = Array.Empty<string>()
                    }
                };

                actual.ShouldBeEquivalentTo(expected, opts => opts.ExcludeMember(nameof(BreadcrumbData.Timestamp)).ExcludeMember(nameof(BreadcrumbData.TimestampUtc)));
            }
        }

        public class Add_With_Caller_Message : BreadcrumbsExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Breadcrumbs_Null()
            {
                Invoking(() => BreadcrumbsExtensions.Add(null, this, Create<string>()))
                  .ShouldThrow<ArgumentNullException>()
                  .WithNamedMessageWhenNull("breadcrumbs");
            }

            [Fact]
            public void Should_Throw_When_Caller_Null()
            {
                Invoking(() => BreadcrumbsExtensions.Add(_breadcrumbs, (object) null, Create<string>()))
                  .ShouldThrow<ArgumentNullException>()
                  .WithNamedMessageWhenNull("caller");
            }

            [Fact]
            public void Should_Throw_When_Message_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue => BreadcrumbsExtensions.Add(_breadcrumbs, this, stringValue),
                    "message");
            }

            [Fact]
            public void Should_Add_Breadcrumb()
            {
                var message = Create<string>();

                BreadcrumbsExtensions.Add(_breadcrumbs, this, message);

                var actual = _breadcrumbs.ToList();

                var expected = new[]
                {
                    new
                    {
                        CallerName = $"AllOverIt.Tests.Diagnostics.Breadcrumbs.Extensions.BreadcrumbsExtensionsFixture+{nameof(Add_With_Caller_Message)}.{nameof(Should_Add_Breadcrumb)}",
                        FilePath = GetCallerFilePath(),
                        LineNumber = GetCallerLineNumber() - 10,
                        Message = message,
                        Metadata = (object)null,
                        Tags = Array.Empty<string>()
                    }
                };

                actual.First().FilePath.ShouldNotBeNullOrEmpty();
                actual.First().LineNumber.ShouldBeGreaterThan(0);

                actual.ShouldBeEquivalentTo(expected, opts => opts.ExcludeMember(nameof(BreadcrumbData.Timestamp)).ExcludeMember(nameof(BreadcrumbData.TimestampUtc)));
            }

            [Fact]
            public void Should_Add_Breadcrumb_Empty_CallerName()
            {
                var message = Create<string>();

                BreadcrumbsExtensions.Add(_breadcrumbs, this, message, string.Empty);

                var actual = _breadcrumbs.ToList();

                var expected = new[]
                {
                    new
                    {
                        CallerName = $"AllOverIt.Tests.Diagnostics.Breadcrumbs.Extensions.BreadcrumbsExtensionsFixture+{nameof(Add_With_Caller_Message)}",
                        FilePath = GetCallerFilePath(),
                        LineNumber = GetCallerLineNumber() - 10,
                        Message = message,
                        Metadata = (object)null,
                        Tags = Array.Empty<string>()
                    }
                };

                actual.First().FilePath.ShouldNotBeNullOrEmpty();
                actual.First().LineNumber.ShouldBeGreaterThan(0);

                actual.ShouldBeEquivalentTo(expected, opts => opts.ExcludeMember(nameof(BreadcrumbData.Timestamp)).ExcludeMember(nameof(BreadcrumbData.TimestampUtc)));
            }

            [Fact]
            public void Should_Add_Breadcrumb_Custom_CallerName()
            {
                var message = Create<string>();
                var callerName = Create<string>();

                BreadcrumbsExtensions.Add(_breadcrumbs, this, message, callerName);

                var actual = _breadcrumbs.ToList();

                var expected = new[]
                {
                    new
                    {
                        CallerName = $"AllOverIt.Tests.Diagnostics.Breadcrumbs.Extensions.BreadcrumbsExtensionsFixture+{nameof(Add_With_Caller_Message)}.{callerName}",
                        FilePath = GetCallerFilePath(),
                        LineNumber = GetCallerLineNumber() - 10,
                        Message = message,
                        Metadata = (object)null,
                        Tags = Array.Empty<string>()
                    }
                };

                actual.First().FilePath.ShouldNotBeNullOrEmpty();
                actual.First().LineNumber.ShouldBeGreaterThan(0);

                actual.ShouldBeEquivalentTo(expected, opts => opts.ExcludeMember(nameof(BreadcrumbData.Timestamp)).ExcludeMember(nameof(BreadcrumbData.TimestampUtc)));
            }
        }

        public class Add_With_Caller_Message_Metadata : BreadcrumbsExtensionsFixture
        {
            private readonly object _metadata;

            public Add_With_Caller_Message_Metadata()
            {
                _metadata = new
                {
                    Prop1 = Create<int>(),
                    Prop2 = Create<string>()
                };
            }

            [Fact]
            public void Should_Throw_When_Breadcrumbs_Null()
            {
                Invoking(() => BreadcrumbsExtensions.Add(null, this, Create<string>(), _metadata))
                  .ShouldThrow<ArgumentNullException>()
                  .WithNamedMessageWhenNull("breadcrumbs");
            }

            [Fact]
            public void Should_Throw_When_Caller_Null()
            {
                Invoking(() => BreadcrumbsExtensions.Add(_breadcrumbs, (object) null, Create<string>(), _metadata))
                  .ShouldThrow<ArgumentNullException>()
                  .WithNamedMessageWhenNull("caller");
            }

            [Fact]
            public void Should_Throw_When_Message_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue => BreadcrumbsExtensions.Add(_breadcrumbs, this, stringValue, _metadata),
                    "message");
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Add_Breadcrumb(bool includeMetadata)
            {
                var message = Create<string>();
                object metadata = includeMetadata ? Create<int>() : null;

                BreadcrumbsExtensions.Add(_breadcrumbs, this, message, _metadata);

                var actual = _breadcrumbs.ToList();

                var expected = new[]
                {
                    new
                    {
                        CallerName = $"AllOverIt.Tests.Diagnostics.Breadcrumbs.Extensions.BreadcrumbsExtensionsFixture+{nameof(Add_With_Caller_Message_Metadata)}.{nameof(Should_Add_Breadcrumb)}",
                        FilePath = GetCallerFilePath(),
                        LineNumber = GetCallerLineNumber() - 10,
                        Message = message,
                        Metadata = _metadata,
                        Tags = Array.Empty<string>()
                    }
                };

                actual.First().FilePath.ShouldNotBeNullOrEmpty();
                actual.First().LineNumber.ShouldBeGreaterThan(0);

                actual.ShouldBeEquivalentTo(expected, opts => opts.ExcludeMember(nameof(BreadcrumbData.Timestamp)).ExcludeMember(nameof(BreadcrumbData.TimestampUtc)));
            }

            [Fact]
            public void Should_Add_Breadcrumb_Empty_CallerName()
            {
                var message = Create<string>();

                BreadcrumbsExtensions.Add(_breadcrumbs, this, message, _metadata, string.Empty);

                var actual = _breadcrumbs.ToList();

                var expected = new[]
                {
                    new
                    {
                        CallerName = $"AllOverIt.Tests.Diagnostics.Breadcrumbs.Extensions.BreadcrumbsExtensionsFixture+{nameof(Add_With_Caller_Message_Metadata)}",
                        FilePath = GetCallerFilePath(),
                        LineNumber = GetCallerLineNumber() - 10,
                        Message = message,
                        Metadata = _metadata,
                        Tags = Array.Empty<string>()
                    }
                };

                actual.First().FilePath.ShouldNotBeNullOrEmpty();
                actual.First().LineNumber.ShouldBeGreaterThan(0);

                actual.ShouldBeEquivalentTo(expected, opts => opts.ExcludeMember(nameof(BreadcrumbData.Timestamp)).ExcludeMember(nameof(BreadcrumbData.TimestampUtc)));
            }

            [Fact]
            public void Should_Add_Breadcrumb_Custom_CallerName()
            {
                var message = Create<string>();
                var callerName = Create<string>();

                BreadcrumbsExtensions.Add(_breadcrumbs, this, message, _metadata, callerName);

                var actual = _breadcrumbs.ToList();

                var expected = new[]
                {
                    new
                    {
                        CallerName = $"AllOverIt.Tests.Diagnostics.Breadcrumbs.Extensions.BreadcrumbsExtensionsFixture+{nameof(Add_With_Caller_Message_Metadata)}.{callerName}",
                        FilePath = GetCallerFilePath(),
                        LineNumber = GetCallerLineNumber() - 10,
                        Message = message,
                        Metadata = _metadata,
                        Tags = Array.Empty<string>()
                    }
                };

                actual.First().FilePath.ShouldNotBeNullOrEmpty();
                actual.First().LineNumber.ShouldBeGreaterThan(0);

                actual.ShouldBeEquivalentTo(expected, opts => opts.ExcludeMember(nameof(BreadcrumbData.Timestamp)).ExcludeMember(nameof(BreadcrumbData.TimestampUtc)));
            }
        }

        // Edge cases that cannot be tested via the public methods
        public class AddBreadcrumb : BreadcrumbsExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Caller_Null_CallerName_Not_Null()
            {
                Invoking(() =>
                {
                    BreadcrumbsExtensions.AddBreadcrumb(_breadcrumbs, null, Create<string>(), Create<string>(), Create<string>());
                })
                .ShouldThrow<InvalidOperationException>()
                .WithMessage("Cannot have a null caller instance.");
            }

            [Fact]
            public void Should_Throw_When_Caller_Not_Null_CallerName_Null()
            {
                Invoking(() =>
                {
                    BreadcrumbsExtensions.AddBreadcrumb(_breadcrumbs, this, Create<string>(), Create<string>(), null);
                })
                .ShouldThrow<InvalidOperationException>()
                .WithMessage("Cannot have a null caller name.");
            }
        }

        private static string GetCallerFilePath([CallerFilePath] string filePath = "")
        {
            return filePath;
        }

        private static int GetCallerLineNumber([CallerLineNumber] int lineNumber = 0)
        {
            return lineNumber;
        }
    }
}







