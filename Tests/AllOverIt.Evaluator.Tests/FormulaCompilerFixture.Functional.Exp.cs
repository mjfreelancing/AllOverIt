namespace AllOverIt.Evaluator.Tests
{
    public class FormulaCompilerFixtureFunctionalExp : FormulaCompilerFixtureFunctionalBase
    {
        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void Should_Return_Exp(double value)
        {
            AssertFormula($"exp({value})", () => Math.Exp(value));
        }
    }
}