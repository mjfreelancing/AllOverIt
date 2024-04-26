using AllOverIt.Extensions;
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

            public void Method1(int arg1) => Arg = arg1;
            private void Method2(int arg1) => Arg = arg1;
            protected void Method3(int arg1, string arg2) => Arg = arg1;
            private void Method3(int arg1, string arg2, bool arg3) => Arg = arg1;
        }

        private readonly DummyInvokeClass _dummyClass = new();

        public class InvokeMethod_Name_Parameters_Bindings : ObjectExtensionsFixture
        {
            [Fact]
            public void Should_Invoke_Public_Method()
            {
                var expected = Create<int>();

                _dummyClass.InvokeMethod(nameof(DummyInvokeClass.Method1), [expected], BindingOptions.Default);

                _dummyClass.Arg.Should().Be(expected);
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
        }

        public class InvokeMethod_MethodInfo_Parameters : ObjectExtensionsFixture
        {
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