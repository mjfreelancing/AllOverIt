using System;
using Xunit;

namespace AllOverIt.Evaluator.Tests
{
    public class FormulaCompilerFixtureFunctionalTangentAngle : FormulaCompilerFixtureFunctionalBase
    {
        [Fact]
        public void Should_Return_Tangent()
        {
            var angle = Math.Tan(Val1);

            AssertFormula($"atan({angle})", () => Math.Atan(angle));
        }
    }
}