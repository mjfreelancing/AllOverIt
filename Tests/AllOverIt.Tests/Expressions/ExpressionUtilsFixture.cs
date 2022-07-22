using AllOverIt.Expressions;
using AllOverIt.Extensions;
using AllOverIt.Fixture;
using FluentAssertions;
using System;
using Xunit;

namespace AllOverIt.Tests.Expressions
{
    public class ExpressionUtilsFixture : FixtureBase
    {
        public class CreateParameterizedValue_Typed : ExpressionUtilsFixture
        {
            [Fact]
            public void Should_Return_Expression_For_Value()
            {
                var expected = Create<int>();

                var actual = ExpressionUtils.CreateParameterizedValue<int>(expected);

                actual.GetValue()
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void Should_Return_Expression_For_Nullable_Value()
            {
                var actual = ExpressionUtils.CreateParameterizedValue<int?>(default);

                actual.GetValue()
                    .Should()
                    .BeNull();
            }

            [Fact]
            public void Should_Return_Expression_For_String()
            {
                var expected = Create<string>();

                var actual = ExpressionUtils.CreateParameterizedValue<string>(expected);

                actual.GetValue()
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void Should_Return_Expression_For_Null_String()
            {
                var actual = ExpressionUtils.CreateParameterizedValue<string>(default);

                actual.GetValue()
                    .Should()
                    .BeNull();
            }
        }

        public class CreateParameterizedValue_Object : ExpressionUtilsFixture
        {
            [Fact]
            public void Should_Throw_When_Type_Not_Provided()
            {
                Invoking(() =>
                {
                    _ = ExpressionUtils.CreateParameterizedValue((string) default, null);
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithMessage("The value type must be provided when creating a parameterized value expression.");
            }

            [Fact]
            public void Should_Determine_Value_Type()
            {
                var expected = Create<int>();

                var actual = ExpressionUtils.CreateParameterizedValue(expected, null);

                actual.GetValue()
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void Should_Determine_String_Type()
            {
                object expected = Create<string>();

                var actual = ExpressionUtils.CreateParameterizedValue(expected, null);

                actual.GetValue()
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void Should_Return_Expression_For_Value()
            {
                var expected = Create<int>();

                var actual = ExpressionUtils.CreateParameterizedValue(expected, typeof(int));

                actual.GetValue()
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void Should_Return_Expression_For_Nullable_Value()
            {
                var actual = ExpressionUtils.CreateParameterizedValue(null, typeof(int?));

                actual.GetValue()
                    .Should()
                    .BeNull();
            }

            [Fact]
            public void Should_Return_Expression_For_String()
            {
                var expected = Create<string>();

                var actual = ExpressionUtils.CreateParameterizedValue(expected, typeof(string));

                actual.GetValue()
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void Should_Return_Expression_For_Null_String()
            {
                var actual = ExpressionUtils.CreateParameterizedValue(null, typeof(string));

                actual.GetValue()
                    .Should()
                    .BeNull();
            }
        }
    }
}