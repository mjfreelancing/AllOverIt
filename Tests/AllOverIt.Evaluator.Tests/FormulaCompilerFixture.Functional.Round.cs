using System;
using Xunit;

namespace AllOverIt.Evaluator.Tests
{
    public class FormulaCompilerFixtureFunctionalRound : FormulaCompilerFixtureFunctionalBase
    {
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(8)]
        public void Should_Return_Rounded_Value(int decimalPlaces)
        {
            AssertFormula($"round({Val1} / {Val2}, {decimalPlaces})", () => Math.Round(Val1 / Val2, decimalPlaces, MidpointRounding.AwayFromZero));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(8)]
        public void Should_Return_Rounded_Negative_Value(int decimalPlaces)
        {
            AssertFormula($"round(-{Val1} / {Val2}, {decimalPlaces})", () => Math.Round(-Val1 / Val2, decimalPlaces, MidpointRounding.AwayFromZero));
        }
    }
}