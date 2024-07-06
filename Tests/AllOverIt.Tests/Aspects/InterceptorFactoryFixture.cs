using AllOverIt.Aspects;
using AllOverIt.Fixture;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace AllOverIt.Tests.Aspects
{
    public class InterceptorFactoryFixture : FixtureBase
    {
        public interface IDummyService
        {
            string GetValue(string value, bool shouldThrow);
            Task<string> GetValueAsync(string value, bool shouldThrow);

            void SetValue(string value);
            Task SetValueAsync(string value);
        }

        private sealed class DummyService : IDummyService
        {
            public string GetValue(string value, bool shouldThrow)
            {
                if (shouldThrow)
                {
                    throw new Exception("Dummy Exception");
                }

                return value;
            }

            public Task<string> GetValueAsync(string value, bool shouldThrow)
            {
                if (shouldThrow)
                {
                    // Returning a faulted exception rather than throwing (different code coverage)
                    var exception = new Exception("Dummy Exception");
                    return Task.FromException<string>(exception);
                }

                return Task.FromResult(GetValue(value, false));
            }

            public void SetValue(string value)
            {
            }

            public Task SetValueAsync(string value)
            {
                return Task.CompletedTask;
            }
        }

        // Must be non-sealed
        public class DummyInterceptor1 : InterceptorBase<IDummyService>
        {
            // Determines how values are assigned to DummyState when calling BeforeInvoke() and AfterInvoke()
            public bool LowerBeforeValue { get; set; }
            public bool UpperAfterValue { get; set; }

            // Used by the tests to see what happened during before/after invoke
            internal DummyState _state { get; set; }

            internal class DummyState : InterceptorState
            {
                public string BeforeValue { get; }
                public string AfterValue { get; internal set; }
                public Exception Fault { get; internal set; }

                public DummyState(string beforeValue)
                {
                    BeforeValue = beforeValue;
                }
            }

            protected override InterceptorState BeforeInvoke(MethodInfo targetMethod, ref object[] args)
            {
                var value = (string) (args[0]);

                if (LowerBeforeValue)
                {
                    value = value.ToLowerInvariant();
                }

                _state = new DummyState(value);

                return _state;
            }

            protected override void AfterInvoke(MethodInfo targetMethod, object[] args, InterceptorState state)
            {
                var value = (string) (args[0]);

                ((DummyState) state).AfterValue = UpperAfterValue ? value.ToUpperInvariant() : value;
            }

            protected override void Faulted(MethodInfo targetMethod, object[] args, InterceptorState state, Exception exception)
            {
                ((DummyState) state).Fault = exception;
            }
        }

        public class WithoutServiceProvider : InterceptorFactoryFixture
        {
            [Fact]
            public void Should_Not_Throw_When_Configure_Null()
            {
                Invoking(() =>
                {
                    _ = CreateDummyInterceptor<DummyInterceptor1>((Action<DummyInterceptor1>) null);
                })
                .Should()
                .NotThrow();
            }

            [Fact]
            public void Should_Call_Configure()
            {
                var actual = false;

                _ = CreateDummyInterceptor<DummyInterceptor1>(interceptor =>
                {
                    actual = true;
                });

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Provide_Interceptor()
            {
                DummyInterceptor1 actual = null;

                var (proxiedService, actualInterceptor) = CreateDummyInterceptor<DummyInterceptor1>(interceptor =>
                {
                    actual = interceptor;
                });

                actual.Should().BeSameAs(actualInterceptor);
            }

            [Fact]
            public void Should_Decorate_Service()
            {
                var (proxiedService, actualInterceptor) = CreateDummyInterceptor<DummyInterceptor1>(interceptor =>
                {
                });

                actualInterceptor._serviceInstance.Should().BeOfType<DummyService>();
            }
        }

        public class WithServiceProvider : InterceptorFactoryFixture
        {
            private readonly ServiceCollection serviceCollection;
            private readonly ServiceProvider serviceProvider;

            public WithServiceProvider()
            {
                serviceCollection = new ServiceCollection();
                serviceProvider = serviceCollection.BuildServiceProvider();
            }

            [Fact]
            public void Should_Provide_ServiceProvider()
            {
                IServiceProvider actual = null;

                var (proxiedService, actualInterceptor) = CreateDummyInterceptor<DummyInterceptor1>(serviceProvider, (provider, interceptor) =>
                {
                    actual = provider;
                });

                actual.Should().BeSameAs(serviceProvider);
            }

            [Fact]
            public void Should_Not_Throw_When_Configure_Null()
            {
                Invoking(() =>
                {
                    _ = CreateDummyInterceptor<DummyInterceptor1>(null, (Action<IServiceProvider, DummyInterceptor1>) null);
                })
                .Should()
                .NotThrow();
            }

            [Fact]
            public void Should_Call_Configure()
            {
                var actual = false;

                _ = CreateDummyInterceptor<DummyInterceptor1>(serviceProvider, (provider, interceptor) =>
                {
                    actual = true;
                });

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Provide_Interceptor()
            {
                DummyInterceptor1 actual = null;

                var (proxiedService, actualInterceptor) = CreateDummyInterceptor<DummyInterceptor1>(serviceProvider, (provider, interceptor) =>
                {
                    actual = interceptor;
                });

                actual.Should().BeSameAs(actualInterceptor);
            }


            [Fact]
            public void Should_Decorate_Service()
            {
                var (proxiedService, actualInterceptor) = CreateDummyInterceptor<DummyInterceptor1>(serviceProvider, (provider, interceptor) =>
                {
                });

                actualInterceptor._serviceInstance.Should().BeOfType<DummyService>();
            }
        }

        private static (IDummyService, TInterceptor) CreateDummyInterceptor<TInterceptor>(Action<TInterceptor> configure)
            where TInterceptor : InterceptorBase<IDummyService>
        {
            var service = new DummyService();

            // Interceptors cannot be new'd up - can only be created via this factory method.
            // This method returns a proxied IDummyService.
            var proxy = InterceptorFactory.CreateInterceptor<IDummyService, TInterceptor>(service, configure);

            return (proxy, (TInterceptor) proxy);
        }

        private static (IDummyService, TInterceptor) CreateDummyInterceptor<TInterceptor>(IServiceProvider serviceProvider, Action<IServiceProvider, TInterceptor> configure)
            where TInterceptor : InterceptorBase<IDummyService>
        {
            var service = new DummyService();

            // Interceptors cannot be new'd up - can only be created via this factory method.
            // This method returns a proxied IDummyService.
            var proxy = InterceptorFactory.CreateInterceptor<IDummyService, TInterceptor>(service, serviceProvider, configure);

            return (proxy, (TInterceptor) proxy);
        }
    }
}