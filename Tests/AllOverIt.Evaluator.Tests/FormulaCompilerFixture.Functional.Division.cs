using FluentAssertions;
using Xunit;

namespace AllOverIt.Evaluator.Tests
{
    public class FormulaCompilerFixtureFunctionalDivision : FormulaCompilerFixtureFunctionalBase
    {
        [Fact]
        public void Should_Divide()
        {
            AssertFormula($"{Val1} / {Val2}", () => Val1 / Val2);
        }

        [Fact]
        public void Should_Divide_Unary_Minus_First()
        {
            AssertFormula($"-{Val1} / {Val2}", () => -Val1 / Val2);
        }

        [Fact]
        public void Should_Divide_Unary_Minus_Second()
        {
            AssertFormula($"{Val1} / -{Val2}", () => Val1 / -Val2);
        }

        [Fact]
        public void Should_Divide_Unary_Minus_Both()
        {
            AssertFormula($"-{Val1} / -{Val2}", () => -Val1 / -Val2);
        }

        [Fact]
        public void Should_Not_Throw_When_Divide_By_Zero()
        {
            // The current implementation returns 'Infinity'
            Invoking(() => AssertFormula($"{Val1} / 0", () => Val1 / 0))
                .Should()
                .NotThrow();
        }

        [Fact]
        public void Should_Return_Infinity_When_Divide_By_Zero()
        {
            AssertFormula($"{Val1} / 0", () => double.PositiveInfinity);
        }
    }
}
