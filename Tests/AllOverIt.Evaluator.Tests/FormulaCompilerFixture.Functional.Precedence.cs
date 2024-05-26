namespace AllOverIt.Evaluator.Tests
{
    public class FormulaCompilerFixtureFunctionalPrecedence : FormulaCompilerFixtureFunctionalBase
    {
        private readonly double _val3;

        public FormulaCompilerFixtureFunctionalPrecedence()
        {
            _val3 = Create<double>();
        }

        [Fact]
        public void Should_Have_Priority_First_Multiply()
        {
            AssertFormula($"{Val1} * {Val2} + {_val3}", () => Val1 * Val2 + _val3);
        }

        [Fact]
        public void Should_Have_Priority_Second_Multiply()
        {
            AssertFormula($"{Val1} - {Val2} * {_val3}", () => Val1 - Val2 * _val3);
        }

        [Fact]
        public void Should_Have_Priority_Both_Multiply()
        {
            AssertFormula(
              string.Format("{0} * {1} + {1} * {2}", Val1, Val2, _val3),
              () => Val1 * Val2 + Val2 * _val3);
        }

        [Fact]
        public void Should_Have_Priority_Parentheses()
        {
            AssertFormula(
              string.Format("{0} * ({1} + {1}) * {2}", Val1, Val2, _val3),
              () => Val1 * (Val2 + Val2) * _val3);
        }

        [Fact]
        public void Should_Have_Priority_Exponent()
        {
            AssertFormula(
              string.Format("{1} * {0} ^ 3", Val1, Val2),
              () => Val2 * Math.Pow(Val1, 3));
        }

        [Fact]
        public void Should_Have_Priority_Exponent_With_Unary_Minus()
        {
            AssertFormula(
              string.Format("- {0} ^ 2", Val1),
              () => -Math.Pow(Val1, 2));
        }

        [Fact]
        public void Should_Have_Priority_Exponent_With_Unary_Plus()
        {
            AssertFormula(
              string.Format("+ {0} ^ 2", Val1),
              () => +Math.Pow(Val1, 2));
        }

        [Fact]
        public void Should_Have_Equal_Priority()
        {
            AssertFormula(
              string.Format("{1} % 9 / {0}", Val1, Val2),
              () => Val2 % 9 / Val1);
        }

        [Fact]
        public void Should_Calculate_Mixed_1()
        {
            // Preventing overflow
            var val1 = Val1 % 10 + 1;
            var val2 = Val2 % 10 + 1;
            var val3 = _val3 % 10 + 1;

            AssertFormula(
              string.Format("3 * -{0}^3 * {1}^2 + {2}", val1, val2, val3),
              () => 3 * -Math.Pow(val1, 3) * Math.Pow(val2, 2) + val3);
        }

        [Fact]
        public void Should_Calculate_Mixed_2()
        {
            // Preventing overflow
            var val1 = Val1 % 10 + 1;
            var val2 = Val2 % 10 + 1;
            var val3 = _val3 % 10 + 1;

            AssertFormula(
              string.Format("3 * -{0}^2 * -{1}^3 * {2}", val1, val2, val3),
              () => 3 * -Math.Pow(val1, 2) * -Math.Pow(val2, 3) * val3);
        }

        [Fact]
        public void Should_Calculate_Mixed_3()
        {
            AssertFormula(
              string.Format("PERC(-{0}, {1})", Val1, Val2),
              () => -Val1 * 100 / Val2);
        }

        [Fact]
        public void Should_Calculate_Mixed_4()
        {
            AssertFormula(
              string.Format("PERC({0}, -{1})", Val1, Val2),
              () => Val1 * 100 / -Val2);
        }

        [Fact]
        public void Should_Calculate_Mixed_5()
        {
            AssertFormula(
              string.Format("PERC(-{0}, -{1})", Val1, Val2),
              () => -Val1 * 100 / -Val2);
        }

        [Fact]
        public void Should_Calculate_Mixed_6()
        {
            AssertFormula(
              string.Format("2 * {0} - {2} * -3.5 - {1}^2 + ROUND({0}+{1}-{2}, 3) - -5E-3", Val1, Val2, _val3),
              () => 2 * Val1 - _val3 * -3.5 - Math.Pow(Val2, 2) + Math.Round(Val1 + Val2 - _val3, 3, MidpointRounding.AwayFromZero) - -5E-3);
        }

        [Fact]
        public void Should_Calculate_Mixed_7()
        {
            AssertFormula(
              "(2^3-6)^(2+3*4)",
              () => Math.Pow((Math.Pow(2, 3) - 6), (2 + 3 * 4)));
        }

        [Fact]
        public void Should_Calculate_Mixed_8()
        {
            AssertFormula(
              "2+3*(4^5-6)^(7+8*9)-10",
              () => 2 + 3 * Math.Pow((Math.Pow(4, 5) - 6), (7 + 8 * 9)) - 10);
        }
    }
}
