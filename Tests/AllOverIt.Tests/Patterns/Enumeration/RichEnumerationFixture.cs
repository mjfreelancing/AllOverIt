using AllOverIt.Exceptions;
using AllOverIt.Fixture;
using AllOverIt.Patterns.Enumeration;
using FluentAssertions;
using System.Runtime.CompilerServices;
using Xunit;

namespace AllOverIt.Tests.Patterns.Enumeration
{
    public class RichEnumerationFixture : FixtureBase
    {
        private class RichEnumDummy : RichEnum<RichEnumDummy>
        {
            public static readonly RichEnumDummy Value1 = new(1);

            // ReSharper disable once ExplicitCallerInfoArgument
            public static readonly RichEnumDummy Value2 = new(2, "Value 2");

            private RichEnumDummy(int value, [CallerMemberName] string name = null)
                : base(value, name)
            {
            }
        }

        private class RichEnumDummy2 : RichEnum<RichEnumDummy2>
        {
            public static readonly RichEnumDummy2 Value1 = new(1);

            // ReSharper disable once ExplicitCallerInfoArgument
            public static readonly RichEnumDummy2 Value2 = new(2, "Value 2");

            private RichEnumDummy2(int value, [CallerMemberName] string name = null)
                : base(value, name)
            {
            }
        }

        public class Constructor : RichEnumerationFixture
        {
            [Fact]
            public void Should_Set_Value()
            {
                RichEnumDummy.Value1.Value.Should().Be(1);
                RichEnumDummy.Value2.Value.Should().Be(2);
            }

            [Fact]
            public void Should_Set_Name_Same_As_Field()
            {
                RichEnumDummy.Value1.Name.Should().Be(nameof(RichEnumDummy.Value1));
            }

            [Fact]
            public void Should_Set_Name_As_Specified()
            {
                RichEnumDummy.Value2.Name.Should().Be("Value 2");
            }
        }

        public class ToStringMethod : RichEnumerationFixture
        {
            [Fact]
            public void Should_Return_Name()
            {
                RichEnumDummy.Value1.ToString().Should().Be(RichEnumDummy.Value1.Name);
                RichEnumDummy.Value2.ToString().Should().Be(RichEnumDummy.Value2.Name);
            }
        }

        public class CompareTo : RichEnumerationFixture
        {
            [Fact]
            public void Should_Not_Compare()
            {
                var actual = RichEnumDummy.Value1.CompareTo(RichEnumDummy.Value2);
                actual.Should().NotBe(0);
            }

            [Fact]
            public void Should_Compare()
            {
                var actual = RichEnumDummy.Value2.CompareTo(RichEnumDummy.Value2);
                actual.Should().Be(0);
            }
        }

