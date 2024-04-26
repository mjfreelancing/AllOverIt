using AllOverIt.Expressions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System.Linq.Expressions;

namespace AllOverIt.Tests.Expressions
{
    public class PredicateBuilderFixture : FixtureBase
    {
        private class DummySubject
        {
            public int Number { get; set; }
            public string String { get; set; }
        }

        public class True : PredicateBuilderFixture
        {
            [Fact]
            public void Should_Evaluate_To_True()
            {
                var func = PredicateBuilder
                  .True<DummySubject>()
                  .Compile();

                var subject = Create<DummySubject>();

                var actual = func.Invoke(subject);

                actual.Should().BeTrue();
            }
        }

        public class False : PredicateBuilderFixture
        {
            [Fact]
            public void Should_Evaluate_To_False()
            {
                var func = PredicateBuilder
                  .False<DummySubject>()
                  .Compile();

                var subject = Create<DummySubject>();

                var actual = func.Invoke(subject);

                actual.Should().BeFalse();
            }
        }

        public class Where : PredicateBuilderFixture
        {
            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Set_Initial_State(bool expected)
            {
                var func = PredicateBuilder
                  .Where<DummySubject>(expected)
                  .Compile();

                var subject = Create<DummySubject>();

                var actual = func.Invoke(subject);

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Return_SameExpression()
            {
                Expression<Func<DummySubject, bool>> expression = model => Create<bool>();

                var actual = PredicateBuilder.Where<DummySubject>(expression);

                actual.Should().Be(expression);
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Evaluate_To_Expected_Value(bool expected)
            {
                var func = PredicateBuilder
                  .Where<DummySubject>(model => expected)
                  .Compile();

                var subject = Create<DummySubject>();

                var actual = func.Invoke(subject);

                actual.Should().Be(expected);
            }
        }

        public class Or : PredicateBuilderFixture
        {
            [Fact]
            public void Should_Throw_When_Left_Expression_Null()
            {
                Invoking(
                    () => PredicateBuilder.Or<DummySubject>(null, model => Create<bool>()))
                  .Should()
                  .Throw<ArgumentNullException>()
                  .WithNamedMessageWhenNull("leftExpression");
            }

            [Fact]
            public void Should_Throw_When_Right_Expression_Null()
            {
                Invoking(
                    () => PredicateBuilder.Or<DummySubject>(model => Create<bool>(), null))
                  .Should()
                  .Throw<ArgumentNullException>()
                  .WithNamedMessageWhenNull("rightExpression");
            }

            [Theory]
            [InlineData(false, false)]
            [InlineData(false, true)]
            [InlineData(true, false)]
            [InlineData(true, true)]
            public void Should_Evaluate_To_Expected_Value(bool lhs, bool rhs)
            {
                var expected = lhs || rhs;

                var func = PredicateBuilder
                  .Or<DummySubject>(model => lhs, model => rhs)
                  .Compile();

                var subject = Create<DummySubject>();

                var actual = func.Invoke(subject);

                actual.Should().Be(expected);
            }

            [Theory]
            [InlineData(0, "a")]
            [InlineData(0, "n")]
            [InlineData(0, "z")]
            [InlineData(100, "a")]
            [InlineData(100, "n")]
            [InlineData(100, "z")]
            public void Should_Compose_Expressions(int input1, string input2)
            {
                var subject = Create<DummySubject>();
                subject.Number = input1;
                subject.String = input2;

                // deliberately using 'sub1' and 'sub2' so the ParameterExpression's have different names
                Expression<Func<DummySubject, bool>> numberPredicate = sub1 => sub1.Number < 50;
                Expression<Func<DummySubject, bool>> stringPredicate = sub2 => sub2.String[0] <= 'n';

                var predicate = PredicateBuilder
                  .Where(numberPredicate)
                  .Or(stringPredicate)
                  .Compile();

                var expected = input1 < 50 || input2[0] <= 'n';

                var actual = predicate.Invoke(subject);

                actual.Should().Be(expected);
            }
        }

        public class And : PredicateBuilderFixture
        {
            [Fact]
            public void Should_Throw_When_Left_Expression_Null()
            {
                Invoking(
                    () => PredicateBuilder.And<DummySubject>(null, model => Create<bool>()))
                  .Should()
                  .Throw<ArgumentNullException>()
                  .WithNamedMessageWhenNull("leftExpression");
            }

            [Fact]
            public void Should_Throw_When_Right_Expression_Null()
            {
                Invoking(
                    () => PredicateBuilder.And<DummySubject>(model => Create<bool>(), null))
                  .Should()
                  .Throw<ArgumentNullException>()
                  .WithNamedMessageWhenNull("rightExpression");
            }

            [Theory]
            [InlineData(false, false)]
            [InlineData(false, true)]
            [InlineData(true, false)]
            [InlineData(true, true)]
            public void Should_Evaluate_To_Expected_Value(bool lhs, bool rhs)
            {
                var expected = lhs && rhs;

                var func = PredicateBuilder
                  .And<DummySubject>(model => lhs, model => rhs)
                  .Compile();

                var subject = Create<DummySubject>();

                var actual = func.Invoke(subject);

                actual.Should().Be(expected);
            }

            [Theory]
            [InlineData(0, "a")]
            [InlineData(0, "n")]
            [InlineData(0, "z")]
            [InlineData(100, "a")]
            [InlineData(100, "n")]
            [InlineData(100, "z")]
            public void Should_Compose_Expressions(int input1, string input2)
            {
                var subject = Create<DummySubject>();
                subject.Number = input1;
                subject.String = input2;

                // deliberately using 'sub1' and 'sub2' so the ParameterExpression's have different names
                Expression<Func<DummySubject, bool>> numberPredicate = sub1 => sub1.Number < 50;
                Expression<Func<DummySubject, bool>> stringPredicate = sub2 => sub2.String[0] <= 'n';

                var predicate = PredicateBuilder
                  .Where(numberPredicate)
                  .And(stringPredicate)
                  .Compile();

                var expected = input1 < 50 && input2[0] <= 'n';

                var actual = predicate.Invoke(subject);

                actual.Should().Be(expected);
            }
        }

        public class Not : PredicateBuilderFixture
        {
            [Fact]
            public void Should_Throw_When_Expression_Null()
            {
                Invoking(() => PredicateBuilder.Not<DummySubject>(null))
                  .Should()
                  .Throw<ArgumentNullException>()
                  .WithNamedMessageWhenNull("expression");
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Evaluate_To_Expected_Value(bool value)
            {
                var expected = !value;

                var func = PredicateBuilder
                  .Not<DummySubject>(model => value)
                  .Compile();

                var subject = Create<DummySubject>();

                var actual = func.Invoke(subject);

                actual.Should().Be(expected);
            }
        }
    }
}
