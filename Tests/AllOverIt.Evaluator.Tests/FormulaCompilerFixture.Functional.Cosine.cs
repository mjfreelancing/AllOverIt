using System;
using Xunit;

namespace AllOverIt.Evaluator.Tests
{
    public class FormulaCompilerFixtureFunctionalCosine : FormulaCompilerFixtureFunctionalBase
    {
        [Fact]
        public void Should_Return_Cosine()
        {
            AssertFormula($"cos({Val1})", () => Math.Cos(Val1));
        }
    }
}