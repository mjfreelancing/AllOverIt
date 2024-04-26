using AllOverIt.DependencyInjection.Extensions;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace AllOverIt.DependencyInjection.Tests.Extensions
{
    public partial class ServiceCollectionExtensionsFixture
    {
        public class AddNamedServices : ServiceCollectionExtensionsFixture
        {
            private interface IDummyInterface
            {
            }

            private sealed class DummyType : IDummyInterface
            {
            }

            [Fact]
            public void Should_Throw_When_ServiceCollection_Null()
            {
                Invoking(() =>
                {
                    _ = ServiceCollectionExtensions.AddNamedServices<IDummyInterface>(null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("serviceCollection");
            }

            [Fact]
            public void Should_Return_INamedServiceBuilder()
            {
                var services = new ServiceCollection();

                var actual = ServiceCollectionExtensions.AddNamedServices<IDummyInterface>(services);

                actual.Should().BeAssignableTo<INamedServiceBuilder<IDummyInterface>>();
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Register_Multiple_Names_To_Same_Implementation(ServiceLifetime lifetime)
            {
                var services = new ServiceCollection();

                var name1 = Create<string>();
                var name2 = Create<string>();

                // This test also extends code coverage to ensure the same service / implementation
                // registration combinations are not duplicated.
                var builder = ServiceCollectionExtensions.AddNamedServices<IDummyInterface>(services);

                switch (lifetime)
                {
                    case ServiceLifetime.Singleton:
                        builder = builder
                            .AsSingleton<DummyType>(name1)
                            .AsSingleton<DummyType>(name2);
                        break;

                    case ServiceLifetime.Scoped:
                        builder = builder
                            .AsScoped<DummyType>(name1)
                            .AsScoped<DummyType>(name2);
                        break;

                    case ServiceLifetime.Transient:
                        builder = builder
                            .AsTransient<DummyType>(name1)
                            .AsTransient<DummyType>(name2);
                        break;
                }

                var provider = services.BuildServiceProvider();

                var actual1 = provider.GetRequiredNamedService<IDummyInterface>(name1);
                var actual2 = provider.GetRequiredNamedService<IDummyInterface>(name2);

                actual1.Should().BeOfType<DummyType>();
                actual2.Should().BeOfType<DummyType>();
            }

            [Fact]
            public void Should_Register_INamedServiceResolver_TService_Singleton()
            {
                var services = new ServiceCollection();

                _ = ServiceCollectionExtensions.AddNamedServices<IDummyInterface>(services);

                var descriptor = services.Single(descriptor => descriptor.ServiceType == typeof(INamedServiceResolver<IDummyInterface>));

                descriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Set_Provider_On_INamedServiceResolver_TService_Generic(ServiceLifetime lifetime)
            {
                var services = new ServiceCollection();

                var builder = ServiceCollectionExtensions.AddNamedServices<IDummyInterface>(services);

                var name = Create<string>();

                switch (lifetime)
                {
                    case ServiceLifetime.Singleton:
                        builder.AsSingleton<DummyType>(name);
                        break;

                    case ServiceLifetime.Scoped:
                        builder.AsScoped<DummyType>(name);
                        break;

                    case ServiceLifetime.Transient:
                        builder.AsTransient<DummyType>(name);
                        break;
                }

                var provider = services.BuildServiceProvider();

                var resolver = provider.GetRequiredService<INamedServiceResolver<IDummyInterface>>() as NamedServiceResolver<IDummyInterface>;

                resolver._provider.Should().BeAssignableTo<IServiceProvider>();
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Set_Provider_On_INamedServiceResolver_TService_Type(ServiceLifetime lifetime)
            {
                var services = new ServiceCollection();

                var builder = ServiceCollectionExtensions.AddNamedServices<IDummyInterface>(services);

                var name = Create<string>();

                switch (lifetime)
                {
                    case ServiceLifetime.Singleton:
                        builder.AsSingleton(name, typeof(DummyType));
                        break;

                    case ServiceLifetime.Scoped:
                        builder.AsScoped(name, typeof(DummyType));
                        break;

                    case ServiceLifetime.Transient:
                        builder.AsTransient(name, typeof(DummyType));
                        break;
                }

                var provider = services.BuildServiceProvider();

                var resolver = provider.GetRequiredService<INamedServiceResolver<IDummyInterface>>() as NamedServiceResolver<IDummyInterface>;

                resolver._provider.Should().BeAssignableTo<IServiceProvider>();
            }

            [Fact]
            public void Should_Register_INamedServiceResolver_Singleton()
            {
                var services = new ServiceCollection();

                _ = ServiceCollectionExtensions.AddNamedServices<IDummyInterface>(services);

                var descriptor = services.Single(descriptor => descriptor.ServiceType == typeof(INamedServiceResolver));

                descriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
            }
        }
    }
}