using Xunit;

namespace AllOverIt.Evaluator.Tests
{
    public class FormulaCompilerFixtureFunctionalIf : FormulaCompilerFixtureFunctionalBase
    {
        [Fact]
        public void Should_Return_If_True()
        {
            AssertFormula($"IF(EQ(1,1),{Val1},{Val2})", () => Val1);
        }

        [Fact]
        public void Should_Return_If_False()
        {
            AssertFormula($"IF(EQ(1,2),{Val1},{Val2})", () => Val2);
        }
    }
}