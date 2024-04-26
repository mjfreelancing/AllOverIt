namespace AllOverIt.Evaluator.Tests
{
    public class FormulaCompilerFixtureFunctionalLog : FormulaCompilerFixtureFunctionalBase
    {
        [Fact]
        public void Should_Return_Log()
        {
            AssertFormula($"log({Val1})", () => Math.Log(Val1));
        }
    }
}