using Xunit;

namespace AllOverIt.Evaluator.Tests
{
    public class FormulaCompilerFixtureFunctionalMultiplication : FormulaCompilerFixtureFunctionalBase
    {
        [Fact]
        public void Should_Multiply()
        {
            AssertFormula($"{_val1} * {_val2}", () => _val1 * _val2);
        }

        [Fact]
        public void Should_Multiply_Unary_Minus_First()
        {
            AssertFormula($"-{_val1} * {_val2}", () => -_val1 * _val2);
        }

        [Fact]
        public void Should_Multiply_Unary_Minus_Second()
        {
            AssertFormula($"{_val1} * -{_val2}", () => _val1 * -_val2);
        }

        [Fact]
        public void Should_Multiply_Unary_Minus_Both()
        {
            AssertFormula($"-{_val1} * -{_val2}", () => -_val1 * -_val2);
        }
    }
}
