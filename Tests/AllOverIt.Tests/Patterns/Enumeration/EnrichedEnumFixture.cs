using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Patterns.Enumeration;
using AllOverIt.Patterns.Enumeration.Exceptions;
using FluentAssertions;
using System.Runtime.CompilerServices;

namespace AllOverIt.Tests.Patterns.Enumeration
{
    public class EnrichedEnumFixture : FixtureBase
    {
        private class DummyEnrichedEnum1 : EnrichedEnum<DummyEnrichedEnum1>
        {
            public static readonly DummyEnrichedEnum1 Value1 = new(1);
            public static readonly DummyEnrichedEnum1 Value2 = new(2, "Value 2");

            private DummyEnrichedEnum1(int value, [CallerMemberName] string name = null)
                : base(value, name)
            {
            }
        }

        private class DummyEnrichedEnum2 : EnrichedEnum<DummyEnrichedEnum2>
        {
            public static readonly DummyEnrichedEnum2 Value1 = new(1);
            public static readonly DummyEnrichedEnum2 Value2 = new(2, "Value 2");

            private DummyEnrichedEnum2(int value, [CallerMemberName] string name = null)
                : base(value, name)
            {
            }
        }

        // Cannot merge DummyBadEnrichedEnum1, DummyBadEnrichedEnum2, DummyBadEnrichedEnum3 due to static initialization
        private class DummyBadEnrichedEnum1 : EnrichedEnum<DummyBadEnrichedEnum1>
        {
            public static readonly DummyBadEnrichedEnum1 NullName = new(1, null);

            private DummyBadEnrichedEnum1(int value, [CallerMemberName] string name = null)
                : base(value, name)
            {
            }
        }

        private class DummyBadEnrichedEnum2 : EnrichedEnum<DummyBadEnrichedEnum2>
        {
            public static readonly DummyBadEnrichedEnum2 EmptyName = new(1, "");

            private DummyBadEnrichedEnum2(int value, [CallerMemberName] string name = null)
                : base(value, name)
            {
            }
        }

        private class DummyBadEnrichedEnum3 : EnrichedEnum<DummyBadEnrichedEnum3>
        {
            public static readonly DummyBadEnrichedEnum3 WhitespaceName = new(1, "  ");

            private DummyBadEnrichedEnum3(int value, [CallerMemberName] string name = null)
                : base(value, name)
            {
            }
        }

        public class Constructor : EnrichedEnumFixture
        {
            [Fact]
            public void Should_Throw_When_Name_Null()
            {
                Invoking(() =>
                {
                    _ = DummyBadEnrichedEnum1.NullName;
                })
                    .Should()
                    .Throw<TypeInitializationException>()
                    .WithInnerException<ArgumentNullException>()
                    .WithNamedMessageWhenNull("name");
            }

            [Fact]
            public void Should_Throw_When_Name_Empty()
            {
                Invoking(() =>
                {
                    _ = DummyBadEnrichedEnum2.EmptyName;
                })
                    .Should()
                    .Throw<TypeInitializationException>()
                    .WithInnerException<ArgumentException>()
                    .WithNamedMessageWhenEmpty("name");
            }

            [Fact]
            public void Should_Throw_When_Name_Whitespace()
            {
                Invoking(() =>
                {
                    _ = DummyBadEnrichedEnum3.WhitespaceName;
                })
                    .Should()
                    .Throw<TypeInitializationException>()
                    .WithInnerException<ArgumentException>()
                    .WithNamedMessageWhenEmpty("name");
            }

            [Fact]
            public void Should_Set_Value()
            {
                DummyEnrichedEnum1.Value1.Value.Should().Be(1);
                DummyEnrichedEnum1.Value2.Value.Should().Be(2);
            }

            [Fact]
            public void Should_Set_Name_Same_As_Field()
            {
                DummyEnrichedEnum1.Value1.Name.Should().Be(nameof(DummyEnrichedEnum1.Value1));
            }

            [Fact]
            public void Should_Set_Name_As_Specified()
            {
                DummyEnrichedEnum1.Value2.Name.Should().Be("Value 2");
            }
        }

        public class ToStringMethod : EnrichedEnumFixture
        {
            [Fact]
            public void Should_Return_Name()
            {
                DummyEnrichedEnum1.Value1.ToString().Should().Be(DummyEnrichedEnum1.Value1.Name);
                DummyEnrichedEnum1.Value2.ToString().Should().Be(DummyEnrichedEnum1.Value2.Name);
            }
        }

