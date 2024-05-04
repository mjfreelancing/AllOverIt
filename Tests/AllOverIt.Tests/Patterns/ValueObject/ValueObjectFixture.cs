using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Patterns.ValueObject;
using AllOverIt.Patterns.ValueObject.Exceptions;
using FluentAssertions;

namespace AllOverIt.Tests.Patterns.ValueObject
{
    public class ValueObjectFixture : FixtureBase
    {
        private class StringValueObject : ValueObject<string, StringValueObject>
        {
            public StringValueObject()
            {
            }

            public StringValueObject(string value)
                : base(value)
            {
            }

            protected override bool ValidateValue(string value)
            {
                // not allowing palindrome
                return value == null || value != ReverseString(value);
            }
        }

        public class Constructor : ValueObjectFixture
        {
            [Fact]
            public void Should_Set_Default_Value()
            {
                var actual = new StringValueObject();

                actual.Value.Should().BeNull();
            }

            [Fact]
            public void Should_Validate_Value()
            {
                Invoking(() =>
                {
                    _ = new StringValueObject("aba");
                })
                .Should()
                .Throw<ValueObjectValidationException>()
                .WithMessage("Invalid value 'aba'.");
            }

            [Fact]
            public void Should_Set_Value()
            {
                var value = Create<string>();

                var actual = new StringValueObject(value);

                actual.Value.Should().Be(value);
            }
        }

