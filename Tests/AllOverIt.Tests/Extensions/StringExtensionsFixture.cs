using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace AllOverIt.Tests.Extensions
{
    public class StringExtensionsFixture : FixtureBase
    {
        public enum DummyEnum : short
        {
            Dummy1, Dummy2, Dummy3
        }

        public class As : StringExtensionsFixture
        {
            [Fact]
            public void Should_Return_Default_Value_If_Value_Is_Null()
            {
                var defaultValue = Create<int>();

                var actual = StringExtensions.As(null, defaultValue);

                actual.Should().Be(defaultValue);
            }

            [Fact]
            public void Should_Return_Default_Value_If_Value_Is_Whitespace()
            {
                var defaultValue = Create<int>();

                var actual = StringExtensions.As("  ", defaultValue);

                actual.Should().Be(defaultValue);
            }

            [Fact]
            public void Should_Return_Default_Value_If_Value_Is_Empty()
            {
                var defaultValue = Create<int>();

                var actual = StringExtensions.As(string.Empty, defaultValue);

                actual.Should().Be(defaultValue);
            }

            [Fact]
            public void Should_Throw_When_No_Suitable_Type_Converter()
            {
                var value = Create<string>();

                Invoking(
                    () => StringExtensions.As(value, Create<int>()))
                  .Should()
                  .Throw<ArgumentException>()
                  .WithMessage($"No converter exists for type 'Int32' when value = '{value}'.");
            }

            [Fact]
            public void Should_Return_The_Converted_Int_Value()
            {
                var value = Create<int>();

                var actual = StringExtensions.As($"{value}", Create<int>());

                actual.Should().Be(value);
            }

            [Fact]
            public void Should_Return_The_Converted_Double_Value()
            {
                var value = Create<double>();

                var actual = StringExtensions.As($"{value}", Create<double>());

                actual.Should().Be(value);
            }

            [Theory]
            [InlineData("1", true)]
            [InlineData("0", false)]
            [InlineData("true", true)]
            [InlineData("false", false)]
            [InlineData("True", true)]
            [InlineData("False", false)]
            [InlineData("TRUE", true)]
            [InlineData("FALSE", false)]
            public void Should_Return_The_Converted_Boolean_Value(string value, bool expected)
            {
                var actual = StringExtensions.As(value, Create<bool>());

                actual.Should().Be(expected);
            }

            [Theory]
            [InlineData(null, DummyEnum.Dummy2, DummyEnum.Dummy2)]
            [InlineData("", DummyEnum.Dummy2, DummyEnum.Dummy2)]
            [InlineData(" ", DummyEnum.Dummy2, DummyEnum.Dummy2)]
            [InlineData("Dummy2", DummyEnum.Dummy1, DummyEnum.Dummy2)]
            [InlineData("dummy2", DummyEnum.Dummy3, DummyEnum.Dummy2)]
            [InlineData("DUMMY2", DummyEnum.Dummy3, DummyEnum.Dummy2)]
            public void Should_Convert_To_Enum(string value, DummyEnum defaultValue, DummyEnum expected)
            {
                var actual = StringExtensions.As<DummyEnum>(value, defaultValue);

                actual.Should().Be(expected);
            }

            [Theory]
            [InlineData("0", NumberStyles.None)]
            [InlineData("none", NumberStyles.None)]
            [InlineData("None", NumberStyles.None)]
            [InlineData("NONE", NumberStyles.None)]

            [InlineData("1", NumberStyles.AllowLeadingWhite)]
            [InlineData("allowleadingwhite", NumberStyles.AllowLeadingWhite)]
            [InlineData("AllowLeadingWhite", NumberStyles.AllowLeadingWhite)]
            [InlineData("ALLOWLEADINGWHITE", NumberStyles.AllowLeadingWhite)]

            [InlineData("7", NumberStyles.Integer)]
            [InlineData("integer", NumberStyles.Integer)]
            [InlineData("Integer", NumberStyles.Integer)]
            [InlineData("INTEGER", NumberStyles.Integer)]
            public void Should_Convert_Enum_With_Flags_Attribute(string value, NumberStyles expected)
            {
                var actual = StringExtensions.As<NumberStyles>(value);

                actual.Should().Be(expected);
            }

            [Theory]
            [InlineData("0", DummyEnum.Dummy1)]
            [InlineData("1", DummyEnum.Dummy2)]
            [InlineData("2", DummyEnum.Dummy3)]
            public void Should_Convert_From_Number_String_To_Enum(string value, DummyEnum expected)
            {
                var actual = StringExtensions.As<DummyEnum>(value);

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Convert_String_To_Guid()
            {
                var expected = Guid.NewGuid();
                var value = expected.ToString();

                var actual = StringExtensions.As<Guid>(value);

                expected.Should().Be(actual);
            }
        }

        public class AsNullable : StringExtensionsFixture
        {
            [Fact]
            public void Should_Return_Null_If_Value_Is_Null()
            {
                var actual = StringExtensions.AsNullable<int>(null);

                actual.Should().NotHaveValue();
            }

            [Fact]
            public void Should_Return_Default_Value_If_Value_Is_Whitespace()
            {
                var actual = StringExtensions.AsNullable<int>("  ");

                actual.Should().NotHaveValue();
            }

            [Fact]
            public void Should_Return_Default_Value_If_Value_Is_Empty()
            {
                var actual = StringExtensions.AsNullable<int>(string.Empty);

                actual.Should().NotHaveValue();
            }

            [Fact]
            public void Should_Return_Default_Value_If_Value_Is_Not_Convertible()
            {
                var actual = StringExtensions.AsNullable<int>(Create<string>());

                actual.Should().NotHaveValue();
            }

            [Fact]
            public void Should_Return_Converted_Int_Value()
            {
                var value = Create<int>();

                var actual = StringExtensions.AsNullable<int>($"{value}");

                actual.Should().Be(value);
            }

            [Fact]
            public void Should_Return_The_Converted_Double_Value()
            {
                var value = Create<double>();

                var actual = StringExtensions.AsNullable<double>($"{value}");

                actual.Should().Be(value);
            }

            [Theory]
            [InlineData("true", true)]
            [InlineData("false", false)]
            [InlineData("True", true)]
            [InlineData("False", false)]
            [InlineData("TRUE", true)]
            [InlineData("FALSE", false)]
            public void Should_Return_The_Converted_Boolean_Value(string value, bool expected)
            {
                var actual = StringExtensions.AsNullable<bool>($"{value}");

                actual.Should().Be(expected);
            }

            [Theory]
            [InlineData("0", DummyEnum.Dummy1)]
            [InlineData("1", DummyEnum.Dummy2)]
            [InlineData("2", DummyEnum.Dummy3)]
            public void Should_Convert_From_Number_String_To_Nullable_Enum(string value, DummyEnum? expected)
            {
                var actual = StringExtensions.AsNullable<DummyEnum>($"{value}");

                actual.Should().Be(expected);
            }

            [Theory]
            [InlineData("dummy1", DummyEnum.Dummy1)]
            [InlineData("Dummy2", DummyEnum.Dummy2)]
            [InlineData("DUMMY3", DummyEnum.Dummy3)]
            public void Should_Return_Converted_Enum_Value(string value, DummyEnum? expected)
            {
                var actual = StringExtensions.AsNullable<DummyEnum>(value);

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Convert_String_To_Guid()
            {
                Guid? expected = Guid.NewGuid();
                var value = expected.ToString();

                var actual = StringExtensions.AsNullable<Guid>(value);

                expected.Should().Be(actual);
            }
        }

        public class IsNullOrEmpty : StringExtensionsFixture
        {
            private sealed class EmptyStrings : IEnumerable<object[]>
            {
                public IEnumerator<object[]> GetEnumerator()
                {
                    yield return new object[] { null };
                    yield return new object[] { string.Empty };
                    yield return new object[] { "   " };
                }

                IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            }

            [Fact]
            public void Should_Return_False()
            {
                var actual = Create<string>();

                actual.IsNullOrEmpty().Should().BeFalse();
            }

            [Theory]
            [ClassData(typeof(EmptyStrings))]
            public void Should_Return_True(string actual)
            {
                actual.IsNullOrEmpty().Should().BeTrue();
            }
        }

        public class IsNotNullOrEmpty : StringExtensionsFixture
        {
            private sealed class EmptyStrings : IEnumerable<object[]>
            {
                public IEnumerator<object[]> GetEnumerator()
                {
                    yield return new object[] { null };
                    yield return new object[] { string.Empty };
                    yield return new object[] { "   " };
                }

                IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            }

            [Fact]
            public void Should_Return_True()
            {
                var actual = Create<string>();

                actual.IsNotNullOrEmpty().Should().BeTrue();
            }

            [Theory]
            [ClassData(typeof(EmptyStrings))]
            public void Should_Return_False(string actual)
            {
                actual.IsNotNullOrEmpty().Should().BeFalse();
            }
        }

        public class IsEmpty : StringExtensionsFixture
        {
            private sealed class EmptyStrings : IEnumerable<object[]>
            {
                public IEnumerator<object[]> GetEnumerator()
                {
                    yield return new object[] { string.Empty };
                    yield return new object[] { "   " };
                }

                IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            }

            [Fact]
            public void Should_Return_False_When_Null()
            {
                string actual = default;

                actual.IsEmpty().Should().BeFalse();
            }

            [Fact]
            public void Should_Return_False_When_Not_Empty_Or_Whitespace()
            {
                var actual = Create<string>();

                actual.IsEmpty().Should().BeFalse();
            }

            [Theory]
            [ClassData(typeof(EmptyStrings))]
            public void Should_Return_True(string actual)
            {
                actual.IsEmpty().Should().BeTrue();
            }
        }

        public class ToBase64 : StringExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Null()
            {
                string value = null;

                Invoking(() =>
                    {
                        value.ToBase64();
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("value");
            }

            [Fact]
            public void Should_Be_Empty_When_Empty()
            {
                var value = string.Empty;

                value.ToBase64()
                    .Should()
                    .Be(string.Empty);
            }

            [Theory]
            [MemberData(nameof(Base64Data))]
            public void Should_Convert_From_Known_Phrase(string phrase, string base64)
            {
                phrase.ToBase64()
                    .Should()
                    .Be(base64);
            }
        }

        public class FromBase64 : StringExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Null()
            {
                string value = null;

                Invoking(() =>
                    {
                        value.FromBase64();
                    })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("value");
            }

            [Fact]
            public void Should_Be_Empty_When_Empty()
            {
                var value = string.Empty;

                value.FromBase64()
                    .Should()
                    .Be(string.Empty);
            }

            [Theory]
            [MemberData(nameof(Base64Data))]
            public void Should_Convert_To_Known_Phrase(string phrase, string base64)
            {
                base64.FromBase64()
                    .Should()
                    .Be(phrase);
            }
        }

        public class ToMemoryStream : StringExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Null()
            {
                string value = null;

                Invoking(() =>
                {
                    value.ToMemoryStream();
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("value");
            }

            [Fact]
            public void Should_Be_Empty_When_Empty()
            {
                var value = string.Empty;

                value.ToMemoryStream()
                    .Length
                    .Should()
                    .Be(0);
            }

            [Fact]
            public void Should_Copy_To_Stream()
            {
                var expected = Create<string>();

                var stream = expected.ToMemoryStream();

                var actual = Encoding.UTF8.GetString(stream.ToArray());

                actual.Should().Be(expected);
            }
        }

        public static IEnumerable<object[]> Base64Data =>
            new List<object[]>
            {
                new object[] {"The quick brown fox jumped over the lazy dog.", "VGhlIHF1aWNrIGJyb3duIGZveCBqdW1wZWQgb3ZlciB0aGUgbGF6eSBkb2cu"},
                new object[] {"THE QUICK BROWN FOX JUMPED OVER THE LAZY DOG!", "VEhFIFFVSUNLIEJST1dOIEZPWCBKVU1QRUQgT1ZFUiBUSEUgTEFaWSBET0ch"},
                new object[] { "AllOverIt!", "QWxsT3Zlckl0IQ==" }
            };
    }
}
