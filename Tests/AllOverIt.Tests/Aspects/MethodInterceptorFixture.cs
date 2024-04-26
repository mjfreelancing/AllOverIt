using AllOverIt.Aspects;
using AllOverIt.Assertion;
using AllOverIt.Fixture;
using FluentAssertions;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace AllOverIt.Tests.Aspects
{
    public class MethodInterceptorFixture : FixtureBase
    {
        public interface IDummyService
        {
            string GetValue(bool canThrow = false);
            Task<string> GetValueAsync(bool canThrow = false);

            void SetValue(bool canThrow = false);
            Task SetValueAsync(bool canThrow = false);
        }

        private sealed class DummyService : IDummyService
        {
            public bool GetValueCalled { get; private set; }
            public bool GetValueAsyncCalled { get; private set; }
            public bool SetValueCalled { get; private set; }
            public bool SetValueAsyncCalled { get; private set; }

            public string GetValue(bool canThrow = false)
            {
                Throw<Exception>.When(canThrow, "GetValue has thrown.");

                GetValueCalled = true;

                return string.Empty;
            }

            public Task<string> GetValueAsync(bool canThrow = false)
            {
                Throw<Exception>.When(canThrow, "GetValueAsync has thrown.");

                GetValueAsyncCalled = true;

                return Task.FromResult(string.Empty);
            }

            public void SetValue(bool canThrow = false)
            {
                Throw<Exception>.When(canThrow, "SetValue has thrown.");

                SetValueCalled = true;
            }

            public Task SetValueAsync(bool canThrow = false)
            {
                Throw<Exception>.When(canThrow, "SetValueAsync has thrown.");

                SetValueAsyncCalled = true;

                return Task.CompletedTask;
            }
        }

        // InterceptorHandlerBase<T> (over InterceptorHandlerBase) is suitable for methods with a T return type.
        private sealed class GetValueHandler : InterceptorMethodHandlerBase<string>
        {
            public bool BeforeHandlerCalled { get; private set; }
            public bool AfterHandlerCalled { get; private set; }
            public Exception Exception { get; private set; }

            public override MethodInfo[] TargetMethods { get; } = [typeof(IDummyService).GetMethod(nameof(IDummyService.GetValue))];

            protected override InterceptorState<string> BeforeMethodInvoke(MethodInfo targetMethod, ref object[] args)
            {
                BeforeHandlerCalled = true;

                return base.BeforeMethodInvoke(targetMethod, ref args);
            }

            protected override void AfterMethodInvoke(MethodInfo targetMethod, object[] args, InterceptorState<string> state)
            {
                base.AfterMethodInvoke(targetMethod, args, state);

                AfterHandlerCalled = true;
            }

            protected override void MethodFaulted(MethodInfo targetMethod, object[] args, InterceptorState<string> state, Exception exception)
            {
                base.MethodFaulted(targetMethod, args, state, exception);

                Exception = exception;
            }
        }

        // InterceptorMethodHandlerAsyncBase<T> is best suited (over InterceptorHandlerBase) for methods with a Task<T> return type.
        private sealed class GetValueAsyncHandler : InterceptorMethodHandlerAsyncBase<string>
        {
            public bool BeforeHandlerCalled { get; private set; }
            public bool AfterHandlerCalled { get; private set; }
            public Exception Exception { get; private set; }

            public override MethodInfo[] TargetMethods { get; } = [typeof(IDummyService).GetMethod(nameof(IDummyService.GetValueAsync))];

            protected override InterceptorState<Task<string>> BeforeMethodInvoke(MethodInfo targetMethod, ref object[] args)
            {
                BeforeHandlerCalled = true;

                return base.BeforeMethodInvoke(targetMethod, ref args);
            }

            protected override void AfterMethodInvoke(MethodInfo targetMethod, object[] args, InterceptorState<Task<string>> state)
            {
                base.AfterMethodInvoke(targetMethod, args, state);

                AfterHandlerCalled = true;
            }

            protected override void MethodFaulted(MethodInfo targetMethod, object[] args, InterceptorState<Task<string>> state, Exception exception)
            {
                base.MethodFaulted(targetMethod, args, state, exception);

                Exception = exception;
            }
        }

        // InterceptorMethodHandlerBase is suitable for methods with a void return type.
        private sealed class SetValueHandler : InterceptorMethodHandlerBase
        {
            public bool BeforeHandlerCalled { get; private set; }
            public bool AfterHandlerCalled { get; private set; }
            public Exception Exception { get; private set; }

            public override MethodInfo[] TargetMethods { get; } = [typeof(IDummyService).GetMethod(nameof(IDummyService.SetValue))];

            protected override InterceptorState BeforeMethodInvoke(MethodInfo targetMethod, ref object[] args)
            {
                BeforeHandlerCalled = true;

                return base.BeforeMethodInvoke(targetMethod, ref args);
            }

            protected override void AfterMethodInvoke(MethodInfo targetMethod, object[] args, InterceptorState state)
            {
                base.AfterMethodInvoke(targetMethod, args, state);

                AfterHandlerCalled = true;
            }

            protected override void MethodFaulted(MethodInfo targetMethod, object[] args, InterceptorState state, Exception exception)
            {
                base.MethodFaulted(targetMethod, args, state, exception);

                Exception = exception;
            }
        }

        // InterceptorHandlerAsyncBase is best suited (over InterceptorHandlerBase) for methods with a Task return type.
        private sealed class SetValueAsyncHandler : InterceptorMethodHandlerAsyncBase
        {
            public bool BeforeHandlerCalled { get; private set; }
            public bool AfterHandlerCalled { get; private set; }
            public Exception Exception { get; private set; }

            public override MethodInfo[] TargetMethods { get; } = [typeof(IDummyService).GetMethod(nameof(IDummyService.SetValueAsync))];

            protected override InterceptorState<Task> BeforeMethodInvoke(MethodInfo targetMethod, ref object[] args)
            {
                BeforeHandlerCalled = true;

                return base.BeforeMethodInvoke(targetMethod, ref args);
            }

            protected override void AfterMethodInvoke(MethodInfo targetMethod, object[] args, InterceptorState<Task> state)
            {
                base.AfterMethodInvoke(targetMethod, args, state);

                AfterHandlerCalled = true;
            }

            protected override void MethodFaulted(MethodInfo targetMethod, object[] args, InterceptorState<Task> state, Exception exception)
            {
                base.MethodFaulted(targetMethod, args, state, exception);

                Exception = exception;
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
        public void Should_Handle_GetValue_Fault()
        {
            var handler = new GetValueHandler();

            var (service, proxy, methodInterceptor) = CreateMethodInterceptor((_, interceptor) =>
            {
                interceptor.AddMethodHandler(handler);
            });

            Invoking(() =>
            {
                proxy.GetValue(true);
            })
            .Should()
            .Throw<Exception>()
            .WithMessage("GetValue has thrown.");

            handler.Exception.Message.Should().Be("GetValue has thrown.");
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
        public async Task Should_Handle_GetValueAsync_Fault()
        {
            var handler = new GetValueAsyncHandler();

            var (service, proxy, methodInterceptor) = CreateMethodInterceptor((_, interceptor) =>
            {
                interceptor.AddMethodHandler(handler);
            });

            await Invoking(async () =>
            {
                await proxy.GetValueAsync(true);
            })
            .Should()
            .ThrowAsync<Exception>()
            .WithMessage("GetValueAsync has thrown.");

            handler.Exception.Message.Should().Be("GetValueAsync has thrown.");
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
        public void Should_Handle_SetValue_Fault()
        {
            var handler = new SetValueHandler();

            var (service, proxy, methodInterceptor) = CreateMethodInterceptor((_, interceptor) =>
            {
                interceptor.AddMethodHandler(handler);
            });

            Invoking(() =>
            {
                proxy.SetValue(true);
            })
            .Should()
            .Throw<Exception>()
            .WithMessage("SetValue has thrown.");

            handler.Exception.Message.Should().Be("SetValue has thrown.");
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

        [Fact]
        public async Task Should_Handle_SetValueAsync_Fault()
        {
            var handler = new SetValueAsyncHandler();

            var (service, proxy, methodInterceptor) = CreateMethodInterceptor((_, interceptor) =>
            {
                interceptor.AddMethodHandler(handler);
            });

            await Invoking(async () =>
            {
                await proxy.SetValueAsync(true);
            })
            .Should()
            .ThrowAsync<Exception>()
            .WithMessage("SetValueAsync has thrown.");

            handler.Exception.Message.Should().Be("SetValueAsync has thrown.");
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