        public class Equals_Object : ValueObjectFixture
        {
            [Fact]
            public void Should_Be_Equal_By_Value()
            {
                var value = Create<string>();
                var string1 = new StringValueObject(value);
                object string2 = new StringValueObject(value);

                var actual = string1.Equals(string2);

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Be_Equal_By_Reference()
            {
                var value = Create<string>();
                var string1 = new StringValueObject(value);
                object string2 = string1;

                var actual = string1.Equals(string2);

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Not_Be_Equal_When_Null()
            {
                var value = Create<string>();
                var string1 = new StringValueObject(value);
                object string2 = null;

                var actual = string1.Equals(string2);

                actual.Should().BeFalse();
            }
        }

        public class Equals_ValueObject : ValueObjectFixture
        {
            [Fact]
            public void Should_Be_Equal_By_Value()
            {
                var value = Create<string>();
                var string1 = new StringValueObject(value);
                var string2 = new StringValueObject(value);

                var actual = string1.Equals(string2);

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Be_Equal_By_Reference()
            {
                var value = Create<string>();
                var string1 = new StringValueObject(value);
                var string2 = string1;

                var actual = string1.Equals(string2);

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Not_Be_Equal_When_Null()
            {
                var value = Create<string>();
                var string1 = new StringValueObject(value);
                StringValueObject string2 = null;

                var actual = string1.Equals(string2);

                actual.Should().BeFalse();
            }
        }

        public class CompareTo : ValueObjectFixture
        {
            [Fact]
            public void Should_Not_Compare()
            {
                var string1 = new StringValueObject(Create<string>());
                var string2 = new StringValueObject(Create<string>());

                var actual = string1.CompareTo(string2);
                actual.Should().NotBe(0);
            }

            [Fact]
            public void Should_Compare()
            {
                var value = Create<string>();
                var string1 = new StringValueObject(value);
                var string2 = new StringValueObject(value);

                var actual = string1.CompareTo(string2);
                actual.Should().Be(0);
            }

            [Fact]
            public void Should_Throw_When_Null()
            {
                Invoking(() =>
                {
                    var value = new StringValueObject(Create<string>());

                    value.CompareTo(null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("other");
            }
        }

        public class Operator_Equals : ValueObjectFixture
        {
            [Fact]
            public void Should_Be_Equal()
            {
                var value = Create<string>();
                var string1 = new StringValueObject(value);
                var string2 = new StringValueObject(value);

                var actual = string1 == string2;

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Not_Be_Equal_When_Null()
            {
                var value = Create<string>();
                var string1 = new StringValueObject(value);

                var actual = string1 == null;

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Not_Be_Equal()
            {
                var string1 = new StringValueObject(Create<string>());
                var string2 = new StringValueObject(Create<string>());

                var actual = string1 == string2;

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Be_Equal_When_Both_Null()
            {
                StringValueObject string1 = null;
                StringValueObject string2 = null;

                var actual = string1 == string2;

                actual.Should().BeTrue();
            }
        }

        public class Operator_NotEquals : ValueObjectFixture
        {
            [Fact]
            public void Should_Be_Equal()
            {
                var value = Create<string>();
                var string1 = new StringValueObject(value);
                var string2 = new StringValueObject(value);

                var actual = string1 != string2;

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Not_Be_Equal_When_Null()
            {
                var value = Create<string>();
                var string1 = new StringValueObject(value);

                var actual = string1 != null;

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Not_Be_Equal()
            {
                var string1 = new StringValueObject(Create<string>());
                var string2 = new StringValueObject(Create<string>());

                var actual = string1 != string2;

                actual.Should().BeTrue();
            }
        }

        public class Operator_GreaterThan : ValueObjectFixture
        {
            [Fact]
            public void Should_Be_GreaterThan()
            {
                var string1 = new StringValueObject("ba");
                var string2 = new StringValueObject("ab");

                var actual = string1 > string2;

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Not_Be_GreaterThan()
            {
                var string1 = new StringValueObject("ab");
                var string2 = new StringValueObject("ba");

                var actual = string1 > string2;

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Not_Be_GreaterThan_When_Equal()
            {
                var string1 = new StringValueObject("ab");
                var string2 = new StringValueObject("ab");

                var actual = string1 > string2;

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Throw_When_Null()
            {
                Invoking(() =>
                {
                    var string1 = new StringValueObject("ab");

                    var actual = string1 > null;
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("other");
            }
        }

        public class Operator_GreaterThanOrEqual : ValueObjectFixture
        {
            [Fact]
            public void Should_Be_GreaterThanOrEqual()
            {
                var string1 = new StringValueObject("ba");
                var string2 = new StringValueObject("ab");

                var actual = string1 >= string2;

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Not_Be_GreaterThanOrEqual()
            {
                var string1 = new StringValueObject("ab");
                var string2 = new StringValueObject("ba");

                var actual = string1 >= string2;

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Be_GreaterThanOrEqual_When_Equal()
            {
                var string1 = new StringValueObject("ab");
                var string2 = new StringValueObject("ab");

                var actual = string1 >= string2;

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Throw_When_Null()
            {
                Invoking(() =>
                {
                    var string1 = new StringValueObject("ab");

                    var actual = string1 >= null;
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("other");
            }
        }

        public class Operator_LessThan : ValueObjectFixture
        {
            [Fact]
            public void Should_Be_LessThan()
            {
                var string1 = new StringValueObject("ab");
                var string2 = new StringValueObject("ba");

                var actual = string1 < string2;

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Not_Be_LessThan()
            {
                var string1 = new StringValueObject("ba");
                var string2 = new StringValueObject("ab");

                var actual = string1 < string2;

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Not_Be_LessThan_When_Equal()
            {
                var string1 = new StringValueObject("ab");
                var string2 = new StringValueObject("ab");

                var actual = string1 < string2;

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Throw_When_Null()
            {
                Invoking(() =>
                {
                    var string1 = new StringValueObject("ab");

                    var actual = string1 < null;
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("other");
            }
        }

        public class Operator_LessThanOrEqual : ValueObjectFixture
        {
            [Fact]
            public void Should_Be_GreaterThanOrEqual()
            {
                var string1 = new StringValueObject("ba");
                var string2 = new StringValueObject("ab");

                var actual = string1 <= string2;

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Not_Be_LessThanOrEqual()
            {
                var string1 = new StringValueObject("ab");
                var string2 = new StringValueObject("ba");

                var actual = string1 <= string2;

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Be_LessThanOrEqual_When_Equal()
            {
                var string1 = new StringValueObject("ab");
                var string2 = new StringValueObject("ab");

                var actual = string1 <= string2;

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Throw_When_Null()
            {
                Invoking(() =>
                {
                    var string1 = new StringValueObject("ab");

                    var actual = string1 <= null;
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("other");
            }
        }

        public class Implicit_Operator : ValueObjectFixture
        {
            [Fact]
            public void Should_Convert()
            {
                var string1 = new StringValueObject(Create<string>());
                string string2 = string1;

                var same = string1.Value == string2;

                same.Should().BeTrue();
            }
        }

        public class Explicit_Operator : ValueObjectFixture
        {
            [Fact]
            public void Should_Convert()
            {
                var value = Create<string>();
                var string1 = (ValueObject<string, StringValueObject>) value;

                var same = value == string1.Value;

                same.Should().BeTrue();
            }
        }

        public class GetHashCodeMethod : ValueObjectFixture
        {
            [Fact]
            public void Should_Convert()
            {
                var value = Create<string>();
                var string1 = (ValueObject<string, StringValueObject>) value;

                var expected = value.GetHashCode();
                var actual = string1.GetHashCode();

                actual.Should().Be(expected);
            }
        }
        private static string ReverseString(string value)
        {
            var array = value.ToCharArray();
            Array.Reverse(array);
            return new string(array);
        }
    }
}