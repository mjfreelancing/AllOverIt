using Xunit;

namespace AllOverIt.Evaluator.Tests
{
    public class AoiFormulaCompilerFixtureFunctionalSubtraction : AoiFormulaCompilerFixtureFunctionalBase
    {
        [Fact]
        public void Should_Subtract()
        {
            AssertFormula($"{_val1} - {_val2}", () => _val1 - _val2);
        }

        [Fact]
        public void Should_Subtract_Unary_Minus_First()
        {
            AssertFormula($"-{_val1} - {_val2}", () => -_val1 - _val2);
        }

        [Fact]
        public void Should_Subtract_Unary_Minus_Second()
        {
            AssertFormula($"{_val1} - -{_val2}", () => _val1 - -_val2);
        }

        [Fact]
        public void Should_Subtract_Unary_Minus_Both()
        {
            AssertFormula($"-{_val1} - -{_val2}", () => -_val1 - -_val2);
        }
    }
}
