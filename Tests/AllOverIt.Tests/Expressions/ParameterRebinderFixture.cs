using AllOverIt.Expressions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AllOverIt.Tests.Expressions
{
    public class ParameterRebinderFixture : FixtureBase
    {
        public class ReplaceParameters : ParameterRebinderFixture
        {
            [Fact]
            public void Should_Throw_When_Mapping_Null()
            {
                Invoking(
                    () => ParameterRebinder.ReplaceParameters(null, this.CreateStub<Expression>()))
                  .Should()
                  .Throw<ArgumentNullException>()
                  .WithNamedMessageWhenNull("parameterMap");
            }

            [Fact]
            public void Should_Throw_When_Expression_Null()
            {
                Invoking(
                    () => ParameterRebinder.ReplaceParameters(new Dictionary<ParameterExpression, ParameterExpression>(), null))
                  .Should()
                  .Throw<ArgumentNullException>()
                  .WithNamedMessageWhenNull("expression");
            }

            [Fact]
            public void Should_Replace_Second_Parameter_With_First()
            {
                Expression<Func<int, bool>> expression1 = val1 => val1 < 50;
                Expression<Func<int, bool>> expression2 = val2 => val2 > 100;

                var map = expression1.Parameters
                  .Select((first, index)
                    => new
                    {
                        First = first,
                        Second = expression2.Parameters[index]
                    }
                  ).ToDictionary(parameter => parameter.Second, parameter => parameter.First);

                // replace parameters in the lambda expression2 with parameters from expression1
                var secondBody = ParameterRebinder.ReplaceParameters(map, expression2.Body);

                var originalBody1 = expression1.Body.ToString();
                var originalBody2 = expression2.Body.ToString();

                originalBody1.Should().Be("(val1 < 50)");
                originalBody2.Should().Be("(val2 > 100)");

                var actualBody = secondBody.ToString();

                actualBody.Should().Be("(val1 > 100)");
            }
        }
    }
}