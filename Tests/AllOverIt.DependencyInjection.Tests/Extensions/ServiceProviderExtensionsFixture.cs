using AllOverIt.DependencyInjection.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace AllOverIt.DependencyInjection.Tests.Extensions
{
    public class ServiceProviderExtensionsFixture : FixtureBase
    {
        public class GetRequiredNamedService : ServiceProviderExtensionsFixture
        {
            private interface IDummyInterface
            {
            }

            private sealed class DummyType1 : IDummyInterface
            {
            }

            private sealed class DummyType2 : IDummyInterface
            {
            }

            [Fact]
            public void Should_Throw_When_Provider_Null()
            {
                Invoking(() =>
                {
                    _ = ServiceProviderExtensions.GetRequiredNamedService<IDummyInterface>(null, Create<string>());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("provider");
            }

            [Fact]
            public void Should_Throw_When_Name_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(stringValue =>
                {
                    _ = ServiceProviderExtensions.GetRequiredNamedService<IDummyInterface>(this.CreateStub<IServiceProvider>(), stringValue);
                }, "name");
            }

            [Fact]
            public void Should_Resolve_Named_Service()
            {
                var services = new ServiceCollection();

                var builder = services.AddNamedServices<IDummyInterface>();

                builder.AsScoped<DummyType1>(nameof(DummyType1));
                builder.AsScoped<DummyType2>(nameof(DummyType2));

                var provider = services.BuildServiceProvider();

                var type1 = ServiceProviderExtensions.GetRequiredNamedService<IDummyInterface>(provider, nameof(DummyType1));
                type1.Should().BeOfType<DummyType1>();

                var type2 = ServiceProviderExtensions.GetRequiredNamedService<IDummyInterface>(provider, nameof(DummyType2));
                type2.Should().BeOfType<DummyType2>();
            }
        }
    }
}