using FluentAssertions;
using Xunit;

namespace AllOverIt.Evaluator.Tests
{
    public class AoiFormulaCompilerFixtureFunctionalDivision : AoiFormulaCompilerFixtureFunctionalBase
    {
        [Fact]
        public void Should_Divide()
        {
            AssertFormula($"{_val1} / {_val2}", () => _val1 / _val2);
        }

        [Fact]
        public void Should_Divide_Unary_Minus_First()
        {
            AssertFormula($"-{_val1} / {_val2}", () => -_val1 / _val2);
        }

        [Fact]
        public void Should_Divide_Unary_Minus_Second()
        {
            AssertFormula($"{_val1} / -{_val2}", () => _val1 / -_val2);
        }

        [Fact]
        public void Should_Divide_Unary_Minus_Both()
        {
            AssertFormula($"-{_val1} / -{_val2}", () => -_val1 / -_val2);
        }

        [Fact]
        public void Should_Not_Throw_When_Divide_By_Zero()
        {
            // The current implementation returns 'Infinity'
            Invoking(() => AssertFormula($"{_val1} / 0", () => _val1 / 0))
                .Should()
                .NotThrow();
        }

        [Fact]
        public void Should_Return_Infinity_When_Divide_By_Zero()
        {
            AssertFormula($"{_val1} / 0", () => double.PositiveInfinity);
        }
    }
}
