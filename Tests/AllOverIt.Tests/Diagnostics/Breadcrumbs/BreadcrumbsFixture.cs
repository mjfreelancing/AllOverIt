using AllOverIt.Diagnostics.Breadcrumbs;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace AllOverIt.Tests.Events
{
    public class BreadcrumbsFixture : FixtureBase
    {
        private readonly Breadcrumbs _breadcrumbs = new();

        public class Add_Message : BreadcrumbsFixture
        {
            [Fact]
            public void Should_Throw_When_Message_Null()
            {
                Invoking(() => _breadcrumbs.Add(null))
                  .Should()
                  .Throw<ArgumentNullException>()
                  .WithNamedMessageWhenNull("message");
            }

            [Fact]
            public void Should_Add_Message_No_Metadata()
            {
                var message = Create<string>();

                _breadcrumbs.Add(message);

                var actual = _breadcrumbs.ToList();

                var expected = new[]
                {
                    new
                    {
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
        }

        public class Add_Message_Metadata : BreadcrumbsFixture
        {
            [Fact]
            public void Should_Throw_When_Message_Null()
            {
                Invoking(() => _breadcrumbs.Add(null, new { }))
                  .Should()
                  .Throw<ArgumentNullException>()
                  .WithNamedMessageWhenNull("message");
            }

            [Fact]
            public void Should_Add_Message_And_Object_Metadata()
            {
                var message = Create<string>();
                var metadata = new { Value = Create<int>() };

                _breadcrumbs.Add(message, metadata);

                var actual = _breadcrumbs.ToList();

                var expected = new[]
                {
                    new
                    {
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
            public void Should_Add_Message_And_Boxed_Metadata()
            {
                var message = Create<string>();
                var metadata = Create<int>();

                _breadcrumbs.Add(message, metadata);

                var actual = _breadcrumbs.ToList();

                var expected = new[]
                {
                    new
                    {
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
                var message1 = Create<string>();
                var metadata1 = new { Value = Create<int>() };

                var message2 = Create<string>();

                var message3 = Create<string>();
                var metadata3 = Create<int>();

                _breadcrumbs.Add(message1, metadata1);
                _breadcrumbs.Add(message2);
                _breadcrumbs.Add(message3, metadata3);

                var actual = _breadcrumbs.ToList();

                var expected = new[]
                {
                    new
                    {
                        Message = message1,
                        Metadata = (object)metadata1
                    },
                    new
                    {
                        Message = message2,
                        Metadata = (object)null
                    },
                    new
                    {
                        Message = message3,
                        Metadata = (object)metadata3
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
    }
}
