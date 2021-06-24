using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Tests.Operations.Dummies;
using AllOverIt.Fixture;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace AllOverIt.Evaluator.Tests.Operations
{
    public class UserDefinedMethodFactoryFixture : FixtureBase
    {
        private UserDefinedMethodFactory _factory;

        public class Constructor : UserDefinedMethodFactoryFixture
        {
            [Fact]
            public void Should_Registry_Built_In_Methods()
            {
                var registry = new Dictionary<string, Lazy<ArithmeticOperationBase>>();
                _factory = new UserDefinedMethodFactory(registry);

                registry.Keys.Should().BeEquivalentTo(new[]
                {
                    "ROUND", "SQRT", "LOG", "LN", "EXP", "PERC", "SIN", "COS", "TAN", "SINH", "COSH", "TANH", "ASIN", "ACOS", "ATAN"
                });
            }

            [Theory]
            [InlineData("ROUND", typeof(RoundOperation))]
            [InlineData("SQRT", typeof(SqrtOperation))]
            [InlineData("LOG", typeof(LogOperation))]
            [InlineData("LN", typeof(LnOperation))]
            [InlineData("EXP", typeof(ExpOperation))]
            [InlineData("PERC", typeof(PercentOperation))]
            [InlineData("SIN", typeof(SinOperation))]
            [InlineData("COS", typeof(CosOperation))]
            [InlineData("TAN", typeof(TanOperation))]
            [InlineData("SINH", typeof(SinhOperation))]
            [InlineData("COSH", typeof(CoshOperation))]
            [InlineData("TANH", typeof(TanhOperation))]
            [InlineData("ASIN", typeof(AsinOperation))]
            [InlineData("ACOS", typeof(AcosOperation))]
            [InlineData("ATAN", typeof(AtanOperation))]
            public void Should_Registry_Built_In_Method_Operations(string name, Type operationType)
            {
                var registry = new Dictionary<string, Lazy<ArithmeticOperationBase>>();
                _factory = new UserDefinedMethodFactory(registry);

                // only here to make sure the test cases are updated if a new operation is added
                registry.Keys.Should().HaveCount(15);

                var operation = _factory.GetMethod(name);

                operation.Should().BeOfType(operationType);
            }
        }

        public class RegisterMethod : UserDefinedMethodFactoryFixture
        {
            public RegisterMethod()
            {
                _factory = new UserDefinedMethodFactory();
            }

            [Fact]
            public void Should_Return_As_Not_Registered()
            {
                var result = _factory.IsRegistered(Create<string>());

                result.Should().BeFalse();
            }

            [Fact]
            public void Should_Return_As_Registered()
            {
                var name = Create<string>();

                _factory.RegisterMethod<ArithmeticOperationDummy>(name);

                var result = _factory.IsRegistered(name);

                result.Should().BeTrue();
            }

            [Fact]
            public void Should_Register_Method_Case_Insensitive()
            {
                var name = Create<string>().ToLower();

                _factory.RegisterMethod<ArithmeticOperationDummy>(name.ToUpper());

                var operation = _factory.GetMethod(name);

                operation.Should().BeOfType<ArithmeticOperationDummy>();
            }
        }

        public class GetMethod : UserDefinedMethodFactoryFixture
        {
            public GetMethod()
            {
                _factory = new UserDefinedMethodFactory();
            }

            [Fact]
            public void Should_Get_Method()
            {
                var name = Create<string>();

                _factory.RegisterMethod<ArithmeticOperationDummy>(name);

                var operation = _factory.GetMethod(name);

                operation.Should().BeOfType<ArithmeticOperationDummy>();
            }

            [Fact]
            public void Should_Get_Method_Case_Insensitive()
            {
                var name = Create<string>().ToLower();

                _factory.RegisterMethod<ArithmeticOperationDummy>(name);

                var operation = _factory.GetMethod(name.ToUpper());

                operation.Should().BeOfType<ArithmeticOperationDummy>();
            }

            [Fact]
            public void Should_Throw_When_Not_Registered()
            {
                var name = Create<string>();

                Invoking(() => _factory.GetMethod(name))
                    .Should()
                    .Throw<KeyNotFoundException>()
                    .WithMessage($"The given key '{name}' was not present in the dictionary.");
            }
        }
    }
}
