using AllOverIt.Exceptions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Patterns.Enumeration;
using FluentAssertions;
using System;
using System.Runtime.CompilerServices;
using Xunit;

namespace AllOverIt.Tests.Patterns.Enumeration
{
    public class EnrichedEnumFixture : FixtureBase
    {
        private class EnrichedEnumDummy : EnrichedEnum<EnrichedEnumDummy>
        {
            public static readonly EnrichedEnumDummy Value1 = new(1);

            // ReSharper disable once ExplicitCallerInfoArgument
            public static readonly EnrichedEnumDummy Value2 = new(2, "Value 2");

            private EnrichedEnumDummy(int value, [CallerMemberName] string name = null)
                : base(value, name)
            {
            }
        }

        private class EnrichedEnumDummy2 : EnrichedEnum<EnrichedEnumDummy2>
        {
            public static readonly EnrichedEnumDummy2 Value1 = new(1);

            // ReSharper disable once ExplicitCallerInfoArgument
            public static readonly EnrichedEnumDummy2 Value2 = new(2, "Value 2");

            private EnrichedEnumDummy2(int value, [CallerMemberName] string name = null)
                : base(value, name)
            {
            }
        }

        public class Constructor : EnrichedEnumFixture
        {
            [Fact]
            public void Should_Set_Value()
            {
                EnrichedEnumDummy.Value1.Value.Should().Be(1);
                EnrichedEnumDummy.Value2.Value.Should().Be(2);
            }

            [Fact]
            public void Should_Set_Name_Same_As_Field()
            {
                EnrichedEnumDummy.Value1.Name.Should().Be(nameof(EnrichedEnumDummy.Value1));
            }

            [Fact]
            public void Should_Set_Name_As_Specified()
            {
                EnrichedEnumDummy.Value2.Name.Should().Be("Value 2");
            }
        }

        public class ToStringMethod : EnrichedEnumFixture
        {
            [Fact]
            public void Should_Return_Name()
            {
                EnrichedEnumDummy.Value1.ToString().Should().Be(EnrichedEnumDummy.Value1.Name);
                EnrichedEnumDummy.Value2.ToString().Should().Be(EnrichedEnumDummy.Value2.Name);
            }
        }

        public class CompareTo : EnrichedEnumFixture
        {
            [Fact]
            public void Should_Not_Compare()
            {
                var actual = EnrichedEnumDummy.Value1.CompareTo(EnrichedEnumDummy.Value2);
                actual.Should().NotBe(0);
            }

            [Fact]
            public void Should_Compare()
            {
                var actual = EnrichedEnumDummy.Value2.CompareTo(EnrichedEnumDummy.Value2);
                actual.Should().Be(0);
            }

