using AllOverIt.Aspects.Interceptor;
using AllOverIt.Extensions;
using AllOverIt.Fixture;
using Castle.DynamicProxy;
using FluentAssertions;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace AllOverIt.Tests.Aspects
{
    public class InterceptorBaseFixture : FixtureBase
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
                return Task.FromResult(GetValue(value, shouldThrow));
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

            protected override InterceptorState BeforeInvoke(MethodInfo targetMethod, ref object[] args, ref object result)
            {
                var value = (string) (args[0]);

                if (LowerBeforeValue)
                {
                    value = value.ToLowerInvariant();
                }

                // Would return InterceptorState.Unit if no state is required
                _state = new DummyState(value);

                return _state;
            }

            protected override void AfterInvoke(MethodInfo targetMethod, object[] args, InterceptorState state, ref object result)
            {
                var value = (string) (args[0]);

                ((DummyState) state).AfterValue = UpperAfterValue ? value.ToUpperInvariant() : value;
            }

            protected override void Faulted(MethodInfo targetMethod, object[] args, InterceptorState state, Exception exception)
            {
                ((DummyState) state).Fault = exception;
            }
        }

        // Must be non-sealed
        public class DummyInterceptor2 : InterceptorBase<IDummyService>
        {
            private class DummyInterceptorState : InterceptorState
            {
            }

            // Determines how values are assigned to DummyState when calling BeforeInvoke() and AfterInvoke()
            public object HandleBeforeResult { get; set; }
            public object HandleAfterResult { get; set; }

            protected override InterceptorState BeforeInvoke(MethodInfo targetMethod, ref object[] args, ref object result)
            {
                var state = new DummyInterceptorState();

                if (HandleBeforeResult is not null)
                {
                    result = HandleBeforeResult;
                    state.Handled = true;
                }

                return state;
            }

            protected override void AfterInvoke(MethodInfo targetMethod, object[] args, InterceptorState state, ref object result)
            {
                if (HandleAfterResult is not null)
                {
                    result = HandleAfterResult;
                }
            }
        }

        // Must be non-sealed
        public class DummyInterceptor3 : InterceptorBase<IDummyService>
        {
            public string AfterArgs { get; set; }

            protected override InterceptorState BeforeInvoke(MethodInfo targetMethod, ref object[] args, ref object result)
            {
                args[0] = ((string) args[0]).ToUpper();

                return InterceptorState.None;
            }

            protected override void AfterInvoke(MethodInfo targetMethod, object[] args, InterceptorState state, ref object result)
            {
                AfterArgs = (string) args[0];
            }
        }

        // Must be non-sealed
        public class DummyInterceptor4 : InterceptorBase<IDummyService>
        {
            public string BeforeArgs { get; set; }
            public string AfterArgs { get; set; }

            protected override InterceptorState BeforeInvoke(MethodInfo targetMethod, ref object[] args, ref object result)
            {
                BeforeArgs = (string) args[0];

                return InterceptorState.None;
            }

            protected override void AfterInvoke(MethodInfo targetMethod, object[] args, InterceptorState state, ref object result)
            {
                AfterArgs = (string) args[0];
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Should_Call_Before_Invoke(bool lowerBeforeValue)
        {
            var (proxiedService, actualInterceptor) = CreateDummyInterceptor1(interceptor =>
            {
                interceptor.LowerBeforeValue = lowerBeforeValue;
            });

            var input = Create<string>();
            var value = $"A{input}b";

            var expectedBefore = lowerBeforeValue ? value.ToLowerInvariant() : value;

            var actual = proxiedService.GetValue(value, false);

            actualInterceptor._state.BeforeValue.Should().Be(expectedBefore);
            actualInterceptor._state.Fault.Should().BeNull();

            actual.Should().Be(value);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task Should_Call_Before_Invoke_Async(bool lowerBeforeValue)
        {
            var (proxiedService, actualInterceptor) = CreateDummyInterceptor1(interceptor =>
            {
                interceptor.LowerBeforeValue = lowerBeforeValue;
            });

            var input = Create<string>();
            var value = $"A{input}b";

            var expectedBefore = lowerBeforeValue ? value.ToLowerInvariant() : value;

            var actual = await proxiedService.GetValueAsync(value, false);

            actualInterceptor._state.BeforeValue.Should().Be(expectedBefore);
            actualInterceptor._state.Fault.Should().BeNull();

            actual.Should().Be(value);
        }

        [Fact]
        public void Should_Mutate_Input_Args()
        {
            var (proxiedService, actualInterceptor) = CreateDummyInterceptor3();

            var input = Create<string>().ToLower();
            var expected = input.ToUpper();

            var actual = proxiedService.GetValue(input, false);

            actual.Should().Be(expected);
        }

        [Fact]
        public async Task Should_Mutate_Input_Args_Async()
        {
            var (proxiedService, actualInterceptor) = CreateDummyInterceptor3();

            var input = Create<string>().ToLower();
            var expected = input.ToUpper();

            var actual = await proxiedService.GetValueAsync(input, false);

            actual.Should().Be(expected);
        }

        [Fact]
        public void Should_Handle_When_Before_Invoke()
        {
            var expected = Create<string>();

            var (proxiedService, actualInterceptor) = CreateDummyInterceptor2(interceptor =>
            {
                interceptor.HandleBeforeResult = expected;
            });

            var input = Create<string>();
            var value = $"A{input}b";

            var actual = proxiedService.GetValue(value, false);

            actual.Should().Be(expected);
        }

        [Fact]
        public async Task Should_Handle_When_Before_InvokeAsync()
        {
            var expected = Create<string>();

            var (proxiedService, actualInterceptor) = CreateDummyInterceptor2(interceptor =>
            {
                interceptor.HandleBeforeResult = Task.FromResult(expected);
            });

            var input = Create<string>();
            var value = $"A{input}b";

            var actual = await proxiedService.GetValueAsync(value, false);

            actual.Should().Be(expected);
        }

        [Fact]
        public void Should_Return_Invoke_Result()
        {
            var (proxiedService, actualInterceptor) = CreateDummyInterceptor1();

            var input = Create<string>();
            var value = $"A{input}b";

            var actual = proxiedService.GetValue(value, false);

            actualInterceptor._state.Fault.Should().BeNull();
            actual.Should().Be(value);
        }

        [Fact]
        public async Task Should_Return_Invoke_Result_Async()
        {
            var (proxiedService, actualInterceptor) = CreateDummyInterceptor1();

            var input = Create<string>();
            var value = $"A{input}b";

            var actual = await proxiedService.GetValueAsync(value, false);

            actualInterceptor._state.Fault.Should().BeNull();
            actual.Should().Be(value);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Should_Call_After_Invoke(bool upperAfterValue)
        {
            var (proxiedService, actualInterceptor) = CreateDummyInterceptor1(interceptor =>
            {
                interceptor.UpperAfterValue = upperAfterValue;
            });

            var input = Create<string>();
            var value = $"A{input}b";

            var expectedAfter = upperAfterValue ? value.ToUpperInvariant() : value;

            var actual = proxiedService.GetValue(value, false);

            actualInterceptor._state.AfterValue.Should().Be(expectedAfter);
            actualInterceptor._state.Fault.Should().BeNull();

            actual.Should().Be(value);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task Should_Call_After_Invoke_Async(bool upperAfterValue)
        {
            var (proxiedService, actualInterceptor) = CreateDummyInterceptor1(interceptor =>
            {
                interceptor.UpperAfterValue = upperAfterValue;
            });

            var input = Create<string>();
            var value = $"A{input}b";

            var expectedAfter = upperAfterValue ? value.ToUpperInvariant() : value;

            var actual = await proxiedService.GetValueAsync(value, false);

            actualInterceptor._state.AfterValue.Should().Be(expectedAfter);
            actualInterceptor._state.Fault.Should().BeNull();

            actual.Should().Be(value);
        }

        [Fact]
        public void Should_Handle_When_After_Invoke()
        {
            var expected = Create<string>();

            var (proxiedService, actualInterceptor) = CreateDummyInterceptor2(interceptor =>
            {
                interceptor.HandleAfterResult = expected;
            });

            var input = Create<string>();
            var value = $"A{input}b";

            var actual = proxiedService.GetValue(value, false);

            actual.Should().Be(expected);
        }

        [Fact]
        public async Task Should_Handle_When_After_InvokeAsync()
        {
            var expected = Create<string>();

            var (proxiedService, actualInterceptor) = CreateDummyInterceptor2(interceptor =>
            {
                interceptor.HandleAfterResult = Task.FromResult(expected);
            });

            var input = Create<string>();
            var value = $"A{input}b";

            var actual = await proxiedService.GetValueAsync(value, false);

            actual.Should().Be(expected);
        }

        [Fact]
        public void Should_Default_Fault()      // When the Fault() method is not overriden
        {
            var (proxiedService, actualInterceptor) = CreateDummyInterceptor2();

            try
            {
                _ = proxiedService.GetValue(Create<string>(), true);

                Assert.Fail("The invocation should have faulted");
            }
            catch(Exception exception)
            {
                exception.Message.Should().Be("Dummy Exception");
            }
        }

        [Fact]
        public void Should_Fault()
        {
            var (proxiedService, actualInterceptor) = CreateDummyInterceptor1();

            try
            {
                _ = proxiedService.GetValue(Create<string>(), true);

                Assert.Fail("The invocation should have faulted");
            }
            catch (Exception exception)
            {
                exception.Message.Should().Be("Dummy Exception");
                actualInterceptor._state.Fault.Should().BeSameAs(exception);
            }
        }

        [Fact]
        public async Task Should_Fault_Async()
        {
            var (proxiedService, actualInterceptor) = CreateDummyInterceptor1();

            try
            {
                _ = await proxiedService.GetValueAsync(Create<string>(), true);

                Assert.Fail("The invocation should have faulted");
            }
            catch (Exception exception)
            {
                exception.Message.Should().Be("Dummy Exception");
                actualInterceptor._state.Fault.Should().BeSameAs(exception);
            }
        }

        [Fact]
        public void Should_Invoke_Method_With_Void_Return_Type()
        {
            var (proxiedService, actualInterceptor) = CreateDummyInterceptor4();

            actualInterceptor.BeforeArgs.Should().BeNull();
            actualInterceptor.AfterArgs.Should().BeNull();

            var expected = Create<string>();

            proxiedService.SetValue(expected);

            actualInterceptor.BeforeArgs.Should().Be(expected);
            actualInterceptor.AfterArgs.Should().Be(expected);
        }

        [Fact]
        public async Task Should_Invoke_Method_With_Task_Return_Type()
        {
            var (proxiedService, actualInterceptor) = CreateDummyInterceptor4();

            actualInterceptor.BeforeArgs.Should().BeNull();
            actualInterceptor.AfterArgs.Should().BeNull();

            var expected = Create<string>();

            await proxiedService.SetValueAsync(expected);

            actualInterceptor.BeforeArgs.Should().Be(expected);
            actualInterceptor.AfterArgs.Should().Be(expected);
        }

        private static (IDummyService, DummyInterceptor1) CreateDummyInterceptor1(Action<DummyInterceptor1> configure = default)
        {
            var service = new DummyService();

            // Interceptors cannot be new'd up - can only be created via this factory method.
            // This method returns a proxied IDummyService this is a DummyInterceptor.
            var proxy = InterceptorFactory.CreateInterceptor<IDummyService, DummyInterceptor1>(service, configure);

            return (proxy, (DummyInterceptor1) proxy);
        }

        private static (IDummyService, DummyInterceptor2) CreateDummyInterceptor2(Action<DummyInterceptor2> configure = default)
        {
            var service = new DummyService();

            // Interceptors cannot be new'd up - can only be created via this factory method.
            // This method returns a proxied IDummyService this is a DummyInterceptor.
            var proxy = InterceptorFactory.CreateInterceptor<IDummyService, DummyInterceptor2>(service, configure);

            return (proxy, (DummyInterceptor2) proxy);
        }

        private static (IDummyService, DummyInterceptor3) CreateDummyInterceptor3()
        {
            var service = new DummyService();

            // Interceptors cannot be new'd up - can only be created via this factory method.
            // This method returns a proxied IDummyService this is a DummyInterceptor.
            var proxy = InterceptorFactory.CreateInterceptor<IDummyService, DummyInterceptor3>(service);

            return (proxy, (DummyInterceptor3) proxy);
        }

        private static (IDummyService, DummyInterceptor4) CreateDummyInterceptor4()
        {
            var service = new DummyService();

            // Interceptors cannot be new'd up - can only be created via this factory method.
            // This method returns a proxied IDummyService this is a DummyInterceptor.
            var proxy = InterceptorFactory.CreateInterceptor<IDummyService, DummyInterceptor4>(service);

            return (proxy, (DummyInterceptor4) proxy);
        }
    }
}