using AllOverIt.Evaluator.Extensions;
using AllOverIt.Evaluator.Variables;
using AllOverIt.Evaluator.Variables.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;

namespace AllOverIt.Evaluator.Tests.Extensions
{
    public class FormulaCompilerExtensionsFixture : FixtureBase
    {
        private readonly FormulaCompiler _formulaCompiler = new();

        public class GetResult : FormulaCompilerExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Compiler_Null()
            {
                Invoking(() =>
                {
                    _ = FormulaCompilerExtensions.GetResult(null, Create<string>());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("compiler");
            }

            [Fact]
            public void Should_Throw_When_Formula_Null()
            {
                Invoking(() =>
                {
                    _ = FormulaCompilerExtensions.GetResult(_formulaCompiler, null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("formula");
            }

            [Fact]
            public void Should_Not_Throw_When_VariableRegistry_Null()
            {
                Invoking(() =>
                {
                    _ = FormulaCompilerExtensions.GetResult(_formulaCompiler, "1+1");
                })
                .Should()
                .NotThrow();
            }

            [Fact]
            public void Should_Return_Result()
            {
                var v1 = Create<int>();
                var v2 = Create<double>();

                var expected = v1 + v2;

                var actual = FormulaCompilerExtensions.GetResult(_formulaCompiler, $"{v1}+{v2}");

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Use_Variable_Registry()
            {
                var variableRegistry = new VariableRegistry();

                var v1 = Create<int>();
                var v2 = Create<double>();

                variableRegistry.AddConstantVariable("v1", v1);
                variableRegistry.AddConstantVariable("v2", v2);

                var expected = v1 + v2;

                var actual = FormulaCompilerExtensions.GetResult(_formulaCompiler, "v1+v2", variableRegistry);

                actual.Should().Be(expected);
            }
        }
    }
}