        public class CompareTo : EnrichedEnumFixture
        {
            [Fact]
            public void Should_Not_Compare()
            {
                var actual = DummyEnrichedEnum1.Value1.CompareTo(DummyEnrichedEnum1.Value2);
                actual.Should().NotBe(0);
            }

            [Fact]
            public void Should_Compare()
            {
                var actual = DummyEnrichedEnum1.Value2.CompareTo(DummyEnrichedEnum1.Value2);
                actual.Should().Be(0);
            }

            [Fact]
            public void Should_Not_Throw_When_Comparing_To_Null()
            {
                Invoking(() =>
                {
                    DummyEnrichedEnum1.Value2.CompareTo(null);
                })
                .Should()
                .NotThrow();
            }
        }

        public class Equals_Object : EnrichedEnumFixture
        {
            [Fact]
            public void Should_Be_Equal()
            {
                object value1 = DummyEnrichedEnum1.Value1;
                var actual = DummyEnrichedEnum1.Value1.Equals(value1);

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Not_Be_Equal_When_Null()
            {
                object nullValue = null;
                var actual = DummyEnrichedEnum1.Value1.Equals(nullValue);

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Not_Be_Equal_Same_Type()
            {
                object value2 = DummyEnrichedEnum1.Value2;
                var actual = DummyEnrichedEnum1.Value1.Equals(value2);

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Not_Be_Equal_Different_Type_Same_Value()
            {
                object value1 = DummyEnrichedEnum2.Value1;
                var actual = DummyEnrichedEnum1.Value1.Equals(value1);

                actual.Should().BeFalse();
            }
        }

        public class Equals_Typed : EnrichedEnumFixture
        {
            [Fact]
            public void Should_Be_Equal()
            {
                var value1 = DummyEnrichedEnum1.Value1;
                var actual = DummyEnrichedEnum1.Value1.Equals(value1);

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Not_Be_Equal_When_Null()
            {
                DummyEnrichedEnum1 nullValue = null;
                var actual = DummyEnrichedEnum1.Value1.Equals(nullValue);

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Not_Be_Equal_Same_Type()
            {
                var value2 = DummyEnrichedEnum1.Value2;
                var actual = DummyEnrichedEnum1.Value1.Equals(value2);

                actual.Should().BeFalse();
            }
        }

        public class GetHashCodeMethod : EnrichedEnumFixture
        {
            [Fact]
            public void Should_Return_Value_HashCode()
            {
                var expected = 2.GetHashCode();
                var actual = DummyEnrichedEnum1.Value2.GetHashCode();

                actual.Should().Be(expected);
            }
        }

        public class GetAllValues : EnrichedEnumFixture
        {
            [Fact]
            public void Should_Get_All_Values()
            {
                var actual = DummyEnrichedEnum1.GetAllValues();

                var expected = new[] { DummyEnrichedEnum1.Value1.Value, DummyEnrichedEnum1.Value2.Value };

                expected.Should().BeEquivalentTo(actual);
            }
        }

        public class GetAllNames : EnrichedEnumFixture
        {
            [Fact]
            public void Should_Get_All_Names()
            {
                var actual = DummyEnrichedEnum1.GetAllNames();

                var expected = new[] { DummyEnrichedEnum1.Value1.Name, DummyEnrichedEnum1.Value2.Name };

                expected.Should().BeEquivalentTo(actual);
            }
        }

        public class GetAll : EnrichedEnumFixture
        {
            [Fact]
            public void Should_Get_All()
            {
                var actual = DummyEnrichedEnum1.GetAll();

                var expected = new[] { DummyEnrichedEnum1.Value1, DummyEnrichedEnum1.Value2 };

                expected.Should().BeEquivalentTo(actual);
            }
        }

        public class From_Int_Value : EnrichedEnumFixture
        {
            [Fact]
            public void Should_Get_From_Number()
            {
                DummyEnrichedEnum1.From(1).Should().Be(DummyEnrichedEnum1.Value1);
                DummyEnrichedEnum1.From(2).Should().Be(DummyEnrichedEnum1.Value2);
            }

            [Fact]
            public void Should_Not_Get_From_Invalid_Value()
            {
                Invoking(() =>
                    {
                        _ = DummyEnrichedEnum1.From(0);
                    })
                    .Should()
                    .Throw<EnrichedEnumException>()
                    .WithMessage("Unable to convert '0' to a DummyEnrichedEnum1.");
            }
        }

        public class From_String_Value : EnrichedEnumFixture
        {
            [Fact]
            public void Should_Get_From_Number_String()
            {
                DummyEnrichedEnum1.From("1").Should().Be(DummyEnrichedEnum1.Value1);
                DummyEnrichedEnum1.From("2").Should().Be(DummyEnrichedEnum1.Value2);
            }

            [Fact]
            public void Should_Get_From_Name()
            {
                DummyEnrichedEnum1.From(nameof(DummyEnrichedEnum1.Value1)).Should().Be(DummyEnrichedEnum1.Value1);
                DummyEnrichedEnum1.From("Value 2").Should().Be(DummyEnrichedEnum1.Value2);
                DummyEnrichedEnum1.From("VALUE 2").Should().Be(DummyEnrichedEnum1.Value2);
            }

            [Fact]
            public void Should_Not_Get_From_Invalid_Value()
            {
                var value = Create<string>();

                Invoking(() =>
                    {
                        _ = DummyEnrichedEnum1.From(value);
                    })
                    .Should()
                    .Throw<EnrichedEnumException>()
                    .WithMessage($"Unable to convert '{value}' to a DummyEnrichedEnum1.");
            }

            [Fact]
            public void Should_Throw_When_Value_Null()
            {
                Invoking(() =>
                {
                    _ = DummyEnrichedEnum1.From((string) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("value");
            }

            [Fact]
            public void Should_Throw_When_Value_Empty()
            {
                Invoking(() =>
                {
                    _ = DummyEnrichedEnum1.From(string.Empty);
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("value");
            }

            [Fact]
            public void Should_Throw_When_Value_Whitespace()
            {
                Invoking(() =>
                {
                    _ = DummyEnrichedEnum1.From("  ");
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("value");
            }
        }

        public class TryFromValue : EnrichedEnumFixture
        {
            [Fact]
            public void Should_Get_From_Value()
            {
                var tryResult = DummyEnrichedEnum1.TryFromValue(1, out var actual);

                tryResult.Should().BeTrue();
                actual.Should().Be(DummyEnrichedEnum1.Value1);
            }

            [Fact]
            public void Should_Not_Get_From_Value()
            {
                var tryResult = DummyEnrichedEnum1.TryFromValue(-1, out var actual);

                tryResult.Should().BeFalse();
                actual.Should().BeNull();
            }
        }

        public class TryFromName : EnrichedEnumFixture
        {
            [Fact]
            public void Should_Get_From_Name()
            {
                var tryResult = DummyEnrichedEnum1.TryFromName("VALUE 2", out var actual);

                tryResult.Should().BeTrue();
                actual.Should().Be(DummyEnrichedEnum1.Value2);
            }

            [Fact]
            public void Should_Not_Get_From_Name()
            {
                var tryResult = DummyEnrichedEnum1.TryFromName(Create<string>(), out var actual);

                tryResult.Should().BeFalse();
                actual.Should().BeNull();
            }

            [Fact]
            public void Should_Throw_When_Name_Null()
            {
                Invoking(() =>
                {
                    _ = DummyEnrichedEnum1.TryFromName((string) null, out _);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("name");
            }

            [Fact]
            public void Should_Throw_When_Name_Empty()
            {
                Invoking(() =>
                {
                    _ = DummyEnrichedEnum1.TryFromName(string.Empty, out _);
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("name");
            }

            [Fact]
            public void Should_Throw_When_Name_Whitespace()
            {
                Invoking(() =>
                {
                    _ = DummyEnrichedEnum1.TryFromName("  ", out _);
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("name");
            }
        }

        public class TryFromNameOrValue : EnrichedEnumFixture
        {
            [Fact]
            public void Should_Convert_From_Value()
            {
                DummyEnrichedEnum1.TryFromNameOrValue("2", out var enumeration).Should().BeTrue();

                enumeration.Should().Be(DummyEnrichedEnum1.Value2);
            }

            [Fact]
            public void Should_Convert_From_Name()
            {
                DummyEnrichedEnum1.TryFromNameOrValue("Value 2", out var enumeration).Should().BeTrue();

                enumeration.Should().Be(DummyEnrichedEnum1.Value2);
            }

            [Fact]
            public void Should_Convert_From_Name_Case_Insensitive()
            {
                DummyEnrichedEnum1.TryFromNameOrValue("VaLuE 2", out var enumeration).Should().BeTrue();

                enumeration.Should().Be(DummyEnrichedEnum1.Value2);
            }

            [Fact]
            public void Should_Not_Convert_From_Value()
            {
                DummyEnrichedEnum1.TryFromNameOrValue("3", out _).Should().BeFalse();
            }

            [Fact]
            public void Should_Not_Convert_From_Name()
            {
                DummyEnrichedEnum1.TryFromNameOrValue("Value 1", out _).Should().BeFalse();
            }

            [Fact]
            public void Should_Throw_When_Name_Null()
            {
                Invoking(() =>
                {
                    _ = DummyEnrichedEnum1.TryFromNameOrValue((string) null, out _);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("nameOrValue");
            }

            [Fact]
            public void Should_Throw_When_Name_Empty()
            {
                Invoking(() =>
                {
                    _ = DummyEnrichedEnum1.TryFromNameOrValue(string.Empty, out _);
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("nameOrValue");
            }

            [Fact]
            public void Should_Throw_When_Name_Whitespace()
            {
                Invoking(() =>
                {
                    _ = DummyEnrichedEnum1.TryFromNameOrValue("  ", out _);
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("nameOrValue");
            }
        }

        public class HasValue : EnrichedEnumFixture
        {
            [Theory]
            [InlineData(1)]
            [InlineData(2)]
            public void Should_Find_Value(int value)
            {
                var actual = DummyEnrichedEnum1.HasValue(value);

                actual.Should().BeTrue();
            }

            [Theory]
            [InlineData(-1)]
            [InlineData(0)]
            public void Should_Not_Find_Value(int value)
            {
                var actual = DummyEnrichedEnum1.HasValue(value);

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
                var actual = DummyEnrichedEnum1.HasName(name);

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Not_Find_Name()
            {
                var actual = DummyEnrichedEnum1.HasName(Create<string>());

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Throw_When_Name_Null()
            {
                Invoking(() =>
                {
                    _ = DummyEnrichedEnum1.HasName((string) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("name");
            }

            [Fact]
            public void Should_Throw_When_Name_Empty()
            {
                Invoking(() =>
                {
                    _ = DummyEnrichedEnum1.HasName(string.Empty);
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("name");
            }

            [Fact]
            public void Should_Throw_When_Name_Whitespace()
            {
                Invoking(() =>
                {
                    _ = DummyEnrichedEnum1.HasName("  ");
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("name");
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
                var actual = DummyEnrichedEnum1.HasNameOrValue(name);

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Not_Find_Name_Or_Value()
            {
                DummyEnrichedEnum1.HasNameOrValue("-1").Should().BeFalse();
                DummyEnrichedEnum1.HasNameOrValue(Create<string>()).Should().BeFalse();
            }

            [Fact]
            public void Should_Throw_When_Name_Null()
            {
                Invoking(() =>
                {
                    _ = DummyEnrichedEnum1.HasNameOrValue((string) null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("nameOrValue");
            }

            [Fact]
            public void Should_Throw_When_Name_Empty()
            {
                Invoking(() =>
                {
                    _ = DummyEnrichedEnum1.HasNameOrValue(string.Empty);
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("nameOrValue");
            }

            [Fact]
            public void Should_Throw_When_Name_Whitespace()
            {
                Invoking(() =>
                {
                    _ = DummyEnrichedEnum1.HasNameOrValue("  ");
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("nameOrValue");
            }
        }

        public class Operator_Equals : EnrichedEnumFixture
        {
            [Fact]
            public void Should_Be_Equal()
            {
                var value1 = DummyEnrichedEnum1.Value1;
                var actual = DummyEnrichedEnum1.Value1 == value1;

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Not_Be_Equal_When_Null()
            {
                DummyEnrichedEnum1 nullValue = null;
                var actual = DummyEnrichedEnum1.Value1 == nullValue;

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Not_Be_Equal_Same_Type()
            {
                var actual = DummyEnrichedEnum1.Value1 == DummyEnrichedEnum1.Value2;

                actual.Should().BeFalse();
            }
        }

        public class Operator_NotEquals : EnrichedEnumFixture
        {
            [Fact]
            public void Should_Be_Equal()
            {
                var value1 = DummyEnrichedEnum1.Value1;
                var actual = DummyEnrichedEnum1.Value1 != value1;

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Not_Be_Equal_When_Null()
            {
                DummyEnrichedEnum1 nullValue = null;

                var actual = DummyEnrichedEnum1.Value1 != nullValue;

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Not_Be_Equal_Same_Type()
            {
                var actual = DummyEnrichedEnum1.Value1 != DummyEnrichedEnum1.Value2;

                actual.Should().BeTrue();
            }
        }

        public class Operator_GreaterThan : EnrichedEnumFixture
        {
            [Fact]
            public void Should_Be_GreaterThan()
            {
                var actual = DummyEnrichedEnum1.Value2 > DummyEnrichedEnum1.Value1;

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Not_Be_GreaterThan()
            {
                var actual = DummyEnrichedEnum1.Value1 > DummyEnrichedEnum1.Value2;

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Not_Be_GreaterThan_When_Equal()
            {
#pragma warning disable CS1718 // Comparison made to same variable
                var actual = DummyEnrichedEnum1.Value1 > DummyEnrichedEnum1.Value1;
#pragma warning restore CS1718 // Comparison made to same variable

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Not_Throw_When_Comparing_To_Null()
            {
                Invoking(() =>
                {
                    _ = DummyEnrichedEnum1.Value2 > null;
                })
                .Should()
                .NotThrow();
            }
        }

        public class Operator_GreaterThanOrEqual : EnrichedEnumFixture
        {
            [Fact]
            public void Should_Be_GreaterThan()
            {
                var actual = DummyEnrichedEnum1.Value2 >= DummyEnrichedEnum1.Value1;

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Be_Equal()
            {
                var value2 = DummyEnrichedEnum1.Value2;
                var actual = DummyEnrichedEnum1.Value2 >= value2;

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Not_Be_GreaterThanOrEqual()
            {
                var actual = DummyEnrichedEnum1.Value1 >= DummyEnrichedEnum1.Value2;

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Not_Throw_When_Comparing_To_Null()
            {
                Invoking(() =>
                {
                    _ = DummyEnrichedEnum1.Value2 >= null;
                })
                .Should()
                .NotThrow();
            }
        }

        public class Operator_LessThan : EnrichedEnumFixture
        {
            [Fact]
            public void Should_Be_LessThan()
            {
                var actual = DummyEnrichedEnum1.Value1 < DummyEnrichedEnum1.Value2;

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Not_Be_LessThan()
            {
                var actual = DummyEnrichedEnum1.Value2 < DummyEnrichedEnum1.Value1;

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Not_Be_LessThan_When_Equal()
            {
#pragma warning disable CS1718 // Comparison made to same variable
                var actual = DummyEnrichedEnum1.Value2 < DummyEnrichedEnum1.Value2;
#pragma warning restore CS1718 // Comparison made to same variable

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Not_Throw_When_Comparing_To_Null()
            {
                Invoking(() =>
                {
                    _ = DummyEnrichedEnum1.Value2 < null;
                })
                .Should()
                .NotThrow();
            }
        }

        public class Operator_LessThanOrEqual : EnrichedEnumFixture
        {
            [Fact]
            public void Should_Be_LessThan()
            {
                var actual = DummyEnrichedEnum1.Value1 <= DummyEnrichedEnum1.Value2;

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Be_Equal()
            {
                var value2 = DummyEnrichedEnum1.Value2;
                var actual = DummyEnrichedEnum1.Value2 <= value2;

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Not_Be_LessThanOrEqual()
            {
                var actual = DummyEnrichedEnum1.Value2 <= DummyEnrichedEnum1.Value1;

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Not_Throw_When_Comparing_To_Null()
            {
                Invoking(() =>
                {
                    _ = DummyEnrichedEnum1.Value2 <= null;
                })
                .Should()
                .NotThrow();
            }
        }

        public class Implicit_Operator : EnrichedEnumFixture
        {
            [Fact]
            public void Should_Convert()
            {
                int value = DummyEnrichedEnum1.Value2;

                value.Should().Be(2);
            }
        }

        public class Explicit_Operator : EnrichedEnumFixture
        {
            [Fact]
            public void Should_Convert()
            {
                var value = (DummyEnrichedEnum1) 2;

                value.Should().Be(DummyEnrichedEnum1.Value2);
            }
        }
    }
}