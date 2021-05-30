using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Tests.Operations.Dummies;
using AllOverIt.Fixture;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace AllOverIt.Evaluator.Tests.Operations
{
    public class AoiUserDefinedMethodFactoryFixture : AoiFixtureBase
    {
        private AoiUserDefinedMethodFactory _factory;

        public class Constructor : AoiUserDefinedMethodFactoryFixture
        {
            [Fact]
            public void Should_Registry_Built_In_Methods()
            {
                var registry = new Dictionary<string, Lazy<AoiArithmeticOperationBase>>();
                _factory = new AoiUserDefinedMethodFactory(registry);

                registry.Keys.Should().BeEquivalentTo(new[]
                {
                    "ROUND", "SQRT", "LOG", "LN", "EXP", "PERC", "SIN", "COS", "TAN", "SINH", "COSH", "TANH", "ASIN", "ACOS", "ATAN"
                });
            }

            [Theory]
            [InlineData("ROUND", typeof(AoiRoundOperation))]
            [InlineData("SQRT", typeof(AoiSqrtOperation))]
            [InlineData("LOG", typeof(AoiLogOperation))]
            [InlineData("LN", typeof(AoiLnOperation))]
            [InlineData("EXP", typeof(AoiExpOperation))]
            [InlineData("PERC", typeof(AoiPercentOperation))]
            [InlineData("SIN", typeof(AoiSinOperation))]
            [InlineData("COS", typeof(AoiCosOperation))]
            [InlineData("TAN", typeof(AoiTanOperation))]
            [InlineData("SINH", typeof(AoiSinhOperation))]
            [InlineData("COSH", typeof(AoiCoshOperation))]
            [InlineData("TANH", typeof(AoiTanhOperation))]
            [InlineData("ASIN", typeof(AoiAsinOperation))]
            [InlineData("ACOS", typeof(AoiAcosOperation))]
            [InlineData("ATAN", typeof(AoiAtanOperation))]
            public void Should_Registry_Built_In_Method_Operations(string name, Type operationType)
            {
                var registry = new Dictionary<string, Lazy<AoiArithmeticOperationBase>>();
                _factory = new AoiUserDefinedMethodFactory(registry);

                // only here to make sure the test cases are updated if a new operation is added
                registry.Keys.Should().HaveCount(15);

                var operation = _factory.GetMethod(name);

                operation.Should().BeOfType(operationType);
            }
        }

        public class RegisterMethod : AoiUserDefinedMethodFactoryFixture
        {
            public RegisterMethod()
            {
                _factory = new AoiUserDefinedMethodFactory();
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

                _factory.RegisterMethod<AoiArithmeticOperationDummy>(name);

                var result = _factory.IsRegistered(name);

                result.Should().BeTrue();
            }

            [Fact]
            public void Should_Register_Method_Case_Insensitive()
            {
                var name = Create<string>().ToLower();

                _factory.RegisterMethod<AoiArithmeticOperationDummy>(name.ToUpper());

                var operation = _factory.GetMethod(name);

                operation.Should().BeOfType<AoiArithmeticOperationDummy>();
            }
        }

        public class GetMethod : AoiUserDefinedMethodFactoryFixture
        {
            public GetMethod()
            {
                _factory = new AoiUserDefinedMethodFactory();
            }

            [Fact]
            public void Should_Get_Method()
            {
                var name = Create<string>();

                _factory.RegisterMethod<AoiArithmeticOperationDummy>(name);

                var operation = _factory.GetMethod(name);

                operation.Should().BeOfType<AoiArithmeticOperationDummy>();
            }

            [Fact]
            public void Should_Get_Method_Case_Insensitive()
            {
                var name = Create<string>().ToLower();

                _factory.RegisterMethod<AoiArithmeticOperationDummy>(name);

                var operation = _factory.GetMethod(name.ToUpper());

                operation.Should().BeOfType<AoiArithmeticOperationDummy>();
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
