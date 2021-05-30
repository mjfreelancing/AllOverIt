using System;
using Xunit;

namespace AllOverIt.Evaluator.Tests
{
    public class AoiFormulaCompilerFixtureFunctionalPrecedence : AoiFormulaCompilerFixtureFunctionalBase
    {
        private readonly double _val3;

        public AoiFormulaCompilerFixtureFunctionalPrecedence()
        {
            _val3 = Create<double>();
        }

        [Fact]
        public void Should_Have_Priority_First_Multiply()
        {
            AssertFormula($"{_val1} * {_val2} + {_val3}", () => _val1 * _val2 + _val3);
        }

        [Fact]
        public void Should_Have_Priority_Second_Multiply()
        {
            AssertFormula($"{_val1} - {_val2} * {_val3}", () => _val1 - _val2 * _val3);
        }

        [Fact]
        public void Should_Have_Priority_Both_Multiply()
        {
            AssertFormula(
              string.Format("{0} * {1} + {1} * {2}", _val1, _val2, _val3),
              () => _val1 * _val2 + _val2 * _val3);
        }

        [Fact]
        public void Should_Have_Priority_Parentheses()
        {
            AssertFormula(
              string.Format("{0} * ({1} + {1}) * {2}", _val1, _val2, _val3),
              () => _val1 * (_val2 + _val2) * _val3);
        }

        [Fact]
        public void Should_Have_Priority_Exponent()
        {
            AssertFormula(
              string.Format("{1} * {0} ^ 3", _val1, _val2),
              () => _val2 * Math.Pow(_val1, 3));
        }

        [Fact]
        public void Should_Have_Priority_Exponent_With_Unary_Minus()
        {
            AssertFormula(
              string.Format("- {0} ^ 2", _val1),
              () => -Math.Pow(_val1, 2));
        }

        [Fact]
        public void Should_Have_Priority_Exponent_With_Unary_Plus()
        {
            AssertFormula(
              string.Format("+ {0} ^ 2", _val1),
              () => +Math.Pow(_val1, 2));
        }

        [Fact]
        public void Should_Have_Equal_Priority()
        {
            AssertFormula(
              string.Format("{1} % 9 / {0}", _val1, _val2),
              () => _val2 % 9 / _val1);
        }

        [Fact]
        public void Should_Calculate_Mixed_1()
        {
            AssertFormula(
              string.Format("3 * -{0}^3 * {1}^2 + {2}", _val1, _val2, _val3),
              () => 3 * -Math.Pow(_val1, 3) * Math.Pow(_val2, 2) + _val3);
        }

        [Fact]
        public void Should_Calculate_Mixed_2()
        {
            AssertFormula(
              string.Format("3 * -{0}^2 * -{1}^3 * {2}", _val1, _val2, _val3),
              () => 3 * -Math.Pow(_val1, 2) * -Math.Pow(_val2, 3) * _val3);
        }

        [Fact]
        public void Should_Calculate_Mixed_3()
        {
            AssertFormula(
              string.Format("PERC(-{0}, {1})", _val1, _val2),
              () => -_val1 * 100 / _val2);
        }

        [Fact]
        public void Should_Calculate_Mixed_4()
        {
            AssertFormula(
              string.Format("PERC({0}, -{1})", _val1, _val2),
              () => _val1 * 100 / -_val2);
        }

        [Fact]
        public void Should_Calculate_Mixed_5()
        {
            AssertFormula(
              string.Format("PERC(-{0}, -{1})", _val1, _val2),
              () => -_val1 * 100 / -_val2);
        }

        [Fact]
        public void Should_Calculate_Mixed_6()
        {
            AssertFormula(
              string.Format("2 * {0} - {2} * -3.5 - {1}^2 + ROUND({0}+{1}-{2}, 3) - -5E-3", _val1, _val2, _val3),
              () => 2 * _val1 - _val3 * -3.5 - Math.Pow(_val2, 2) + Math.Round(_val1 + _val2 - _val3, 3, MidpointRounding.AwayFromZero) - -5E-3);
        }
    }
}
