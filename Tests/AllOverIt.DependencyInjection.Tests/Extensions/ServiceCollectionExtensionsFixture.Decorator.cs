using System;
using System.Linq;
using System.Collections.Generic;
using AllOverIt.DependencyInjection.Extensions;
using AllOverIt.Extensions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using AllOverIt.DependencyInjection.Tests.Helpers;

namespace AllOverIt.DependencyInjection.Tests.Extensions
{
    public partial class ServiceCollectionExtensionsFixture
    {
        private interface IDummyInterface
        {
        }

        private sealed class Dummy1 : IDummyInterface
        {
        }

        private sealed class Dummy2 : IDummyInterface
        {
        }

        private sealed class DummyDecorator : IDummyInterface
        {
            public IDummyInterface Decorated{ get; }

            public DummyDecorator(IDummyInterface dummy)
            {
                Decorated = dummy;
            }
        }

        public class Decorator : ServiceCollectionExtensionsFixture
        {
            [Fact]
            public void Should_Decorate_Registered_Services()
            {
                var services = new ServiceCollection();

                services.AddSingleton<IDummyInterface, Dummy1>();
                services.AddSingleton<IDummyInterface, Dummy2>();

                ServiceCollectionExtensions.Decorate<IDummyInterface, DummyDecorator>(services);

                var provider = services.BuildServiceProvider();

                var instances = provider
                    .GetService<IEnumerable<IDummyInterface>>()
                    .AsReadOnlyCollection();

                var decoratedTypes = new List<Type>();

                foreach (var instance in instances)
                {
                    instance.Should().BeOfType<DummyDecorator>();

                    var decorator = instance as DummyDecorator;

                    decoratedTypes.Add(decorator!.Decorated.GetType());
                }

                decoratedTypes.Should().BeEquivalentTo(new[] {typeof(Dummy1), typeof(Dummy2)});
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton, true, true)]
            [InlineData(ServiceLifetime.Scoped, true, false)]
            [InlineData(ServiceLifetime.Transient, false, false)]
            public void Should_Decorate_With_Same_LifeTime(ServiceLifetime lifetime, bool sameScopeExpected, bool differentScopeExpected)
            {
                var services = new ServiceCollection();

                switch (lifetime)
                {
                    case ServiceLifetime.Singleton:
                        services.AddSingleton<IDummyInterface, Dummy1>();
                        services.AddSingleton<IDummyInterface, Dummy2>();
                        break;

                    case ServiceLifetime.Scoped:
                        services.AddScoped<IDummyInterface, Dummy1>();
                        services.AddScoped<IDummyInterface, Dummy2>();
                        break;

                    case ServiceLifetime.Transient:
                        services.AddTransient<IDummyInterface, Dummy1>();
                        services.AddTransient<IDummyInterface, Dummy2>();
                        break;
                }
                
                ServiceCollectionExtensions.Decorate<IDummyInterface, DummyDecorator>(services);

                var provider = services.BuildServiceProvider();

                var instances1a = provider
                    .GetService<IEnumerable<IDummyInterface>>()
                    .AsReadOnlyCollection();

                var instances1b = provider
                    .GetService<IEnumerable<IDummyInterface>>()
                    .AsReadOnlyCollection();

                DependencyHelper.AssertInstanceEquality(instances1a, instances1b, sameScopeExpected);

                using (var scope = provider.CreateScope())
                {
                    var scopedProvider = scope.ServiceProvider;

                    var instances2 = scopedProvider
                        .GetService<IEnumerable<IDummyInterface>>()
                        .AsReadOnlyCollection();

                    DependencyHelper.AssertInstanceEquality(instances1a, instances2, differentScopeExpected);
                    DependencyHelper.AssertInstanceEquality(instances1b, instances2, differentScopeExpected);
                }
            }
        }
    }
}