using Xunit;

namespace AllOverIt.Evaluator.Tests
{
    public class FormulaCompilerFixtureFunctionalAddition : FormulaCompilerFixtureFunctionalBase
    {
        [Fact]
        public void Should_Add()
        {
            AssertFormula($"{_val1} + {_val2}", () => _val1 + _val2);
        }

        [Fact]
        public void Should_Add_Unary_Minus_First()
        {
            AssertFormula($"-{_val1} + {_val2}", () => -_val1 + _val2);
        }

        [Fact]
        public void Should_Add_Unary_Minus_Second()
        {
            AssertFormula($"{_val1} + -{_val2}", () => _val1 + -_val2);
        }

        [Fact]
        public void Should_Add_Unary_Minus_Both()
        {
            AssertFormula($"-{_val1} + -{_val2}", () => -_val1 + -_val2);
        }
    }
}
