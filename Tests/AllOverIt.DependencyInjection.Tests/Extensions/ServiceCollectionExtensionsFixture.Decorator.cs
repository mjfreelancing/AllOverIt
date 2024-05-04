using AllOverIt.Aspects;
using AllOverIt.DependencyInjection.Exceptions;
using AllOverIt.DependencyInjection.Extensions;
using AllOverIt.DependencyInjection.Tests.Helpers;
using AllOverIt.Extensions;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace AllOverIt.DependencyInjection.Tests.Extensions
{
    public partial class ServiceCollectionExtensionsFixture
    {
        private interface IDummyDecoratorInterface
        {
            void SetValue(int value);
        }

        private sealed class DummyDecorator1 : IDummyDecoratorInterface
        {
            public void SetValue(int value) { }
        }

        private sealed class DummyDecorator2 : IDummyDecoratorInterface
        {
            public void SetValue(int value) { }
        }

        private sealed class DummyDecorator3 : IDummyDecoratorInterface
        {
            public IDummyDecoratorInterface Decorated { get; }

            public DummyDecorator3(IDummyDecoratorInterface dummy)
            {
                Decorated = dummy;
            }

            public void SetValue(int value) { }
        }

        public class Decorator : ServiceCollectionExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_ServiceCollection_Null()
            {
                Invoking(() =>
                {
                    _ = ServiceCollectionExtensions.Decorate<IDummyDecoratorInterface, DummyDecorator3>(null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("serviceCollection");
            }

            [Fact]
            public void Should_Decorate_Registered_Services()
            {
                var services = new ServiceCollection();

                services.AddSingleton<IDummyDecoratorInterface, DummyDecorator1>();
                services.AddSingleton<IDummyDecoratorInterface, DummyDecorator2>();

                ServiceCollectionExtensions.Decorate<IDummyDecoratorInterface, DummyDecorator3>(services);

                var provider = services.BuildServiceProvider();

                var instances = provider
                    .GetService<IEnumerable<IDummyDecoratorInterface>>()
                    .AsReadOnlyCollection();

                var decoratedTypes = new List<Type>();

                foreach (var instance in instances)
                {
                    instance.Should().BeOfType<DummyDecorator3>();

                    var decorator = instance as DummyDecorator3;

                    decoratedTypes.Add(decorator!.Decorated.GetType());
                }

                decoratedTypes.Should().BeEquivalentTo(new[] { typeof(DummyDecorator1), typeof(DummyDecorator2) });
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
                        services.AddSingleton<IDummyDecoratorInterface, DummyDecorator1>();
                        services.AddSingleton<IDummyDecoratorInterface, DummyDecorator2>();
                        break;

                    case ServiceLifetime.Scoped:
                        services.AddScoped<IDummyDecoratorInterface, DummyDecorator1>();
                        services.AddScoped<IDummyDecoratorInterface, DummyDecorator2>();
                        break;

                    case ServiceLifetime.Transient:
                        services.AddTransient<IDummyDecoratorInterface, DummyDecorator1>();
                        services.AddTransient<IDummyDecoratorInterface, DummyDecorator2>();
                        break;
                }

                ServiceCollectionExtensions.Decorate<IDummyDecoratorInterface, DummyDecorator3>(services);

                var provider = services.BuildServiceProvider();

                var instances1a = provider
                    .GetService<IEnumerable<IDummyDecoratorInterface>>()
                    .AsReadOnlyCollection();

                var instances1b = provider
                    .GetService<IEnumerable<IDummyDecoratorInterface>>()
                    .AsReadOnlyCollection();

                DependencyHelper.AssertInstanceEquality(instances1a, instances1b, sameScopeExpected);

                using (var scope = provider.CreateScope())
                {
                    var scopedProvider = scope.ServiceProvider;

                    var instances2 = scopedProvider
                        .GetService<IEnumerable<IDummyDecoratorInterface>>()
                        .AsReadOnlyCollection();

                    DependencyHelper.AssertInstanceEquality(instances1a, instances2, differentScopeExpected);
                    DependencyHelper.AssertInstanceEquality(instances1b, instances2, differentScopeExpected);
                }
            }

            [Fact]
            public void Should_Throw_When_No_Interface_Registered()
            {
                Invoking(() =>
                {
                    var services = new ServiceCollection();

                    ServiceCollectionExtensions.Decorate<IDummyDecoratorInterface, DummyDecorator3>(services);
                })
                .Should()
                .Throw<DependencyRegistrationException>()
                .WithMessage($"No registered services found for the type '{typeof(IDummyDecoratorInterface).GetFriendlyName()}'.");
            }

            [Fact]
            public void Should_Resolve_Implementation_Instance()
            {
                var services = new ServiceCollection();

                var expected = new DummyDecorator1();

                // only applicable to case ServiceLifetime.Singleton:
                services.AddSingleton<IDummyDecoratorInterface>(expected);

                ServiceCollectionExtensions.Decorate<IDummyDecoratorInterface, DummyDecorator3>(services);

                var provider = services.BuildServiceProvider();

                var actual = provider.GetRequiredService<IDummyDecoratorInterface>() as DummyDecorator3;

                actual.Decorated.Should().BeSameAs(expected);
            }

            [Fact]
            public void Should_Resolve_Implementation_Factory()
            {
                var services = new ServiceCollection();

                // only applicable to case ServiceLifetime.Singleton:
                services.AddSingleton<IDummyDecoratorInterface>(_ => new DummyDecorator1());

                ServiceCollectionExtensions.Decorate<IDummyDecoratorInterface, DummyDecorator3>(services);

                var provider = services.BuildServiceProvider();

                var actual = provider.GetRequiredService<IDummyDecoratorInterface>() as DummyDecorator3;

                actual.Decorated.Should().BeOfType<DummyDecorator1>();
            }

        }

        public class DecorateWithInterceptor : ServiceCollectionExtensionsFixture
        {
            private class DummyInterceptor : InterceptorBase<IDummyDecoratorInterface>
            {
                public Func<int, int> Callback { get; set; }
                public int Actual { get; private set; }

                protected override InterceptorState BeforeInvoke(MethodInfo targetMethod, ref object[] args)
                {
                    args[0] = Callback.Invoke((int) args[0]);

                    return base.BeforeInvoke(targetMethod, ref args);
                }

                protected override void AfterInvoke(MethodInfo targetMethod, object[] args, InterceptorState state)
                {
                    base.AfterInvoke(targetMethod, args, state);

                    Actual = (int) args[0];
                }
            }

            [Fact]
            public void Should_Throw_When_ServiceCollection_Null()
            {
                Invoking(() =>
                {
                    _ = ServiceCollectionExtensions.DecorateWithInterceptor<IDummyDecoratorInterface, DummyInterceptor>(null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("serviceCollection");
            }

            [Fact]
            public void Should_Not_Throw_When_Configure_Null()
            {
                var services = new ServiceCollection();

                services.AddSingleton<IDummyDecoratorInterface, DummyDecorator1>();

                Invoking(() =>
                {
                    _ = ServiceCollectionExtensions.DecorateWithInterceptor<IDummyDecoratorInterface, DummyInterceptor>(services, null);
                })
                .Should()
                .NotThrow();
            }

            [Fact]
            public void Should_Return_Same_Services()
            {
                var services = new ServiceCollection();

                services.AddSingleton<IDummyDecoratorInterface, DummyDecorator1>();

                var actual = ServiceCollectionExtensions.DecorateWithInterceptor<IDummyDecoratorInterface, DummyInterceptor>(services, null);

                actual.Should().BeSameAs(services);
            }

            [Fact]
            public void Should_Configure_Interceptor()
            {
                var services = new ServiceCollection();

                services.AddSingleton<IDummyDecoratorInterface, DummyDecorator1>();

                DummyInterceptor actual = default;

                _ = ServiceCollectionExtensions.DecorateWithInterceptor<IDummyDecoratorInterface, DummyInterceptor>(services, (_, interceptor) =>
                {
                    actual = interceptor;
                });

                var provider = services.BuildServiceProvider();

                _ = provider.GetRequiredService<IDummyDecoratorInterface>();

                actual.Should().BeAssignableTo(typeof(IDummyDecoratorInterface));
            }

            [Fact]
            public void Should_Resolve_As_Interceptor()
            {
                var services = new ServiceCollection();

                services.AddSingleton<IDummyDecoratorInterface, DummyDecorator1>();

                int actual = 0;

                int updater(int value)
                {
                    actual = value;
                    return value;
                }

                _ = ServiceCollectionExtensions.DecorateWithInterceptor<IDummyDecoratorInterface, DummyInterceptor>(services, (_, interceptor) =>
                {
                    interceptor.Callback = updater;
                });

                var provider = services.BuildServiceProvider();

                var service = provider.GetRequiredService<IDummyDecoratorInterface>();

                var expected = Create<int>();

                service.SetValue(expected);

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Modify_Input_Args()
            {
                var services = new ServiceCollection();

                services.AddSingleton<IDummyDecoratorInterface, DummyDecorator1>();

                int updater(int value)
                {
                    return value * 2;
                }

                _ = ServiceCollectionExtensions.DecorateWithInterceptor<IDummyDecoratorInterface, DummyInterceptor>(services, (_, interceptor) =>
                {
                    // Used to double the input argument
                    interceptor.Callback = updater;
                });

                var provider = services.BuildServiceProvider();

                var service = provider.GetRequiredService<IDummyDecoratorInterface>();

                var input = Create<int>();
                var expected = input * 2;

                service.SetValue(input);

                (service as DummyInterceptor).Actual.Should().Be(expected);
            }
        }
    }
}