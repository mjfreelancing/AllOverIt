using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Tests.Helpers;
using AllOverIt.Evaluator.Variables;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using FakeItEasy;
using FluentAssertions;
using System;
using Xunit;

namespace AllOverIt.Evaluator.Tests
{
    public class FormulaParserFixture : FixtureBase
    {
        private readonly Fake<IFormulaReader> _formulaReaderFake;
        private readonly Fake<IVariableRegistry> _variableRegistryFake;
        private readonly Fake<IFormulaProcessor> _formulaProcessorFake;
        private IFormulaParser _formulaParser;

        public FormulaParserFixture()
        {
            _formulaReaderFake = new Fake<IFormulaReader>();
            _variableRegistryFake = new Fake<IVariableRegistry>();
            _formulaProcessorFake = new Fake<IFormulaProcessor>();

            _formulaParser = new FormulaParser(_formulaProcessorFake.FakedObject, f => _formulaReaderFake.FakedObject);
        }

        public class CreateDefault : FormulaParserFixture
        {
            private readonly IArithmeticOperationFactory _arithmeticfactory;
            private readonly IUserDefinedMethodFactory _userMethodFactory;
            private readonly IFormulaProcessor _formulaProcessor;

            public CreateDefault()
            {
                _arithmeticfactory = this.CreateStub<IArithmeticOperationFactory>();
                _userMethodFactory = this.CreateStub<IUserDefinedMethodFactory>();
                _formulaProcessor = this.CreateStub<IFormulaProcessor>();
            }

            [Fact]
            public void Should_Throw_When_ArithmeticOperationFactory_Null()
            {
                Invoking(() => _formulaParser = FormulaParser.Create(null, _userMethodFactory, (a, u) => _formulaProcessor))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("arithmeticFactory");
            }

            [Fact]
            public void Should_Throw_When_UserDefinedMethodFactory_Null()
            {
                Invoking(() => _formulaParser = FormulaParser.Create(_arithmeticfactory, null, (a, u) => _formulaProcessor))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("userMethodFactory");
            }

            [Fact]
            public void Should_Does_Throw_When_ProcessorCreator_Null()
            {
                Invoking(() => _formulaParser = FormulaParser.Create(_arithmeticfactory, _userMethodFactory, null))
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Create_Processor_Using_Arithmentic_Factory()
            {
                IArithmeticOperationFactory actualFactory = null;

                Func<IArithmeticOperationFactory, IUserDefinedMethodFactory, IFormulaProcessor> processorCreator = (a, u) =>
                {
                    actualFactory = a;

                    return _formulaProcessor;
                };

                _formulaParser = FormulaParser.Create(_arithmeticfactory, _userMethodFactory, processorCreator);

                actualFactory.Should().BeSameAs(_arithmeticfactory);
            }

            [Fact]
            public void Should_Create_Processor_Using_UserMethod_Factory()
            {
                IUserDefinedMethodFactory actualFactory = null;

                Func<IArithmeticOperationFactory, IUserDefinedMethodFactory, IFormulaProcessor> processorCreator = (a, u) =>
                {
                    actualFactory = u;

                    return _formulaProcessor;
                };

                _formulaParser = FormulaParser.Create(_arithmeticfactory, _userMethodFactory, processorCreator);

                actualFactory.Should().BeSameAs(_userMethodFactory);
            }

            [Fact]
            public void Should_Create_Processor()
            {
                IFormulaProcessor actualProcessor = null;

                Func<IArithmeticOperationFactory, IUserDefinedMethodFactory, IFormulaProcessor> processorCreator = (a, u) =>
                {
                    a.Should().BeSameAs(_arithmeticfactory);
                    u.Should().BeSameAs(_userMethodFactory);

                    actualProcessor = _formulaProcessor;

                    return _formulaProcessor;
                };

                _formulaParser = FormulaParser.Create(_arithmeticfactory, _userMethodFactory, processorCreator);

                actualProcessor.Should().BeSameAs(_formulaProcessor);
            }

            [Fact]
            public void Should_Create_Default_Processor_When_Null()
            {
                _formulaParser = FormulaParser.Create(new ArithmeticOperationFactory(), new UserDefinedMethodFactory(), null);

                var value1 = Create<double>();
                var value2 = Create<double>();
                var value3 = Create<double>();

                var expected = value1 + value2 + value3;

                var factory = new VariableFactory();
                var registry = factory.CreateVariableRegistry();
                var variable = factory.CreateConstantVariable("a", value3);

                registry.AddVariable(variable);

                var processorResult = _formulaParser.Parse($"{value1}+{value2}+a", registry);

                var expression = processorResult.FormulaExpression;
                var variableNames = processorResult.ReferencedVariableNames;

                variableNames.Should().Contain("a");

                var result = expression.Compile().Invoke();

                result.Should().Be(expected);
            }

            [Fact]
            public void Should_Create_Parser_1()
            {
                _formulaParser = FormulaParser.CreateDefault();

                _formulaParser.Should().BeOfType<FormulaParser>();
            }

            [Fact]
            public void Should_Create_Parser_2()
            {
                _formulaParser = FormulaParser.Create(_arithmeticfactory, _userMethodFactory, (a, u) => _formulaProcessor);

                _formulaParser.Should().BeOfType<FormulaParser>();
            }
        }

