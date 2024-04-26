namespace AllOverIt.Evaluator.Tests
{
    public class FormulaCompilerFixtureFunctionalAddition : FormulaCompilerFixtureFunctionalBase
    {
        [Fact]
        public void Should_Add()
        {
            AssertFormula($"{Val1} + {Val2}", () => Val1 + Val2);
        }

        [Fact]
        public void Should_Add_Unary_Minus_First()
        {
            AssertFormula($"-{Val1} + {Val2}", () => -Val1 + Val2);
        }

        [Fact]
        public void Should_Add_Unary_Minus_Second()
        {
            AssertFormula($"{Val1} + -{Val2}", () => Val1 + -Val2);
        }

        [Fact]
        public void Should_Add_Unary_Minus_Both()
        {
            AssertFormula($"-{Val1} + -{Val2}", () => -Val1 + -Val2);
        }
    }
}
