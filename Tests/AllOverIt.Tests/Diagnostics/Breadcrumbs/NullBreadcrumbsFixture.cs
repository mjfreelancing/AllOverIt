using AllOverIt.Diagnostics.Breadcrumbs;
using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Patterns.Specification.Extensions;
using FluentAssertions;
using System.Collections;

namespace AllOverIt.Tests.Diagnostics.Breadcrumbs
{
    public class NullBreadcrumbsFixture : FixtureBase
    {
        private readonly NullBreadcrumbs _breadcrumbs = new();

        public class Instance : NullBreadcrumbsFixture
        {
            [Fact]
            public void Should_Return_NullBreadcrumbs()
            {
                var actual = NullBreadcrumbs.Instance;

                actual.Should().BeOfType<NullBreadcrumbs>();
            }
        }

        public class Count : BreadcrumbsFixture
        {
            [Fact]
            public void Should_Be_Empty()
            {
                NullBreadcrumbs.Instance.Count.Should().Be(0);
            }

            [Fact]
            public void Should_Have_Expected_Count()
            {
                var breadcrumbs = NullBreadcrumbs.Instance;

                Enumerable
                    .Range(1, GetWithinRange(10, 20))
                    .ForEach((_, index) =>
                    {
                        var breadcrumb = Create<BreadcrumbData>();

                        breadcrumbs.Add(breadcrumb);
                    });

                breadcrumbs.Count.Should().Be(0);
            }
        }

        public class Enabled : NullBreadcrumbsFixture
        {
            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Set_Enabled(bool enabled)
            {
                _breadcrumbs.Enabled = enabled;

                _breadcrumbs.Enabled.Should().Be(enabled);
            }
        }

        public class StartTimestamp : NullBreadcrumbsFixture
        {
            [Fact]
            public void Should_Return_Default()
            {
                _breadcrumbs.StartTimestamp.Should().Be(default);
            }
        }

        public class Add : NullBreadcrumbsFixture
        {
            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Not_Add(bool enabled)
            {
                _breadcrumbs.Enabled = enabled;

                _breadcrumbs.Add(new BreadcrumbData());

                _breadcrumbs.Any().Should().BeFalse();
            }
        }

        public class Clear : NullBreadcrumbsFixture
        {
            [Fact]
            public void Should_Not_Throw()
            {
                Invoking(() =>
                {
                    _breadcrumbs.Clear();
                })
                .Should()
                .NotThrow();
            }
        }

        public class Reset : NullBreadcrumbsFixture
        {
            [Fact]
            public void Should_Not_Throw()
            {
                Invoking(() =>
                {
                    _breadcrumbs.Reset();
                })
                .Should()
                .NotThrow();
            }
        }

        public class GetEnumerator : NullBreadcrumbsFixture
        {
            [Fact]
            public void Should_Not_Enumerate()
            {
                _breadcrumbs.GetEnumerator().MoveNext().Should().BeFalse();
            }
        }

        public class GetEnumerator_Explicit : NullBreadcrumbsFixture
        {
            [Fact]
            public void Should_Not_Enumerate()
            {
                ((IEnumerable) _breadcrumbs).GetEnumerator().MoveNext().Should().BeFalse();
            }
        }
    }
}
