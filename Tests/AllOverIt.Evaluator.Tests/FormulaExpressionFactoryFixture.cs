using AllOverIt.Evaluator.Exceptions;
using AllOverIt.Evaluator.Operations;
using AllOverIt.Evaluator.Variables;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using FakeItEasy;
using Shouldly;
using System.Linq.Expressions;

namespace AllOverIt.Evaluator.Tests
{
    public class FormulaExpressionFactoryFixture : FixtureBase
    {
        private readonly int _argumentCount;
        private readonly Fake<IArithmeticOperation> _operationFake;
        private readonly Expression[] _expectedExpressions;
        private readonly Stack<Expression> _stack;

        public FormulaExpressionFactoryFixture()
        {
            _argumentCount = Create<int>() % 20 + 1;
            _operationFake = new Fake<IArithmeticOperation>();

            _operationFake
              .CallsTo(fake => fake.ArgumentCount)
              .Returns(_argumentCount);

            var expressions = new Stack<Expression>();

            for (var i = 0; i < _argumentCount; ++i)
            {
                expressions.Push(this.CreateStub<Expression>());
            }

            _expectedExpressions = expressions.Reverse().ToArray();

            _stack = new Stack<Expression>(_expectedExpressions);
        }

        public class CreateExpression_OperationBase : FormulaExpressionFactoryFixture
        {
            [Fact]
            public void Should_Throw_When_Operation_Null()
            {
                var exception = Should.Throw<ArgumentNullException>(() => FormulaExpressionFactory.CreateExpression(null, _stack));

                exception.WithNamedMessageWhenNull("operation");
            }

            [Fact]
            public void Should_Throw_When_Expression_Null()
            {
                var exception = Should.Throw<ArgumentNullException>(() =>
                    FormulaExpressionFactory.CreateExpression(this.CreateStub<ArithmeticOperationBase>(), null));

                exception.WithNamedMessageWhenNull("expressionStack");
            }

            [Fact]
            public void Should_Throw_When_Insufficient_Parameters()
            {
                _operationFake
                    .CallsTo(fake => fake.ArgumentCount)
                    .Returns(_argumentCount + 1);

                var exception = Should.Throw<FormulaException>(() => FormulaExpressionFactory.CreateExpression(_operationFake.FakedObject, _stack));

                exception.Message.ShouldBe(
                    $"Insufficient expressions in the stack. {_argumentCount} available, {_argumentCount + 1} required.");
            }

            [Fact]
            public void Should_Call_Operation_GetExpression()
            {
                Expression[] actualExpressions = null;

                _operationFake
                    .CallsTo(fake => fake.GetExpression(A<Expression[]>.Ignored))
                    .Invokes(call => actualExpressions = call.Arguments.Get<Expression[]>(0));

                FormulaExpressionFactory.CreateExpression(_operationFake.FakedObject, _stack);

                actualExpressions.ShouldBe(_expectedExpressions);
            }

            [Fact]
            public void Should_Return_Expected_Expression()
            {
                var expected = this.CreateStub<Expression>();

                _operationFake
                    .CallsTo(fake => fake.GetExpression(A<Expression[]>.Ignored))
                    .Returns(expected);

                var actual = FormulaExpressionFactory.CreateExpression(_operationFake.FakedObject, _stack);

                actual.ShouldBeSameAs(expected);
            }
        }

        public class CreateExpression_Variable : FormulaExpressionFactoryFixture
        {
            [Fact]
            public void Should_Throw_When_Variable_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue => FormulaExpressionFactory.CreateExpression(stringValue, this.CreateStub<IVariableRegistry>()),
                    "variableName");
            }

            [Fact]
            public void Should_Throw_When_Expression_Null()
            {
                var exception = Should.Throw<ArgumentNullException>(() => FormulaExpressionFactory.CreateExpression(Create<string>(), null));

                exception.WithNamedMessageWhenNull("variableRegistry");
            }

            [Fact]
            public void Should_Return_Variable_Expression()
            {
                var variableName = Create<string>();
                var variableRegistry = new VariableRegistry();

                var expected = CreateVariableExpression(variableName, variableRegistry).ToString();
                var actual = FormulaExpressionFactory.CreateExpression(variableName, variableRegistry).ToString();

                actual.ShouldBe(expected);
            }

            private static Expression CreateVariableExpression(string variableName, IVariableRegistry variableRegistry)
            {
                var registry = Expression.Constant(variableRegistry);

                var getValueMethod = typeof(IReadableVariableRegistry).GetMethod("GetValue", new[] { typeof(string) });
                var variableExpression = new Expression[] { Expression.Constant(variableName) };

                return Expression.Call(registry, getValueMethod, variableExpression);
            }
        }
    }
}
