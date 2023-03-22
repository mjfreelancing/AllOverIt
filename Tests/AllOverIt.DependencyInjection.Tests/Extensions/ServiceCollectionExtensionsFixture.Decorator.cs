﻿using AllOverIt.Aspects.Interceptor;
using AllOverIt.DependencyInjection.Extensions;
using AllOverIt.DependencyInjection.Tests.Helpers;
using AllOverIt.Extensions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace AllOverIt.DependencyInjection.Tests.Extensions
{
    public partial class ServiceCollectionExtensionsFixture
    {
        private interface IDummyInterface
        {
            void SetValue(int value);
        }

        private sealed class Dummy1 : IDummyInterface
        {
            public void SetValue(int value) { }
        }

        private sealed class Dummy2 : IDummyInterface
        {
            public void SetValue(int value) { }
        }

        private sealed class DummyDecorator : IDummyInterface
        {
            public IDummyInterface Decorated{ get; }

            public DummyDecorator(IDummyInterface dummy)
            {
                Decorated = dummy;
            }

            public void SetValue(int value) { }
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

        public class DecorateWithInterceptor : ServiceCollectionExtensionsFixture
        {
            private class DummyInterceptor : InterceptorBase<IDummyInterface>
            {
                public Action<int> Callback { get; set; }

                protected override InterceptorState BeforeInvoke(MethodInfo targetMethod, object[] args)
                {
                    Callback.Invoke((int)args[0]);

                    return base.BeforeInvoke(targetMethod, args);
                }
            }

            [Fact]
            public void Should_Not_Throw_When_Configure_Null()
            {
                var services = new ServiceCollection();

                services.AddSingleton<IDummyInterface, Dummy1>();

                Invoking(() =>
                {
                    _ = ServiceCollectionExtensions.DecorateWithInterceptor<IDummyInterface, DummyInterceptor>(services, null);
                })
                .Should()
                .NotThrow();
            }

            [Fact]
            public void Should_Return_Same_Services()
            {
                var services = new ServiceCollection();

                services.AddSingleton<IDummyInterface, Dummy1>();

                var actual = ServiceCollectionExtensions.DecorateWithInterceptor<IDummyInterface, DummyInterceptor>(services, null);

                actual.Should().BeSameAs(services);
            }

            [Fact]
            public void Should_Configure_Interceptor()
            {
                var services = new ServiceCollection();

                services.AddSingleton<IDummyInterface, Dummy1>();

                DummyInterceptor actual = default;

                _ = ServiceCollectionExtensions.DecorateWithInterceptor<IDummyInterface, DummyInterceptor>(services, interceptor =>
                {
                    actual = interceptor;
                });

                var provider = services.BuildServiceProvider();

                _ = provider.GetRequiredService<IDummyInterface>();

                actual.Should().BeAssignableTo(typeof(IDummyInterface));
            }

            [Fact]
            public void Should_Resolve_As_Interceptor()
            {
                var services = new ServiceCollection();

                services.AddSingleton<IDummyInterface, Dummy1>();

                int actual = 0;

                Action<int> updater = value => actual = value;

                _ = ServiceCollectionExtensions.DecorateWithInterceptor<IDummyInterface, DummyInterceptor>(services, interceptor =>
                {
                    interceptor.Callback = updater;
                });

                var provider = services.BuildServiceProvider();

                var service = provider.GetRequiredService<IDummyInterface>();

                var expected = Create<int>();

                service.SetValue(expected);

                actual.Should().Be(expected);
            }
        }
    }
}