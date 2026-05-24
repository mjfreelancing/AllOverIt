using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Tests.Operations.Dummies;
using AllOverIt.Fixture;
using Shouldly;
using AllOverIt.Fixture.Extensions;

namespace AllOverIt.Evaluator.Tests.Operations
{
    public class UserDefinedMethodFactoryFixture : FixtureBase
    {
        private UserDefinedMethodFactory _factory;

        public class RegisteredMethods : UserDefinedMethodFactoryFixture
        {
            [Theory]
            [InlineData("ROUND", typeof(RoundOperation))]
            [InlineData("SQRT", typeof(SqrtOperation))]
            [InlineData("CBRT", typeof(CubeRootOperation))]
            [InlineData("LOG10", typeof(Log10Operation))]
            [InlineData("LOG2", typeof(Log2Operation))]
            [InlineData("LOG", typeof(LogOperation))]
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
            [InlineData("MIN", typeof(MinOperation))]
            [InlineData("MAX", typeof(MaxOperation))]
            [InlineData("ABS", typeof(AbsOperation))]
            [InlineData("CEIL", typeof(CeilingOperation))]
            [InlineData("FLOOR", typeof(FloorOperation))]
            [InlineData("IF", typeof(IfOperation))]
            [InlineData("EQ", typeof(EqualOperation))]
            [InlineData("NE", typeof(NotEqualOperation))]
            [InlineData("GT", typeof(GreaterThanOperation))]
            [InlineData("GTE", typeof(GreaterThanOrEqualOperation))]
            [InlineData("LT", typeof(LessThanOperation))]
            [InlineData("LTE", typeof(LessThanOrEqualOperation))]
            public void Should_Registry_Built_In_Method_Operations(string name, Type operationType)
            {
                _factory = new UserDefinedMethodFactory();

                // only here to make sure the test cases are updated if a new operation is added
                _factory.RegisteredMethods.Count().ShouldBe(29);

                var operation = _factory.GetMethod(name);

                operation.ShouldBeOfType(operationType);
            }

            [Fact]
            public void Should_Return_Registered_And_UserDefined_Methods()
            {
                _factory = new UserDefinedMethodFactory();

                var methodName = Create<string>();
                _factory.RegisterMethod<LessThanOrEqualOperation>(methodName);

                _factory.RegisteredMethods.Count().ShouldBe(30);      // 29 built-in plus 1

                _factory.RegisteredMethods.ShouldContain(methodName.ToUpperInvariant());
                _factory.IsRegistered(methodName).ShouldBeTrue();    // The method names are added uppercase - use IsRegistered() as another sanity check
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

                result.ShouldBeFalse();
            }

            [Fact]
            public void Should_Return_As_Registered()
            {
                var name = Create<string>();

                _factory.RegisterMethod<DummyArithmeticOperation>(name);

                var result = _factory.IsRegistered(name);

                result.ShouldBeTrue();
            }

            [Fact]
            public void Should_Register_Method_Case_Insensitive()
            {
                var name = Create<string>().ToLower();

                _factory.RegisterMethod<DummyArithmeticOperation>(name.ToUpper());

                var operation = _factory.GetMethod(name);

                operation.ShouldBeOfType<DummyArithmeticOperation>();
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

                _factory.RegisterMethod<DummyArithmeticOperation>(name);

                var operation = _factory.GetMethod(name);

                operation.ShouldBeOfType<DummyArithmeticOperation>();
            }

            [Fact]
            public void Should_Get_Method_Case_Insensitive()
            {
                var name = Create<string>().ToLower();

                _factory.RegisterMethod<DummyArithmeticOperation>(name);

                var operation = _factory.GetMethod(name.ToUpper());

                operation.ShouldBeOfType<DummyArithmeticOperation>();
            }

            [Fact]
            public void Should_Throw_When_Not_Registered()
            {
                var name = Create<string>();

                Should.Throw<KeyNotFoundException>(() => _factory.GetMethod(name))
                    .WithMessage($"The '{name}' method is not registered with the {nameof(UserDefinedMethodFactory)}.");
            }
        }
    }
}
