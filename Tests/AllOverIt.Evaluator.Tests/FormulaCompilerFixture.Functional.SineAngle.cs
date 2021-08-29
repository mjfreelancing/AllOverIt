using System;
using Xunit;

namespace AllOverIt.Evaluator.Tests
{
    public class FormulaCompilerFixtureFunctionalSineAngle : FormulaCompilerFixtureFunctionalBase
    {
        [Fact]
        public void Should_Return_Sine()
        {
            var angle = Math.Sin(Val1);

            AssertFormula($"asin({angle})", () => Math.Asin(angle));
        }
    }
}