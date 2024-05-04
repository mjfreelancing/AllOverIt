namespace AllOverIt.Evaluator.Tests
{
    public class FormulaCompilerFixtureFunctionalTangent : FormulaCompilerFixtureFunctionalBase
    {
        [Fact]
        public void Should_Return_Tangent()
        {
            AssertFormula($"tan({Val1})", () => Math.Tan(Val1));
        }
    }
}