        public class Constructor : FormulaParserFixture
        {
            [Fact]
            public void Should_Throw_When_Processpr_Null()
            {
                Invoking(() => _formulaParser = new FormulaParser(null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("formulaProcessor");
            }

            [Fact]
            public void Should_Throw_When_Reader_Factory_Null()
            {
                Invoking(() => _formulaParser = new FormulaParser(this.CreateStub<IFormulaProcessor>(), null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("formulaReaderCreator");
            }
        }

        public class Parse : FormulaParserFixture
        {
            [Fact]
            public void Should_Throw_When_Formula_Null()
            {
                Invoking(() => _formulaParser.Parse(null, _variableRegistryFake.FakedObject))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("formula");
            }

            [Fact]
            public void Should_Throw_When_Formula_Empty()
            {
                Invoking(() => _formulaParser.Parse(string.Empty, _variableRegistryFake.FakedObject))
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("formula");
            }

            [Fact]
            public void Should_Throw_When_Formula_Whitespace()
            {
                Invoking(() => _formulaParser.Parse(" ", _variableRegistryFake.FakedObject))
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("formula");
            }

            [Fact]
            public void Should_Throw_When_Variable_Registry_Null()
            {
                Invoking(() => _formulaParser.Parse(Create<string>(), null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("variableRegistry");
            }

            [Fact]
            public void Should_Remove_Formula_Whitespace()
            {
                var value = Create<double>();
                var formula = $"{value}     ";
                var expected = $"{value}";
                string actual = null;

                Func<string, IFormulaReader> reader = f =>
                {
                    actual = f;
                    return _formulaReaderFake.FakedObject;
                };

                _formulaParser = new FormulaParser(
                  _formulaProcessorFake.FakedObject,
                  reader);

                _formulaParser.Parse(formula, _variableRegistryFake.FakedObject);

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Not_Mutate_Input_Formula()
            {
                var value1 = Create<int>();
                var value2 = Create<int>();
                var formula = $"{value1}  +  {value2}";

                _formulaParser.Parse(formula, _variableRegistryFake.FakedObject);

                formula.Should().Be($"{value1}  +  {value2}");
            }

            [Fact]
            public void Should_Invoke_Formula_Reader_Factory()
            {
                var created = false;

                Func<string, IFormulaReader> readerFactory = f =>
                {
                    created = true;
                    return _formulaReaderFake.FakedObject;
                };

                _formulaParser = new FormulaParser(_formulaProcessorFake.FakedObject, readerFactory);

                _formulaParser.Parse(Create<string>(), _variableRegistryFake.FakedObject);

                created.Should().BeTrue();

                _formulaProcessorFake
                  .CallsTo(fake => fake.Process(_formulaReaderFake.FakedObject, _variableRegistryFake.FakedObject))
                  .MustHaveHappened(1, Times.Exactly);
            }

            [Fact]
            public void Should_Call_FormulaProcessor_Process()
            {
                _formulaParser.Parse(Create<string>(), _variableRegistryFake.FakedObject);

                _formulaProcessorFake
                  .CallsTo(fake => fake.Process(_formulaReaderFake.FakedObject, _variableRegistryFake.FakedObject))
                  .MustHaveHappened(1, Times.Exactly);
            }

            [Fact]
            public void Should_Return_ProcessorResult()
            {
                var expected = EvaluatorHelpers.CreateFormulaProcessorResult(Create<double>(), CreateMany<string>());

                _formulaProcessorFake
                  .CallsTo(fake => fake.Process(_formulaReaderFake.FakedObject, _variableRegistryFake.FakedObject))
                  .Returns(expected);

                var actual = _formulaParser.Parse(Create<string>(), _variableRegistryFake.FakedObject);

                actual.Should().Be(expected);
            }
        }
    }
}
