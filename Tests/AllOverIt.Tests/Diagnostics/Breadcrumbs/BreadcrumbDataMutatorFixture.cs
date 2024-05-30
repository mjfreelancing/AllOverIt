using AllOverIt.Diagnostics.Breadcrumbs;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;

namespace AllOverIt.Tests.Diagnostics.Breadcrumbs
{
    public class BreadcrumbDataMutatorFixture : FixtureBase
    {
        private readonly BreadcrumbData _breadcrumbData;
        private readonly IBreadcrumbDataMutator _mutator;

        public BreadcrumbDataMutatorFixture()
        {
            _breadcrumbData = Create<BreadcrumbData>();
            _mutator = new BreadcrumbDataMutator(_breadcrumbData);
        }

        public class Constructor : BreadcrumbDataMutatorFixture
        {
            [Fact]
            public void Should_Throw_When_BreadcrumbData_Null()
            {
                Invoking(() =>
                {
                    _ = new BreadcrumbDataMutator(null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("breadcrumbData");
            }
        }

        public class WithMetadata : BreadcrumbDataMutatorFixture
        {
            [Fact]
            public void Should_Mutate_Metadata()
            {
                var current = _breadcrumbData.Metadata;

                _mutator.WithMetadata(Create<string>());

                current
                    .Should()
                    .NotBeSameAs(_breadcrumbData.Metadata);
            }

            [Fact]
            public void Should_Null_Metadata()
            {
                _breadcrumbData.Metadata.Should().NotBeNull();

                _mutator.WithMetadata(null);

                _breadcrumbData.Metadata.Should().BeNull();
            }
        }

        public class WithTag : BreadcrumbDataMutatorFixture
        {
            [Fact]
            public void Should_Mutate_Tags()
            {
                var tag = Create<string>();
                var current = _breadcrumbData.Tags;

                _mutator.WithTag(tag);

                current
                    .Should()
                    .BeEquivalentTo([tag]);
            }

            [Fact]
            public void Should_Null_Tags()
            {
                _breadcrumbData.Tags.Should().NotBeNull();

                _mutator.WithTag(null);

                _breadcrumbData.Tags.Should().BeNull();
            }
        }

        public class WithTags : BreadcrumbDataMutatorFixture
        {
            [Fact]
            public void Should_Mutate_Tags()
            {
                var tags = CreateMany<string>();
                var current = _breadcrumbData.Tags;

                _mutator.WithTags([.. tags]);

                current
                    .Should()
                    .BeEquivalentTo(tags);
            }

            [Fact]
            public void Should_Null_Tags()
            {
                _breadcrumbData.Tags.Should().NotBeNull();

                _mutator.WithTags(null);

                _breadcrumbData.Tags.Should().BeNull();
            }
        }
    }
}