            [Fact]
            public void Should_Throw_When_Null()
            {
                Invoking(() =>
                    {
                        // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                        EnrichedEnumDummy.Value2.CompareTo(null);
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("other");
            }
        }

        public class Equals_Object : EnrichedEnumFixture
        {
            [Fact]
            public void Should_Be_Equal()
            {
                object enum1 = EnrichedEnumDummy.Value1;
                var actual = EnrichedEnumDummy.Value1.Equals(enum1);

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Not_Be_Equal_When_Null()
            {
                object enumNull = null;
                var actual = EnrichedEnumDummy.Value1.Equals(enumNull);

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Not_Be_Equal_Same_Type()
            {
                object enum2 = EnrichedEnumDummy.Value2;
                var actual = EnrichedEnumDummy.Value1.Equals(enum2);

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Not_Be_Equal_Different_Type_Same_Value()
            {
                object enum1 = EnrichedEnumDummy2.Value1;
                var actual = EnrichedEnumDummy.Value1.Equals(enum1);

                actual.Should().BeFalse();
            }
        }

        public class Equals_Typed : EnrichedEnumFixture
        {
            [Fact]
            public void Should_Be_Equal()
            {
                var enum1 = EnrichedEnumDummy.Value1;
                var actual = EnrichedEnumDummy.Value1.Equals(enum1);

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Not_Be_Equal_When_Null()
            {
                EnrichedEnumDummy enumNull = null;
                var actual = EnrichedEnumDummy.Value1.Equals(enumNull);

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Not_Be_Equal_Same_Type()
            {
                var enum2 = EnrichedEnumDummy.Value2;
                var actual = EnrichedEnumDummy.Value1.Equals(enum2);

                actual.Should().BeFalse();
            }
        }

        public class GetHashCodeMethod : EnrichedEnumFixture
        {
            [Fact]
            public void Should_Return_Value_HashCode()
            {
                var expected = 2.GetHashCode();
                var actual = EnrichedEnumDummy.Value2.GetHashCode();

                actual.Should().Be(expected);
            }
        }

        public class GetAllValues : EnrichedEnumFixture
        {
            [Fact]
            public void Should_Get_All_Values()
            {
                var actual = EnrichedEnumDummy.GetAllValues();

                var expected = new[] {EnrichedEnumDummy.Value1.Value, EnrichedEnumDummy.Value2.Value};

                expected.Should().BeEquivalentTo(actual);
            }
        }

        public class GetAllNames : EnrichedEnumFixture
        {
            [Fact]
            public void Should_Get_All_Names()
            {
                var actual = EnrichedEnumDummy.GetAllNames();

                var expected = new[] {EnrichedEnumDummy.Value1.Name, EnrichedEnumDummy.Value2.Name};

                expected.Should().BeEquivalentTo(actual);
            }
        }

        public class GetAll : EnrichedEnumFixture
        {
            [Fact]
            public void Should_Get_All()
            {
                var actual = EnrichedEnumDummy.GetAll();

                var expected = new[] {EnrichedEnumDummy.Value1, EnrichedEnumDummy.Value2};

                expected.Should().BeEquivalentTo(actual);
            }
        }

        public class From_Int_Value : EnrichedEnumFixture
        {
            [Fact]
            public void Should_Get_From_Number()
            {
                EnrichedEnumDummy.From(1).Should().Be(EnrichedEnumDummy.Value1);
                EnrichedEnumDummy.From(2).Should().Be(EnrichedEnumDummy.Value2);
            }

            [Fact]
            public void Should_Not_Get_From_Invalid_Value()
            {
                Invoking(() =>
                    {
                        _ = EnrichedEnumDummy.From(0);
                    })
                    .Should()
                    .Throw<EnrichedEnumException>()
                    .WithMessage("Unable to convert '0' to a EnrichedEnumDummy.");
            }
        }

        public class From_String_Value : EnrichedEnumFixture
        {
            [Fact]
            public void Should_Get_From_Number_String()
            {
                EnrichedEnumDummy.From("1").Should().Be(EnrichedEnumDummy.Value1);
                EnrichedEnumDummy.From("2").Should().Be(EnrichedEnumDummy.Value2);
            }

            [Fact]
            public void Should_Get_From_Name()
            {
                EnrichedEnumDummy.From(nameof(EnrichedEnumDummy.Value1)).Should().Be(EnrichedEnumDummy.Value1);
                EnrichedEnumDummy.From("Value 2").Should().Be(EnrichedEnumDummy.Value2);
                EnrichedEnumDummy.From("VALUE 2").Should().Be(EnrichedEnumDummy.Value2);
            }

            [Fact]
            public void Should_Not_Get_From_Invalid_Value()
            {
                var value = Create<string>();

                Invoking(() =>
                    {
                        _ = EnrichedEnumDummy.From(value);
                    })
                    .Should()
                    .Throw<EnrichedEnumException>()
                    .WithMessage($"Unable to convert '{value}' to a EnrichedEnumDummy.");
            }
        }

        public class TryFromValue : EnrichedEnumFixture
        {
            [Fact]
            public void Should_Get_From_Value()
            {
                var tryResult = EnrichedEnumDummy.TryFromValue(1, out var actual);

                tryResult.Should().BeTrue();
                actual.Should().Be(EnrichedEnumDummy.Value1);
            }

            [Fact]
            public void Should_Not_Get_From_Value()
            {
                var tryResult = EnrichedEnumDummy.TryFromValue(-1, out var actual);

                tryResult.Should().BeFalse();
                actual.Should().BeNull();
            }
        }

        public class TryFromName : EnrichedEnumFixture
        {
            [Fact]
            public void Should_Get_From_Name()
            {
                var tryResult = EnrichedEnumDummy.TryFromName("VALUE 2", out var actual);

                tryResult.Should().BeTrue();
                actual.Should().Be(EnrichedEnumDummy.Value2);
            }

            [Fact]
            public void Should_Not_Get_From_Name()
            {
                var tryResult = EnrichedEnumDummy.TryFromName(Create<string>(), out var actual);

                tryResult.Should().BeFalse();
                actual.Should().BeNull();
            }
        }

        public class TryFromNameOrValue : EnrichedEnumFixture
        {
            [Fact]
            public void Should_Convert_From_Value()
            {
                EnrichedEnumDummy.TryFromNameOrValue("2", out var enumeration).Should().BeTrue();

                enumeration.Should().Be(EnrichedEnumDummy.Value2);
            }

            [Fact]
            public void Should_Convert_From_Name()
            {
                EnrichedEnumDummy.TryFromNameOrValue("Value 2", out var enumeration).Should().BeTrue();

                enumeration.Should().Be(EnrichedEnumDummy.Value2);
            }

            [Fact]
            public void Should_Convert_From_Name_Case_Insensitive()
            {
                EnrichedEnumDummy.TryFromNameOrValue("VaLuE 2", out var enumeration).Should().BeTrue();

                enumeration.Should().Be(EnrichedEnumDummy.Value2);
            }

            [Fact]
            public void Should_Not_Convert_From_Value()
            {
                EnrichedEnumDummy.TryFromNameOrValue("3", out _).Should().BeFalse();
            }

            [Fact]
            public void Should_Not_Convert_From_Name()
            {
                EnrichedEnumDummy.TryFromNameOrValue("Value 1", out _).Should().BeFalse();
            }
        }

        public class HasValue : EnrichedEnumFixture
        {
            [Theory]
            [InlineData(1)]
            [InlineData(2)]
            public void Should_Find_Value(int value)
            {
                var actual = EnrichedEnumDummy.HasValue(value);

                actual.Should().BeTrue();
            }

            [Theory]
            [InlineData(-1)]
            [InlineData(0)]
            public void Should_Not_Find_Value(int value)
            {
                var actual = EnrichedEnumDummy.HasValue(value);

                actual.Should().BeFalse();
            }
        }

        public class HasName : EnrichedEnumFixture
        {
            [Theory]
            [InlineData("Value1")]
            [InlineData("VALUE1")]
            [InlineData("Value 2")]
            [InlineData("VALUE 2")]
            public void Should_Find_Name(string name)
            {
                var actual = EnrichedEnumDummy.HasName(name);

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Not_Find_Name()
            {
                var actual = EnrichedEnumDummy.HasName(Create<string>());

                actual.Should().BeFalse();
            }
        }

        public class HasNameOrValue : EnrichedEnumFixture
        {
            [Theory]
            [InlineData("1")]
            [InlineData("Value1")]
            [InlineData("VALUE1")]
            [InlineData("2")]
            [InlineData("Value 2")]
            [InlineData("VALUE 2")]
            public void Should_Find_Name_Or_Value(string name)
            {
                var actual = EnrichedEnumDummy.HasNameOrValue(name);

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Not_Find_Name_Or_Value()
            {
                EnrichedEnumDummy.HasNameOrValue("-1").Should().BeFalse();
                EnrichedEnumDummy.HasNameOrValue(Create<string>()).Should().BeFalse();
            }
        }

        public class Operator_Equals : EnrichedEnumFixture
        {
            [Fact]
            public void Should_Be_Equal()
            {
                var enum1 = EnrichedEnumDummy.Value1;
                var actual = EnrichedEnumDummy.Value1 == enum1;

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Not_Be_Equal_When_Null()
            {
                EnrichedEnumDummy enumNull = null;
                var actual = EnrichedEnumDummy.Value1 == enumNull;

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Not_Be_Equal_Same_Type()
            {
                var actual = EnrichedEnumDummy.Value1 == EnrichedEnumDummy.Value2;

                actual.Should().BeFalse();
            }
        }

        public class Operator_NotEquals : EnrichedEnumFixture
        {
            [Fact]
            public void Should_Be_Equal()
            {
                var enum1 = EnrichedEnumDummy.Value1;
                var actual = EnrichedEnumDummy.Value1 != enum1;

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Not_Be_Equal_When_Null()
            {
                EnrichedEnumDummy enumNull = null;

                // ReSharper disable once ExpressionIsAlwaysNull
                var actual = EnrichedEnumDummy.Value1 != enumNull;

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Not_Be_Equal_Same_Type()
            {
                var actual = EnrichedEnumDummy.Value1 != EnrichedEnumDummy.Value2;

                actual.Should().BeTrue();
            }
        }

        public class Operator_GreaterThan : EnrichedEnumFixture
        {
            [Fact]
            public void Should_Be_GreaterThan()
            {
                var actual = EnrichedEnumDummy.Value2 > EnrichedEnumDummy.Value1;

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Not_Be_GreaterThan()
            {
                var actual = EnrichedEnumDummy.Value1 > EnrichedEnumDummy.Value2;

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Not_Be_GreaterThan_When_Equal()
            {
#pragma warning disable CS1718 // Comparison made to same variable
                var actual = EnrichedEnumDummy.Value1 > EnrichedEnumDummy.Value1;
#pragma warning restore CS1718 // Comparison made to same variable

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Throw_When_Null()
            {
                Invoking(() =>
                    {
                        _ = EnrichedEnumDummy.Value2 > null;
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("other");
            }
        }

        public class Operator_GreaterThanOrEqual : EnrichedEnumFixture
        {
            [Fact]
            public void Should_Be_GreaterThan()
            {
                var actual = EnrichedEnumDummy.Value2 >= EnrichedEnumDummy.Value1;

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Be_Equal()
            {
                var enum2 = EnrichedEnumDummy.Value2;
                var actual = EnrichedEnumDummy.Value2 >= enum2;

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Not_Be_GreaterThanOrEqual()
            {
                var actual = EnrichedEnumDummy.Value1 >= EnrichedEnumDummy.Value2;

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Throw_When_Null()
            {
                Invoking(() =>
                    {
                        _ = EnrichedEnumDummy.Value2 >= null;
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("other");
            }
        }

        public class Operator_LessThan : EnrichedEnumFixture
        {
            [Fact]
            public void Should_Be_LessThan()
            {
                var actual = EnrichedEnumDummy.Value1 < EnrichedEnumDummy.Value2;

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Not_Be_LessThan()
            {
                var actual = EnrichedEnumDummy.Value2 < EnrichedEnumDummy.Value1;

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Not_Be_LessThan_When_Equal()
            {
#pragma warning disable CS1718 // Comparison made to same variable
                var actual = EnrichedEnumDummy.Value2 < EnrichedEnumDummy.Value2;
#pragma warning restore CS1718 // Comparison made to same variable

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Throw_When_Null()
            {
                Invoking(() =>
                {
                    _ = EnrichedEnumDummy.Value2 < null;
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("other");
            }
        }

        public class Operator_LessThanOrEqual : EnrichedEnumFixture
        {
            [Fact]
            public void Should_Be_LessThan()
            {
                var actual = EnrichedEnumDummy.Value1 <= EnrichedEnumDummy.Value2;

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Be_Equal()
            {
                var enum2 = EnrichedEnumDummy.Value2;
                var actual = EnrichedEnumDummy.Value2 <= enum2;

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Not_Be_LessThanOrEqual()
            {
                var actual = EnrichedEnumDummy.Value2 <= EnrichedEnumDummy.Value1;

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Throw_When_Null()
            {
                Invoking(() =>
                {
                    _ = EnrichedEnumDummy.Value2 <= null;
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("other");
            }
        }

        public class Implicit_Operator : EnrichedEnumFixture
        {
            [Fact]
            public void Should_Convert()
            {
                int value = EnrichedEnumDummy.Value2;

                value.Should().Be(2);
            }
        }

        public class Explicit_Operator : EnrichedEnumFixture
        {
            [Fact]
            public void Should_Convert()
            {
                var value = (EnrichedEnumDummy) 2;

                value.Should().Be(EnrichedEnumDummy.Value2);
            }
        }
    }
}