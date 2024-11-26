using AllOverIt.Extensions;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Reflection;
using FluentAssertions;
using System.Reflection;

namespace AllOverIt.Tests.Extensions
{
    public partial class ObjectExtensionsFixture
    {
        private class DummyInvokeClass
        {
            public int Arg { get; private set; }

            public int Method1(int arg1) => Arg = arg1;
            private void Method2(int arg1) => Arg = arg1;
            protected void Method3(int arg1, string arg2) => Arg = arg1;
            private void Method3(int arg1, string arg2, bool arg3) => Arg = arg1;
            public static int Method4(int arg1) => arg1;
        }

        private readonly DummyInvokeClass _dummyClass = new();

        public class InvokeMethod_Name_Parameters_Bindings : ObjectExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Instance_Null()
            {
                Invoking(() =>
                {
                    ObjectExtensions.InvokeMethod((DummyInvokeClass) null, nameof(DummyInvokeClass.Method1), [Create<int>()], BindingOptions.Default);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("instance");
            }

            [Fact]
            public void Should_Throw_When_Method_Name_Null()
            {
                Invoking(() =>
                {
                    _dummyClass.InvokeMethod(null, [Create<int>()], BindingOptions.Default);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("methodName");
            }

            [Fact]
            public void Should_Throw_When_Method_Name_Empty()
            {
                Invoking(() =>
                {
                    _dummyClass.InvokeMethod(string.Empty, [Create<int>()], BindingOptions.Default);
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("methodName");
            }

            [Fact]
            public void Should_Throw_When_Method_Name_Whitespace()
            {
                Invoking(() =>
                {
                    _dummyClass.InvokeMethod("   ", [Create<int>()], BindingOptions.Default);
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("methodName");
            }

            [Fact]
            public void Should_Invoke_Public_Method()
            {
                var expected = Create<int>();

                _dummyClass.InvokeMethod(nameof(DummyInvokeClass.Method1), [expected], BindingOptions.Default);

                _dummyClass.Arg.Should().Be(expected);
            }

            [Fact]
            public void Should_Return_Result()
            {
                var expected = Create<int>();

                var actual = _dummyClass.InvokeMethod(nameof(DummyInvokeClass.Method1), [expected], BindingOptions.Default);

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Invoke_Private_Method()
            {
                var expected = Create<int>();

                _dummyClass.InvokeMethod("Method2", [expected], BindingOptions.Instance | BindingOptions.Private);

                _dummyClass.Arg.Should().Be(expected);
            }
        }

        public class InvokeMethod_Type_Name_Parameters_Bindings : ObjectExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Instance_Null()
            {
                Invoking(() =>
                {
                    ObjectExtensions.InvokeMethod(typeof(DummyInvokeClass), (Type) null, nameof(DummyInvokeClass.Method1), [Create<int>()], BindingOptions.Default);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("instanceType");
            }

            [Fact]
            public void Should_Throw_When_Method_Name_Null()
            {
                Invoking(() =>
                {
                    _dummyClass.InvokeMethod(typeof(DummyInvokeClass), null, [Create<int>()], BindingOptions.Default);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("methodName");
            }

            [Fact]
            public void Should_Throw_When_Method_Name_Empty()
            {
                Invoking(() =>
                {
                    _dummyClass.InvokeMethod(typeof(DummyInvokeClass), string.Empty, [Create<int>()], BindingOptions.Default);
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("methodName");
            }

            [Fact]
            public void Should_Throw_When_Method_Name_Whitespace()
            {
                Invoking(() =>
                {
                    _dummyClass.InvokeMethod(typeof(DummyInvokeClass), "   ", [Create<int>()], BindingOptions.Default);
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("methodName");
            }

            [Fact]
            public void Should_Invoke_Static_Method()
            {
                var expected = Create<int>();

                var actual = ObjectExtensions.InvokeMethod(null, typeof(DummyInvokeClass), nameof(DummyInvokeClass.Method4), [expected], BindingOptions.Default);

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Return_Result()
            {
                var expected = Create<int>();

                var actual = _dummyClass.InvokeMethod(typeof(DummyInvokeClass), nameof(DummyInvokeClass.Method1), [expected], BindingOptions.Default);

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Invoke_Public_Method()
            {
                var expected = Create<int>();

                _dummyClass.InvokeMethod(typeof(DummyInvokeClass), nameof(DummyInvokeClass.Method1), [expected], BindingOptions.Default);

                _dummyClass.Arg.Should().Be(expected);
            }

            [Fact]
            public void Should_Invoke_Private_Method()
            {
                var expected = Create<int>();

                _dummyClass.InvokeMethod(typeof(DummyInvokeClass), "Method2", [expected], BindingOptions.Instance | BindingOptions.Private);

                _dummyClass.Arg.Should().Be(expected);
            }
        }

        public class InvokeMethod_Type_Name_Types_Parameters : ObjectExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Instance_Null()
            {
                Invoking(() =>
                {
                    ObjectExtensions.InvokeMethod(typeof(DummyInvokeClass), (Type) null, nameof(DummyInvokeClass.Method1), [typeof(int)], [Create<int>()]);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("instanceType");
            }

            [Fact]
            public void Should_Throw_When_Method_Name_Null()
            {
                Invoking(() =>
                {
                    _dummyClass.InvokeMethod(typeof(DummyInvokeClass), null, [typeof(int)], [Create<int>()]);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("methodName");
            }

            [Fact]
            public void Should_Throw_When_Method_Name_Empty()
            {
                Invoking(() =>
                {
                    _dummyClass.InvokeMethod(typeof(DummyInvokeClass), string.Empty, [typeof(int)], [Create<int>()]);
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("methodName");
            }

            [Fact]
            public void Should_Throw_When_Method_Name_Whitespace()
            {
                Invoking(() =>
                {
                    _dummyClass.InvokeMethod(typeof(DummyInvokeClass), "   ", [typeof(int)], [Create<int>()]);
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("methodName");
            }

            [Fact]
            public void Should_Invoke_Static_Method()
            {
                var expected = Create<int>();

                var actual = ObjectExtensions.InvokeMethod(null, typeof(DummyInvokeClass), nameof(DummyInvokeClass.Method4), [typeof(int)], [expected]);

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Return_Result()
            {
                var expected = Create<int>();

                var actual = _dummyClass.InvokeMethod(typeof(DummyInvokeClass), nameof(DummyInvokeClass.Method1), [typeof(int)], [expected]);

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Invoke_Protected_Method()
            {
                var expected = Create<int>();

                _dummyClass.InvokeMethod(typeof(DummyInvokeClass), "Method3", [typeof(int), typeof(string)], [expected, Create<string>()]);

                _dummyClass.Arg.Should().Be(expected);
            }

            [Fact]
            public void Should_Invoke_Private_Method()
            {
                var expected = Create<int>();

                _dummyClass.InvokeMethod(typeof(DummyInvokeClass), "Method3", [typeof(int), typeof(string), typeof(bool)], [expected, Create<string>(), Create<bool>()]);

                _dummyClass.Arg.Should().Be(expected);
            }

            [Fact]
            public void Should_Throw_When_Method_Not_Found()
            {
                var methodName = Create<string>();

                Invoking(() =>
                {
                    _dummyClass.InvokeMethod(typeof(DummyInvokeClass), methodName, Type.EmptyTypes, null);

                })
                .Should()
                .Throw<ArgumentException>()
                .WithMessage($"The {methodName} method was not found on type {typeof(DummyInvokeClass).GetFriendlyName()}. (Parameter '{nameof(methodName)}')");
            }
        }

        public class InvokeMethod_MethodInfo_Parameters : ObjectExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_MethodInfo_Null()
            {
                Invoking(() =>
                {
                    _dummyClass.InvokeMethod(null, [Create<int>()]);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("methodInfo");
            }

            [Fact]
            public void Should_Invoke_Static_Method()
            {
                var expected = Create<int>();

                var methodInfo = typeof(DummyInvokeClass).GetMethodInfo("Method4", [typeof(int)]);

                var actual = ObjectExtensions.InvokeMethod(null, methodInfo, [expected]);

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Return_Result()
            {
                var expected = Create<int>();

                var methodInfo = typeof(DummyInvokeClass).GetMethodInfo(nameof(DummyInvokeClass.Method1), [typeof(int)]);

                var actual = _dummyClass.InvokeMethod(methodInfo, [expected]);

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Invoke_Protected_Method()
            {
                var expected = Create<int>();

                var methodInfo = typeof(DummyInvokeClass).GetMethodInfo("Method3", [typeof(int), typeof(string)]);

                _dummyClass.InvokeMethod(methodInfo, [expected, Create<string>()]);

                _dummyClass.Arg.Should().Be(expected);
            }

            [Fact]
            public void Should_Invoke_Private_Method()
            {
                var expected = Create<int>();

                var methodInfo = typeof(DummyInvokeClass).GetMethodInfo("Method3", [typeof(int), typeof(string), typeof(bool)]);

                _dummyClass.InvokeMethod(methodInfo, [expected, Create<string>(), Create<bool>()]);

                _dummyClass.Arg.Should().Be(expected);
            }
        }
    }
}