        public class Equals_Object : RichEnumerationFixture
        {
            [Fact]
            public void Should_Be_Equal()
            {
                object enum1 = RichEnumDummy.Value1;
                var actual = RichEnumDummy.Value1.Equals(enum1);

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Not_Be_Equal_When_Null()
            {
                object enumNull = null;
                var actual = RichEnumDummy.Value1.Equals(enumNull);

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Not_Be_Equal_Same_Type()
            {
                object enum2 = RichEnumDummy.Value2;
                var actual = RichEnumDummy.Value1.Equals(enum2);

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Not_Be_Equal_Different_Type_Same_Value()
            {
                object enum1 = RichEnumDummy2.Value1;
                var actual = RichEnumDummy.Value1.Equals(enum1);

                actual.Should().BeFalse();
            }
        }

        public class Equals_Typed : RichEnumerationFixture
        {
            [Fact]
            public void Should_Be_Equal()
            {
                var enum1 = RichEnumDummy.Value1;
                var actual = RichEnumDummy.Value1.Equals(enum1);

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Not_Be_Equal_When_Null()
            {
                RichEnumDummy enumNull = null;
                var actual = RichEnumDummy.Value1.Equals(enumNull);

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Not_Be_Equal_Same_Type()
            {
                var enum2 = RichEnumDummy.Value2;
                var actual = RichEnumDummy.Value1.Equals(enum2);

                actual.Should().BeFalse();
            }
        }

        public class GetHashCodeMethod : RichEnumerationFixture
        {
            [Fact]
            public void Should_Return_Value_HashCode()
            {
                var expected = 2.GetHashCode();
                var actual = RichEnumDummy.Value2.GetHashCode();

                actual.Should().Be(expected);
            }
        }

        public class GetAllValues : RichEnumerationFixture
        {
            [Fact]
            public void Should_Get_All_Values()
            {
                var actual = RichEnumDummy.GetAllValues();

                actual.Should().BeEquivalentTo(new[] { RichEnumDummy.Value1.Value, RichEnumDummy.Value2.Value });
            }
        }

        public class GetAllNames : RichEnumerationFixture
        {
            [Fact]
            public void Should_Get_All_Names()
            {
                var actual = RichEnumDummy.GetAllNames();

                actual.Should().BeEquivalentTo(RichEnumDummy.Value1.Name, RichEnumDummy.Value2.Name);
            }
        }

        public class GetAll : RichEnumerationFixture
        {
            [Fact]
            public void Should_Get_All()
            {
                var actual = RichEnumDummy.GetAll();

                actual.Should().BeEquivalentTo(new[] { RichEnumDummy.Value1, RichEnumDummy.Value2 });
            }
        }

        public class From_Int_Value : RichEnumerationFixture
        {
            [Fact]
            public void Should_Get_From_Number()
            {
                RichEnumDummy.From(1).Should().Be(RichEnumDummy.Value1);
                RichEnumDummy.From(2).Should().Be(RichEnumDummy.Value2);
            }

            [Fact]
            public void Should_Not_Get_From_Invalid_Value()
            {
                Invoking(() =>
                    {
                        _ = RichEnumDummy.From(0);
                    })
                    .Should()
                    .Throw<RichEnumException>()
                    .WithMessage("Unable to convert '0' to a RichEnumDummy.");
            }
        }

        public class From_String_Value : RichEnumerationFixture
        {
            [Fact]
            public void Should_Get_From_Number_String()
            {
                RichEnumDummy.From("1").Should().Be(RichEnumDummy.Value1);
                RichEnumDummy.From("2").Should().Be(RichEnumDummy.Value2);
            }

            [Fact]
            public void Should_Get_From_Name()
            {
                RichEnumDummy.From(nameof(RichEnumDummy.Value1)).Should().Be(RichEnumDummy.Value1);
                RichEnumDummy.From("Value 2").Should().Be(RichEnumDummy.Value2);
                RichEnumDummy.From("VALUE 2").Should().Be(RichEnumDummy.Value2);
            }

            [Fact]
            public void Should_Not_Get_From_Invalid_Value()
            {
                var value = Create<string>();

                Invoking(() =>
                    {
                        _ = RichEnumDummy.From(value);
                    })
                    .Should()
                    .Throw<RichEnumException>()
                    .WithMessage($"Unable to convert '{value}' to a RichEnumDummy.");
            }
        }

        public class TryFromNameOrValue : RichEnumerationFixture
        {
            [Fact]
            public void Should_Convert_From_Value()
            {
                RichEnumDummy.TryFromNameOrValue("2", out var enumeration).Should().BeTrue();

                enumeration.Should().Be(RichEnumDummy.Value2);
            }

            [Fact]
            public void Should_Convert_From_Name()
            {
                RichEnumDummy.TryFromNameOrValue("Value 2", out var enumeration).Should().BeTrue();

                enumeration.Should().Be(RichEnumDummy.Value2);
            }

            [Fact]
            public void Should_Convert_From_Name_Case_Insensitive()
            {
                RichEnumDummy.TryFromNameOrValue("VaLuE 2", out var enumeration).Should().BeTrue();

                enumeration.Should().Be(RichEnumDummy.Value2);
            }

            [Fact]
            public void Should_Not_Convert_From_Value()
            {
                RichEnumDummy.TryFromNameOrValue("3", out var _).Should().BeFalse();
            }

            [Fact]
            public void Should_Not_Convert_From_Name()
            {
                RichEnumDummy.TryFromNameOrValue("Value 1", out var _).Should().BeFalse();
            }
        }
    }
}