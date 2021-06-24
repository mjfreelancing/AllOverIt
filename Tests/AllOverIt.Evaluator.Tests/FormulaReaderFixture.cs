using AllOverIt.Evaluator.Exceptions;
using AllOverIt.Evaluator.Operations;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FakeItEasy;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Xunit;

namespace AllOverIt.Evaluator.Tests
{
    public class FormulaReaderFixture : AoiFixtureBase, IDisposable
    {
        private FormulaReader _formulaReader;
        private readonly Fake<TextReader> _textReaderFake;
        private readonly Fake<IArithmeticOperationFactory> _arithmeticOperationFactoryFake;
        private readonly string _formula;

        public FormulaReaderFixture()
        {
            _formula = Create<string>();
            _textReaderFake = new Fake<TextReader>();
            _arithmeticOperationFactoryFake = new Fake<IArithmeticOperationFactory>();

            _formulaReader = new FormulaReader(f => _textReaderFake.FakedObject, _formula);
        }

        public class Constructor : FormulaReaderFixture
        {
            [Fact]
            public void Should_Set_DecimalSeparator()
            {
                var expected = Convert.ToChar(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);

                FormulaReader.DecimalSeparator.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Throw_When_Formula_Null()
            {
                Invoking(() => _formulaReader = new FormulaReader(null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("formula");
            }

            [Fact]
            public void Should_Throw_When_Formula_Empty()
            {
                Invoking(() => _formulaReader = new FormulaReader(string.Empty))
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("formula");
            }

            [Fact]
            public void Should_Throw_When_Formula_Whitespace()
            {
                Invoking(() => _formulaReader = new FormulaReader("  "))
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("formula");
            }

            [Fact]
            public void Should_Throw_When_Reader_Factory_Null()
            {
                Invoking(() => _formulaReader = new FormulaReader(null, Create<string>()))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("readerFactory");
            }
        }

        public class PeekNext : FormulaReaderFixture
        {
            [Fact]
            public void Should_Call_Reader_Peek()
            {
                _formulaReader.PeekNext();

                _textReaderFake
                  .CallsTo(fake => fake.Peek())
                  .MustHaveHappened(1, Times.Exactly);
            }

            [Fact]
            public void Should_Return_Peek_Value()
            {
                var expected = Create<int>();

                _textReaderFake
                  .CallsTo(fake => fake.Peek())
                  .Returns(expected);

                var actual = _formulaReader.PeekNext();

                actual.Should().Be(expected);
            }
        }

        public class ReadNext : FormulaReaderFixture
        {
            [Fact]
            public void Should_Call_Reader_Read()
            {
                _formulaReader.ReadNext();

                _textReaderFake
                  .CallsTo(fake => fake.Read())
                  .MustHaveHappened(1, Times.Exactly);
            }

            [Fact]
            public void Should_Return_Next_Value()
            {
                var expected = Create<int>();

                _textReaderFake
                  .CallsTo(fake => fake.Read())
                  .Returns(expected);

                var actual = _formulaReader.ReadNext();

                actual.Should().Be(expected);
            }
        }

        public class ReadNumerical : FormulaReaderFixture
        {
            private readonly List<int> _charsToRead;

            public ReadNumerical()
            {
                _charsToRead = new List<int>();
            }

            private TType CreateValue<TType>()
            {
                var value = Create<TType>();
                var valueChars = value.ToString().Select(c => (int)c);

                _charsToRead.AddRange(valueChars);

                return value;
            }

            [Fact]
            public void Should_Throw_When_Nothing_To_Read()
            {
                _textReaderFake
                  .CallsTo(fake => fake.Peek())
                  .Returns(-1);

                Invoking(() => _formulaReader.ReadNumerical())
                    .Should()
                    .Throw<FormulaException>()
                    .WithMessage("Nothing to read");
            }

            [Fact]
            public void Should_Throw_When_Starts_With_Non_Digit()
            {
                _textReaderFake
                  .CallsTo(fake => fake.Peek())
                  .ReturnsNextFromSequence('a', -1);

                Invoking(() => _formulaReader.ReadNumerical())
                    .Should()
                    .Throw<FormulaException>()
                    .WithMessage("Unexpected non-numerical token");
            }

            [Fact]
            public void Should_Return_Integral_Value()
            {
                var value = CreateValue<int>();

                _charsToRead.AddRange(new[] { (int)'a' });   // not expected after a numerical but we're only passing numerical values here

                _textReaderFake
                  .CallsTo(fake => fake.Peek())
                  .ReturnsNextFromSequence(_charsToRead.ToArray());

                var actual = _formulaReader.ReadNumerical();

                actual.Should().Be(value);
            }

            [Fact]
            public void Should_Return_Floating_Value()
            {
                var value = CreateValue<double>();
                _charsToRead.AddRange(new[] { (int)'a' });   // not expected after a numerical but we're only passing numerical values here

                _textReaderFake
                  .CallsTo(fake => fake.Peek())
                  .ReturnsNextFromSequence(_charsToRead.ToArray());

                var actual = _formulaReader.ReadNumerical();

                actual.Should().Be(value);
            }

            [Fact]
            public void Should_Return_Positive_Exp_Value()
            {
                var value = CreateValue<int>();
                _charsToRead.AddRange(new[] { 'e', '2', -1 });

                _textReaderFake
                  .CallsTo(fake => fake.Peek())
                  .ReturnsNextFromSequence(_charsToRead.ToArray());

                var actual = _formulaReader.ReadNumerical();

                actual.Should().Be(value * 100);
            }

            [Fact]
            public void Should_Return_Negative_Exp_Value()
            {
                var value = CreateValue<int>();
                _charsToRead.AddRange(new[] { 'e', '-', '2', -1 });

                _textReaderFake
                  .CallsTo(fake => fake.Peek())
                  .ReturnsNextFromSequence(_charsToRead.ToArray());

                var actual = _formulaReader.ReadNumerical();

                actual.Should().Be(value / 100.0);
            }

            [Fact]
            public void Should_Throw_When_Invalid_Exp_Value()
            {
                var value = CreateValue<int>();
                _charsToRead.AddRange(new[] { 'e', -1 });

                _textReaderFake
                  .CallsTo(fake => fake.Peek())
                  .ReturnsNextFromSequence(_charsToRead.ToArray());

                Invoking(() => _formulaReader.ReadNumerical())
                    .Should()
                    .Throw<FormulaException>()
                    .WithMessage($"Invalid numerical value: {value}e");
            }

            [Fact]
            public void Should_Throw_With_Multiple_Consecutive_Decimal_Points()
            {
                var value = CreateValue<int>();
                _charsToRead.AddRange(new[] { (int)'.', '.' });

                _textReaderFake
                  .CallsTo(fake => fake.Peek())
                  .ReturnsNextFromSequence(_charsToRead.ToArray());

                Invoking(() => _formulaReader.ReadNumerical())
                    .Should()
                    .Throw<FormulaException>()
                    .WithMessage($"Invalid numerical value: {value}..");
            }

            [Fact]
            public void Should_Throw_With_Multiple_Decimal_Points()
            {
                var value = CreateValue<int>();
                _charsToRead.Insert(0, (int)'.');
                _charsToRead.AddRange(new[] { (int)'.' });

                _textReaderFake
                  .CallsTo(fake => fake.Peek())
                  .ReturnsNextFromSequence(_charsToRead.ToArray());

                Invoking(() => _formulaReader.ReadNumerical())
                    .Should()
                    .Throw<FormulaException>()
                    .WithMessage($"Invalid numerical value: .{value}.");
            }
        }

        public class ReadNamedOperand : FormulaReaderFixture
        {
            [Fact]
            public void Should_Throw_When_Operation_Factory_Null()
            {
                Invoking(() => _formulaReader.ReadNamedOperand(null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("operationFactory");
            }

            [Fact]
            public void Should_Throw_When_Nothing_To_Read()
            {
                _textReaderFake
                  .CallsTo(fake => fake.Peek())
                  .Returns(-1);

                Invoking(() => _formulaReader.ReadNamedOperand(_arithmeticOperationFactoryFake.FakedObject))
                    .Should()
                    .Throw<FormulaException>()
                    .WithMessage("Nothing to read");
            }

            [Fact]
            public void Should_Throw_When_Starts_With_Non_Digit()
            {
                Invoking(() =>
                    {
                        _textReaderFake
                            .CallsTo(fake => fake.Peek())
                            .ReturnsNextFromSequence(new[] { '(', -1 });

                        _formulaReader.ReadNamedOperand(_arithmeticOperationFactoryFake.FakedObject);
                    })
                    .Should()
                    .Throw<FormulaException>()
                    .WithMessage("Unexpected empty named operand");
            }

            [Fact]
            public void Should_Return_Method_Name()
            {
                _textReaderFake
                  .CallsTo(fake => fake.Peek())
                  .ReturnsNextFromSequence(new[] { (int)'a', '(' });

                var actual = _formulaReader.ReadNamedOperand(_arithmeticOperationFactoryFake.FakedObject);

                actual.Should().Be("a");
            }

            [Fact]
            public void Should_Return_Argument_Name()
            {
                _textReaderFake
                  .CallsTo(fake => fake.Peek())
                  .ReturnsNextFromSequence(new[] { (int)'a', ',' });

                var actual = _formulaReader.ReadNamedOperand(_arithmeticOperationFactoryFake.FakedObject);

                actual.Should().Be("a");
            }

            [Fact]
            public void Should_Return_Last_Argument_Name()
            {
                _textReaderFake
                  .CallsTo(fake => fake.Peek())
                  .ReturnsNextFromSequence(new[] { (int)'a', ')' });

                var actual = _formulaReader.ReadNamedOperand(_arithmeticOperationFactoryFake.FakedObject);

                actual.Should().Be("a");
            }

            [Fact]
            public void Should_Return_Variable_Name()
            {
                _arithmeticOperationFactoryFake
                  .CallsTo(fake => fake.IsCandidate('+'))
                  .Returns(true);

                _textReaderFake
                  .CallsTo(fake => fake.Peek())
                  .ReturnsNextFromSequence(new[] { (int)'a', '+', '1' });

                var actual = _formulaReader.ReadNamedOperand(_arithmeticOperationFactoryFake.FakedObject);

                actual.Should().Be("a");
            }

            [Fact]
            public void Should_Return_Last_Variable_Name()
            {
                _textReaderFake
                  .CallsTo(fake => fake.Peek())
                  .ReturnsNextFromSequence(new[] { 'a', -1 });

                var actual = _formulaReader.ReadNamedOperand(_arithmeticOperationFactoryFake.FakedObject);

                actual.Should().Be("a");
            }
        }

        public class ReadOperator : FormulaReaderFixture
        {
            [Fact]
            public void Should_Throw_When_Operation_Factory_Null()
            {
                Invoking(() => _formulaReader.ReadOperator(null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("operationFactory");
            }

            [Fact]
            public void Should_Throw_When_Nothing_To_Read()
            {
                _textReaderFake
                  .CallsTo(fake => fake.Peek())
                  .Returns(-1);

                Invoking(() => _formulaReader.ReadOperator(_arithmeticOperationFactoryFake.FakedObject))
                    .Should()
                    .Throw<FormulaException>()
                    .WithMessage("Nothing to read");
            }

            [Fact]
            public void Should_Throw_When_No_Valid_Operator()
            {
                _textReaderFake
                  .CallsTo(fake => fake.Peek())
                  .Returns(Create<int>());

                _arithmeticOperationFactoryFake
                  .CallsTo(fake => fake.IsCandidate(A<char>.Ignored))
                  .Returns(false);

                Invoking(() => _formulaReader.ReadOperator(_arithmeticOperationFactoryFake.FakedObject))
                    .Should()
                    .Throw<FormulaException>()
                    .WithMessage("Unexpected empty operation");
            }

            [Fact]
            public void Should_Return_Registered_Operator()
            {
                var opChar = CreateExcluding('-', '+');
                var breakoutChar = CreateExcluding('-', '+', opChar);

                _textReaderFake
                  .CallsTo(fake => fake.Peek())
                  .ReturnsNextFromSequence(new[] { (int)opChar, breakoutChar });

                _arithmeticOperationFactoryFake
                  .CallsTo(fake => fake.IsCandidate(opChar))
                  .Returns(true);

                _arithmeticOperationFactoryFake
                  .CallsTo(fake => fake.IsCandidate(breakoutChar))
                  .Returns(false);

                var actual = _formulaReader.ReadOperator(_arithmeticOperationFactoryFake.FakedObject);

                actual.Should().Be(opChar.ToString());
            }

            [Fact]
            public void Should_Detect_Unary_Minus()
            {
                var opChar = Create<char>();    // test is expected to pass even if + or -

                _textReaderFake
                  .CallsTo(fake => fake.Peek())
                  .ReturnsNextFromSequence(new[] { (int)opChar, '-' });

                _arithmeticOperationFactoryFake
                  .CallsTo(fake => fake.IsCandidate('-'))
                  .Returns(true);

                _arithmeticOperationFactoryFake
                  .CallsTo(fake => fake.IsCandidate(opChar))
                  .Returns(true);

                var actual = _formulaReader.ReadOperator(_arithmeticOperationFactoryFake.FakedObject);

                actual.Should().Be(opChar.ToString());
            }

            [Fact]
            public void Should_Detect_Unary_Plus()
            {
                var opChar = Create<char>();    // test is expected to pass even if + or -

                _textReaderFake
                  .CallsTo(fake => fake.Peek())
                  .ReturnsNextFromSequence(new[] { (int)opChar, '+' });

                _arithmeticOperationFactoryFake
                  .CallsTo(fake => fake.IsCandidate('+'))
                  .Returns(true);

                _arithmeticOperationFactoryFake
                  .CallsTo(fake => fake.IsCandidate(opChar))
                  .Returns(true);

                var actual = _formulaReader.ReadOperator(_arithmeticOperationFactoryFake.FakedObject);

                actual.Should().Be(opChar.ToString());
            }
        }

        public class IsNumericalCandidate : FormulaReaderFixture
        {
            [Fact]
            public void Should_Return_True_For_Digit()
            {
                const string numbers = "0123456789";
                var index = Create<int>() % 10;

                var actual = FormulaReader.IsNumericalCandidate(numbers.ElementAt(index));

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Return_True_For_Decimal_Character()
            {
                var decimalSeparator = Convert.ToChar(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);

                var actual = FormulaReader.IsNumericalCandidate(decimalSeparator);

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Return_False_For_Non_Digit_Candidate()
            {
                var decimalSeparator = Convert.ToChar(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);

                var exclusions = "0123456789".Select(c => c).ToList();
                exclusions.AddRange(new[] { decimalSeparator });

                var token = CreateExcluding(exclusions.ToArray());

                var actual = FormulaReader.IsNumericalCandidate(token);

                actual.Should().BeFalse();
            }
        }

        public class Disposal : FormulaReaderFixture
        {
            // Using this dummy class because FluentAssertions complained about not being able to intercept a sealed method when using _textReaderFake. 
            private class ReaderDummy
              : TextReader
            {
                public int _disposeCount { get; set; }

                protected override void Dispose(bool disposing)
                {
                    base.Dispose(disposing);

                    ++_disposeCount;
                }
            }

            [Fact]
            public void Should_Dispose_Once()
            {
                var reader = new ReaderDummy();
                _formulaReader = new FormulaReader(f => reader, _formula);

                _formulaReader.Dispose();
                _formulaReader.Dispose();

                reader._disposeCount.Should().Be(1);
            }
        }

        #region IDisposable Support
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (_formulaReader != null)
                    {
                        _formulaReader.Dispose();
                        _formulaReader = null;
                    }
                }

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
