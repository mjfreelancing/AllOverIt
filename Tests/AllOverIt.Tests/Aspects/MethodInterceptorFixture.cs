using AllOverIt.Aspects.Interceptor;
using AllOverIt.Fixture;
using Castle.DynamicProxy;
using FluentAssertions;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace AllOverIt.Tests.Aspects
{
    public class MethodInterceptorFixture : FixtureBase
    {
        public interface IDummyService
        {
            string GetValue();
            Task<string> GetValueAsync();

            void SetValue();
            Task SetValueAsync();
        }

        private sealed class DummyService : IDummyService
        {
            public bool GetValueCalled { get; private set; }
            public bool GetValueAsyncCalled { get; private set; }
            public bool SetValueCalled { get; private set; }
            public bool SetValueAsyncCalled { get; private set; }

            public string GetValue()
            {
                GetValueCalled = true;

                return string.Empty;
            }

            public Task<string> GetValueAsync()
            {
                GetValueAsyncCalled = true;

                return Task.FromResult(string.Empty);
            }

            public void SetValue()
            {
                SetValueCalled = true;
            }

            public Task SetValueAsync()
            {
                SetValueAsyncCalled = true;

                return Task.CompletedTask;
            }
        }

        // InterceptorHandlerBase<T> (over InterceptorHandlerBase) is suitable for methods with a T return type.
        private sealed class GetValueHandler : InterceptorHandlerBase<string>
        {
            public bool BeforeHandlerCalled { get; private set; }
            public bool AfterHandlerCalled { get; private set; }

            public override MethodInfo[] TargetMethods { get; } = [typeof(IDummyService).GetMethod(nameof(IDummyService.GetValue))];

            protected override InterceptorState BeforeInvoke(MethodInfo targetMethod, ref object[] args, ref string result)
            {
                BeforeHandlerCalled = true;

                return base.BeforeInvoke(targetMethod, ref args, ref result);
            }

            protected override string AfterInvoke(MethodInfo targetMethod, object[] args, InterceptorState state, string result)
            {
                AfterHandlerCalled = true;

                return base.AfterInvoke(targetMethod, args, state, result);
            }
        }

        // InterceptorHandlerAsyncBase<T> is best suited (over InterceptorHandlerBase) for methods with a Task<T> return type.
        private sealed class GetValueAsyncHandler : InterceptorHandlerAsyncBase<string>
        {
            public bool BeforeHandlerCalled { get; private set; }
            public bool AfterHandlerCalled { get; private set; }

            public override MethodInfo[] TargetMethods { get; } = [typeof(IDummyService).GetMethod(nameof(IDummyService.GetValueAsync))];

            protected override InterceptorState BeforeInvoke(MethodInfo targetMethod, ref object[] args, ref Task<string> result)
            {
                BeforeHandlerCalled = true;

                return base.BeforeInvoke(targetMethod, ref args, ref result);
            }

            protected override async Task<string> AfterInvoke(MethodInfo targetMethod, object[] args, InterceptorState state, Task<string> result)
            {
                AfterHandlerCalled = true;

                // Only putting this in to test the async/await works as desired
                await Task.Delay(1);

                return await base.AfterInvoke(targetMethod, args, state, result);
            }
        }

        // InterceptorHandlerBase is suitable for methods with a void return type.
        private sealed class SetValueHandler : InterceptorHandlerBase
        {
            public bool BeforeHandlerCalled { get; private set; }
            public bool AfterHandlerCalled { get; private set; }

            public override MethodInfo[] TargetMethods { get; } = [typeof(IDummyService).GetMethod(nameof(IDummyService.SetValue))];

            protected override InterceptorState BeforeInvoke(MethodInfo targetMethod, ref object[] args)
            {
                BeforeHandlerCalled = true;

                return base.BeforeInvoke(targetMethod, ref args);
            }

            protected override void AfterInvoke(MethodInfo targetMethod, object[] args, InterceptorState state)
            {
                AfterHandlerCalled = true;

                base.AfterInvoke(targetMethod, args, state);
            }
        }

        // InterceptorHandlerAsyncBase is best suited (over InterceptorHandlerBase) for methods with a Task return type.
        private sealed class SetValueAsyncHandler : InterceptorHandlerAsyncBase
        {
            public bool BeforeHandlerCalled { get; private set; }
            public bool AfterHandlerCalled { get; private set; }

            public override MethodInfo[] TargetMethods { get; } = [typeof(IDummyService).GetMethod(nameof(IDummyService.SetValueAsync))];

            protected override InterceptorState BeforeInvoke(MethodInfo targetMethod, ref object[] args, ref Task result)
            {
                BeforeHandlerCalled = true;

                return base.BeforeInvoke(targetMethod, ref args, ref result);
            }

            protected override Task AfterInvoke(MethodInfo targetMethod, object[] args, InterceptorState state, Task result)
            {
                AfterHandlerCalled = true;

                return base.AfterInvoke(targetMethod, args, state, result);
            }
        }

        [Fact]
        public void Should_Handle_GetValue()
        {
            var handler = new GetValueHandler();

            var (service, proxy, methodInterceptor) = CreateMethodInterceptor((_, interceptor) =>
            {
                interceptor.AddMethodHandler(handler);
            });

            proxy.SetValue();

            // The real service should be called
            service.SetValueCalled.Should().BeTrue();

            // But the method interceptor should not be called
            handler.BeforeHandlerCalled.Should().BeFalse();
            handler.AfterHandlerCalled.Should().BeFalse();

            _ = proxy.GetValue();

            // The real service and the method interceptor should be called
            service.GetValueCalled.Should().BeTrue();
            handler.BeforeHandlerCalled.Should().BeTrue();
            handler.AfterHandlerCalled.Should().BeTrue();
        }

        [Fact]
        public async Task Should_Handle_GetValueAsync()
        {
            var handler = new GetValueAsyncHandler();

            var (service, proxy, methodInterceptor) = CreateMethodInterceptor((_, interceptor) =>
            {
                interceptor.AddMethodHandler(handler);
            });

            await proxy.SetValueAsync();

            // The real service should be called
            service.SetValueAsyncCalled.Should().BeTrue();

            // But the method interceptor should not be called
            handler.BeforeHandlerCalled.Should().BeFalse();
            handler.AfterHandlerCalled.Should().BeFalse();

            _ = await proxy.GetValueAsync();

            // The real service and the method interceptor should be called
            service.GetValueAsyncCalled.Should().BeTrue();
            handler.BeforeHandlerCalled.Should().BeTrue();
            handler.AfterHandlerCalled.Should().BeTrue();
        }

        [Fact]
        public void Should_Handle_SetValue()
        {
            var handler = new SetValueHandler();

            var (service, proxy, methodInterceptor) = CreateMethodInterceptor((_, interceptor) =>
            {
                interceptor.AddMethodHandler(handler);
            });

            _ = proxy.GetValue();

            // The real service should be called
            service.GetValueCalled.Should().BeTrue();

            // But the method interceptor should not be called
            handler.BeforeHandlerCalled.Should().BeFalse();
            handler.AfterHandlerCalled.Should().BeFalse();

            proxy.SetValue();

            // The real service and the method interceptor should be called
            service.SetValueCalled.Should().BeTrue();
            handler.BeforeHandlerCalled.Should().BeTrue();
            handler.AfterHandlerCalled.Should().BeTrue();
        }

        [Fact]
        public async Task Should_Handle_SetValueAsync()
        {
            var handler = new SetValueAsyncHandler();

            var (service, proxy, methodInterceptor) = CreateMethodInterceptor((_, interceptor) =>
            {
                interceptor.AddMethodHandler(handler);
            });

            _ = await proxy.GetValueAsync();

            // The real service should be called
            service.GetValueAsyncCalled.Should().BeTrue();

            // But the method interceptor should not be called
            handler.BeforeHandlerCalled.Should().BeFalse();
            handler.AfterHandlerCalled.Should().BeFalse();

            await proxy.SetValueAsync();

            // The real service and the method interceptor should be called
            service.SetValueAsyncCalled.Should().BeTrue();
            handler.BeforeHandlerCalled.Should().BeTrue();
            handler.AfterHandlerCalled.Should().BeTrue();
        }

        private static (DummyService, IDummyService, MethodInterceptor<IDummyService>) CreateMethodInterceptor(Action<IServiceProvider, MethodInterceptor<IDummyService>> configure)
        {
            var service = new DummyService();

            // Interceptors cannot be new'd up - can only be created via this factory method.
            // This method returns a proxied IDummyService.
            var proxy = InterceptorFactory.CreateInterceptor<IDummyService, MethodInterceptor<IDummyService>>(service, null, configure);

            return (service, proxy, (MethodInterceptor<IDummyService>) proxy);
        }
    }
}