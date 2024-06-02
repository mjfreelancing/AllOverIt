using AllOverIt.Extensions;
using AllOverIt.Fixture.Exceptions;
using AllOverIt.Fixture.FakeItEasy;
using AllOverIt.Fixture.Tests.Dummies;
using AllOverIt.Patterns.Enumeration;
using AutoFixture;
using AutoFixture.Dsl;
using FakeItEasy;
using FluentAssertions;
using System.Runtime.CompilerServices;

namespace AllOverIt.Fixture.Tests
{
    public class FixtureBaseFixture : FixtureBase
    {
        private sealed class DefaultConstructorExceptionDummy : Exception
        {
            public DefaultConstructorExceptionDummy()
                : base("abc")
            {
            }
        }

        private sealed class NoDefaultConstructorExceptionDummy : Exception
        {
            public NoDefaultConstructorExceptionDummy(string message)
                : base(message)
            {
            }
        }

        private sealed class ConstructorWithMessageExceptionDummy : Exception
        {
            public ConstructorWithMessageExceptionDummy(string message)
                : base(message)
            {
            }
        }

        public class Constructor : FixtureBaseFixture
        {
            private class FixtureDummy : FixtureBase
            {
                public FixtureDummy()
                {
                }

                public FixtureDummy(IFixture fixture)
                    : base(fixture)
                {
                }

                public FixtureDummy(ICustomization customization)
                    : base(customization)
                {
                }
            }

            [Fact]
            public void Should_Use_AutoFixture()
            {
                Fixture.Should().BeOfType<AutoFixture.Fixture>();
            }

            [Fact]
            public void Should_Have_Customizations()
            {
                Fixture.Customizations.Should().ContainItemsAssignableTo<NodeComposer<float>>();
                Fixture.Customizations.Should().ContainItemsAssignableTo<NodeComposer<double>>();
                Fixture.Customizations.Should().ContainItemsAssignableTo<NodeComposer<decimal>>();
            }

            [Fact]
            public void Should_Use_Custom_Fixture()
            {
                var fixtureFake = this.CreateStub<IFixture>();
                var fixture = new FixtureDummy(fixtureFake);

                fixture.Fixture.Should().BeSameAs(fixtureFake);
            }

            [Fact]
            public void Should_Add_Customization()
            {
                var fixtureFake = this.CreateStub<IFixture>();
                var fixture = new FixtureDummy(fixtureFake);

                var customizationFake = this.CreateStub<ICustomization>();
                fixture.Customize(customizationFake);

                A.CallTo(() => fixtureFake.Customize(customizationFake)).MustHaveHappenedOnceExactly();
            }
        }

        public class Invoking_Action : FixtureBaseFixture
        {
            [Fact]
            public void Should_Throw_When_Action_Null()
            {
                Action action = () => Invoking(null);

                action
                  .Should()
                  .Throw<ArgumentNullException>();
            }

            [Fact]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0039:Use local function", Justification = "It's part of the test")]
            public void Should_Return_Same_Action()
            {
                Action toInvoke = () => { };

                var invoked = Invoking(toInvoke);

                invoked.Should().BeSameAs(toInvoke);
            }
        }

        public class Invoking_Func : FixtureBaseFixture
        {
            [Fact]
            public void Should_Throw_When_Action_Null()
            {
                Action action = () => Invoking((Func<bool>) null);

                action
                  .Should()
                  .Throw<ArgumentNullException>();
            }

            [Fact]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0039:Use local function", Justification = "It's part of the test")]
            public void Should_Return_Same_Action()
            {
                Func<string> toInvoke = () => Create<string>();

                var invoked = Invoking(toInvoke);

                invoked.Should().BeSameAs(toInvoke);
            }
        }

        public class Inject_ : FixtureBaseFixture
        {
            [Fact]
            public void Should_Inject_Constant()
            {
                var expected = Create<int>();

                Inject(expected);

                var actual = Create<int>();

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Inject_Instance()
            {
                var expected = Create<DummyClass>();

                Inject(expected);

                var actual = Create<DummyClass>();

                actual.Should().BeSameAs(expected);
            }

            [Fact]
            public void Should_Return_Same_Instance()
            {
                var expected = Create<DummyClass>();

                Inject(expected);

                var items = CreateMany<DummyClass>();

                foreach (var actual in items)
                {
                    actual.Should().BeSameAs(expected);
                }
            }
        }

        public class Register_No_Arg : FixtureBaseFixture
        {
            [Fact]
            public void Should_Throw_If_Creator_Null()
            {
                Invoking(() => Register<DummyClass>(null))
                  .Should()
                  .Throw<ArgumentNullException>();
            }

            [Fact]
            public void Should_Call_Creator()
            {
                var creatorCalled = false;

                DummyClass creator()
                {
                    creatorCalled = true;
                    return new DummyClass();
                }

                Register(creator);

                Create<DummyClass>();

                creatorCalled.Should().BeTrue();
            }

            [Fact]
            public void Should_Create_DummyClassThreeArgs()
            {
                static DummyClass creator() => new();

                Register(creator);

                var actual = Create<DummyClass>();

                actual.Should().BeOfType<DummyClass>();
            }
        }

        public class Register_One_Arg : FixtureBaseFixture
        {
            [Fact]
            public void Should_Throw_If_Creator_Null()
            {
                Invoking(() => Register<int, DummyClassOneArg>(null))
                  .Should()
                  .Throw<ArgumentNullException>();
            }

            [Fact]
            public void Should_Call_Creator()
            {
                var creatorCalled = false;

                DummyClassOneArg creator(int arg1)
                {
                    creatorCalled = true;
                    return new DummyClassOneArg(arg1);
                }

                Register((Func<int, DummyClassOneArg>) creator);

                Create<DummyClassOneArg>();

                creatorCalled.Should().BeTrue();
            }

            [Fact]
            public void Should_Create_DummyClassThreeArgs()
            {
                static DummyClassOneArg creator(int arg1) => new(arg1);

                Register((Func<int, DummyClassOneArg>) creator);

                var actual = Create<DummyClassOneArg>();

                actual.Should().BeOfType<DummyClassOneArg>();
            }
        }

        public class Register_Two_Args : FixtureBaseFixture
        {
            [Fact]
            public void Should_Throw_If_Creator_Null()
            {
                Invoking(() => Register<int, double, DummyClassTwoArgs>(null))
                  .Should()
                  .Throw<ArgumentNullException>();
            }

            [Fact]
            public void Should_Call_Creator()
            {
                var creatorCalled = false;

                DummyClassTwoArgs creator(int arg1, double arg2)
                {
                    creatorCalled = true;
                    return new DummyClassTwoArgs(arg1, arg2);
                }

                Register((Func<int, double, DummyClassTwoArgs>) creator);

                Create<DummyClassTwoArgs>();

                creatorCalled.Should().BeTrue();
            }

            [Fact]
            public void Should_Create_DummyClassThreeArgs()
            {
                static DummyClassTwoArgs creator(int arg1, double arg2) => new(arg1, arg2);

                Register((Func<int, double, DummyClassTwoArgs>) creator);

                var actual = Create<DummyClassTwoArgs>();

                actual.Should().BeOfType<DummyClassTwoArgs>();
            }
        }

        public class Register_Three_Args : FixtureBaseFixture
        {
            [Fact]
            public void Should_Throw_If_Creator_Null()
            {
                Invoking(() => Register<int, double, DummyEnum, DummyClassThreeArgs>(null))
                  .Should()
                  .Throw<ArgumentNullException>();
            }

            [Fact]
            public void Should_Call_Creator()
            {
                var creatorCalled = false;

                DummyClassThreeArgs creator(int arg1, double arg2, DummyEnum arg3)
                {
                    creatorCalled = true;
                    return new DummyClassThreeArgs(arg1, arg2, arg3);
                }

                Register((Func<int, double, DummyEnum, DummyClassThreeArgs>) creator);

                Create<DummyClassThreeArgs>();

                creatorCalled.Should().BeTrue();
            }

            [Fact]
            public void Should_Create_DummyClassThreeArgs()
            {
                static DummyClassThreeArgs creator(int arg1, double arg2, DummyEnum arg3) => new(arg1, arg2, arg3);

                Register((Func<int, double, DummyEnum, DummyClassThreeArgs>) creator);

                var actual = Create<DummyClassThreeArgs>();

                actual.Should().BeOfType<DummyClassThreeArgs>();
            }
        }

        public class Register_Four_Args : FixtureBaseFixture
        {
            [Fact]
            public void Should_Throw_If_Creator_Null()
            {
                Invoking(() => Register<int, double, DummyEnum, float, DummyClassFourArgs>(null))
                  .Should()
                  .Throw<ArgumentNullException>();
            }

            [Fact]
            public void Should_Call_Creator()
            {
                var creatorCalled = false;

                DummyClassFourArgs creator(int arg1, double arg2, DummyEnum arg3, float arg4)
                {
                    creatorCalled = true;
                    return new DummyClassFourArgs(arg1, arg2, arg3, arg4);
                }

                Register((Func<int, double, DummyEnum, float, DummyClassFourArgs>) creator);

                Create<DummyClassFourArgs>();

                creatorCalled.Should().BeTrue();
            }

            [Fact]
            public void Should_Create_DummyClassFourArgs()
            {
                static DummyClassFourArgs creator(int arg1, double arg2, DummyEnum arg3, float arg4) => new(arg1, arg2, arg3, arg4);

                Register((Func<int, double, DummyEnum, float, DummyClassFourArgs>) creator);

                var actual = Create<DummyClassFourArgs>();

                actual.Should().BeOfType<DummyClassFourArgs>();
            }
        }

        public class Create_ : FixtureBaseFixture
        {
            private class DummyEnrichedEnum : EnrichedEnum<DummyEnrichedEnum>
            {
                public static readonly DummyEnrichedEnum One = new(1);
                public static readonly DummyEnrichedEnum Two = new(2);
                public static readonly DummyEnrichedEnum Three = new(3);
                public static readonly DummyEnrichedEnum Four = new(4);

                private DummyEnrichedEnum(int value, [CallerMemberName] string name = null)
                    : base(value, name)
                {
                }
            }

            [Fact]
            public void Should_Create_String()
            {
                var value = Create<string>();

                value.Should().NotBeNullOrEmpty();
            }

            [Fact]
            public void Should_Create_Random_Enum()
            {
                var value1 = Create<DummyEnum>();
                var value2 = Create<DummyEnum>();

                while (value2 - value1 == 1)
                {
                    value2 = Create<DummyEnum>();
                }

                (value2 - value1).Should().NotBe(1);
            }

            [Fact]
            public void Should_Create_Enriched_Enum()
            {
                Customize<DummyEnrichedEnum>();

                var value = Create<DummyEnrichedEnum>();

                value.Should().BeOfType<DummyEnrichedEnum>();
            }

            [Fact]
            public void Should_Create_Populated_Class()
            {
                var expectedInt = Create<int>();
                var expectedString = Create<string>();

                Fixture.Register(() => expectedInt);
                Fixture.Register(() => expectedString);

                var value = Create<DummyClass>();

                value.IntValue.Should().Be(expectedInt);
                value.StringValue.Should().Be(expectedString);
            }
        }

        public class CreateExcluding_Int : FixtureBaseFixture
        {
            [Fact]
            public void Should_Exclude_Value()
            {
                var excludedValue = Create<int>();

                var value = CreateExcluding(excludedValue);

                value.Should().NotBe(excludedValue);
            }

            [Fact]
            public void Should_Exclude_Values()
            {
                var excludedValues = CreateMany<int>().ToArray();

                var value = CreateExcluding(excludedValues);

                foreach (var excludedValue in excludedValues)
                {
                    value.Should().NotBe(excludedValue);
                }
            }
        }

        public class CreateExcluding_Double : FixtureBaseFixture
        {
            [Fact]
            public void Should_Exclude_Value()
            {
                var excludedValue = Create<double>();

                var value = CreateExcluding(excludedValue);

                value.Should().NotBe(excludedValue);
            }

            [Fact]
            public void Should_Exclude_Values()
            {
                var excludedValues = CreateMany<double>().ToArray();

                var value = CreateExcluding(excludedValues);

                foreach (var excludedValue in excludedValues)
                {
                    value.Should().NotBe(excludedValue);
                }
            }
        }

        public class CreateExcluding_Enum : FixtureBaseFixture
        {
            [Fact]
            public void Should_Exclude_Value()
            {
                var excludedValue = Create<DummyEnum>();

                var value = CreateExcluding(excludedValue);

                value.Should().NotBe(excludedValue);
            }

            [Fact]
            public void Should_Exclude_Values()
            {
                var excludedValues = CreateMany<DummyEnum>(3).ToArray();

                var value = CreateExcluding(excludedValues);

                foreach (var excludedValue in excludedValues)
                {
                    value.Should().NotBe(excludedValue);
                }
            }
        }

        public class CreateExcluding_Dummy : FixtureBaseFixture
        {
            [Fact]
            public void Should_Exclude_Value()
            {
                var excludedValue = Create<DummyClass>();

                var value = CreateExcluding(excludedValue);

                excludedValue.Should().NotBeEquivalentTo(value);
            }

            [Fact]
            public void Should_Exclude_Values()
            {
                var excludedValues = CreateMany<DummyClass>().ToArray();

                var value = CreateExcluding(excludedValues);

                foreach (var excludedValue in excludedValues)
                {
                    excludedValue.Should().NotBeEquivalentTo(value);
                }
            }
        }

        public class CreateMany_ : FixtureBaseFixture
        {
            [Fact]
            public void Should_Throw_When_Creating_Less_Than_One()
            {
                Invoking(() => { CreateMany<int>(0); })
                  .Should()
                  .Throw<ArgumentOutOfRangeException>()
                  .WithMessage("count must be greater than zero (Parameter 'count')");
            }

            [Fact]
            public void Should_Create_Default_Count()
            {
                var values = CreateMany<int>();

                values.Should().HaveCount(5);
            }

            [Fact]
            public void Should_Create_Expected_Count()
            {
                var count = 1 + Create<int>() % 100;
                var values = CreateMany<int>(count);

                values.Should().HaveCount(count);
            }

            [Fact]
            public void Should_Create_Many_Numerical()
            {
                var values = CreateMany<int>();

                values.Should().HaveCount(5);
            }

            [Fact]
            public void Should_Create_Many_String()
            {
                var values = CreateMany<string>();

                values.Should().HaveCount(5);
            }

            [Fact]
            public void Should_Create_Many_Random_Enum()
            {
                var values = CreateMany<DummyEnum>();

                values.Should().AllBeOfType<DummyEnum>();

                var count = values.Count;

                var diffs = Enumerable
                  .Range(0, count - 1)
                  .Select(index => values[count - index - 1] - values[count - index - 2])
                  .Distinct()
                  .ToList();

                diffs.Should().NotHaveCount(1);
            }

            [Fact]
            public void Should_Create_Many_Class()
            {
                var values = CreateMany<DummyClass>();

                values.Should().HaveCount(5);
            }

            [Fact]
            public void Should_Create_Many_Populated_Class()
            {
                var expectedIntegers = CreateMany<int>();
                var expectedStrings = CreateMany<string>();

                var intIndex = 0;
                var strIndex = 0;

                Fixture.Register(() => expectedIntegers[intIndex++]);
                Fixture.Register(() => expectedStrings[strIndex++]);

                var values = CreateMany<DummyClass>();

                var index = 0;
                foreach (var value in values)
                {
                    value.IntValue.Should().Be(expectedIntegers[index]);
                    value.StringValue.Should().Be(expectedStrings[index]);

                    index++;
                }
            }
        }

        public class CreateManyDistinct_ : FixtureBaseFixture
        {
            [Fact]
            public void Should_Throw_When_Creating_Less_Than_One()
            {
                Invoking(() => { CreateManyDistinct<int>(0); })
                  .Should()
                  .Throw<ArgumentOutOfRangeException>()
                  .WithMessage("count must be greater than zero (Parameter 'count')");
            }

            [Fact]
            public void Should_Create_Default_Count()
            {
                var values = CreateManyDistinct<int>();

                values.Should().HaveCount(5);
            }

            [Fact]
            public void Should_Create_Expected_Count()
            {
                var count = 1 + Create<int>() % 100;
                var values = CreateManyDistinct<int>(count);

                values.Should().HaveCount(count);
            }

            [Fact]
            public void Should_Create_Many_Distinct_Numerical()
            {
                var values = CreateManyDistinct<int>();

                values.Should().OnlyHaveUniqueItems();
            }

            [Fact]
            public void Should_Create_Many_Distinct_String()
            {
                var values = CreateManyDistinct<string>();

                values.Should().OnlyHaveUniqueItems();
            }

            [Fact]
            public void Should_Create_Many_Distinct_Enum()
            {
                var values = CreateManyDistinct<DummyEnum>();

                values.Should().OnlyHaveUniqueItems();
            }

            [Fact]
            public void Should_Create_Many_Class()
            {
                var values = CreateManyDistinct<DummyClass>();

                values.Should().OnlyHaveUniqueItems();
            }
        }

        public class CreateManyExcluding_ : FixtureBaseFixture
        {
            [Fact]
            public void Should_Throw_When_Creating_Less_Than_One()
            {
                Invoking(() => { CreateManyExcluding<int>(0); })
                  .Should()
                  .Throw<ArgumentOutOfRangeException>()
                  .WithMessage("count must be greater than zero (Parameter 'count')");
            }

            [Fact]
            public void Should_Create_Default_Count()
            {
                var excludedValues = CreateMany<int>().ToArray();

                var values = CreateManyExcluding(excludes: excludedValues);

                values.Should().HaveCount(5);
            }

            [Fact]
            public void Should_Exclude_Duplicates()
            {
                var count = 1 + Create<int>() % 100;
                var excludedValues = CreateMany<int>().ToArray();

                var values = CreateManyExcluding(count, false, excludedValues);

                values.GroupBy(item => item).Should().HaveCount(count);
            }

            [Fact]
            public void Should_Create_Expected_Count()
            {
                var count = 1 + Create<int>() % 100;
                var allowDuplicates = Create<bool>();
                var excludedValues = CreateMany<int>().ToArray();

                var values = CreateManyExcluding(count, allowDuplicates, excludedValues);

                values.Should().HaveCount(count);
            }
        }

        public class GetWithinRange_Int : FixtureBaseFixture
        {
            [Fact]
            public void Should_Throw_When_Min_Greater_Than_Max()
            {
                var maxValue = Create<int>();
                var minValue = maxValue + 1;

                Invoking(() => { GetWithinRange(minValue, maxValue); })
                  .Should()
                  .Throw<ArgumentOutOfRangeException>()
                  .WithMessage("The minValue must be less than maxValue (Parameter 'minValue')");
            }

            [Fact]
            public void Should_Not_Throw_When_Min_And_Max_On_Limits()
            {
                Invoking(() => { GetWithinRange(int.MinValue, int.MaxValue); })
                  .Should()
                  .NotThrow();
            }

            [Fact]
            public void Should_Not_Throw_When_Max_Greater_Than_Min()
            {
                var maxValue = Create<int>();
                var minValue = maxValue - 1;

                Invoking(() => { GetWithinRange(minValue, maxValue); })
                  .Should()
                  .NotThrow();
            }

            [Fact]
            public void Should_Return_Within_Expected_Range()
            {
                var minValue = Create<int>();
                var maxValue = minValue + Create<int>();

                var value = GetWithinRange(minValue, maxValue);

                value.Should().BeGreaterOrEqualTo(minValue).And.BeLessOrEqualTo(maxValue);
            }
        }

        public class GetWithinRange_Double : FixtureBaseFixture
        {
            [Fact]
            public void Should_Throw_When_Min_Greater_Than_Max()
            {
                var maxValue = Create<double>();
                var minValue = maxValue + 0.1d;

                Invoking(() => { GetWithinRange(minValue, maxValue); })
                  .Should()
                  .Throw<ArgumentOutOfRangeException>()
                  .WithMessage("The minValue must be less than maxValue (Parameter 'minValue')");
            }

            [Fact]
            public void Should_Not_Throw_When_Min_And_Max_On_Limits()
            {
                Invoking(() => { GetWithinRange(double.MinValue, double.MaxValue); })
                  .Should()
                  .NotThrow();
            }

            [Fact]
            public void Should_Not_Throw_When_Max_Greater_Than_Min()
            {
                var maxValue = Create<double>();
                var minValue = maxValue - 0.1d;

                Invoking(() => { GetWithinRange(minValue, maxValue); })
                  .Should()
                  .NotThrow();
            }

            [Fact]
            public void Should_Return_Within_Expected_Range()
            {
                var minValue = Create<double>();
                var maxValue = minValue + Create<double>();

                var value = GetWithinRange(minValue, maxValue);

                value.Should().BeGreaterOrEqualTo(minValue).And.BeLessOrEqualTo(maxValue);
            }
        }

        public class GetManyWithinRange_Int : FixtureBaseFixture
        {
            [Fact]
            public void Should_Throw_When_Min_Greater_Than_Max()
            {
                var maxValue = Create<int>();
                var minValue = maxValue + 1;

                Invoking(() => { GetManyWithinRange(minValue, maxValue); })
                  .Should()
                  .Throw<ArgumentOutOfRangeException>()
                  .WithMessage("The minValue must be less than maxValue (Parameter 'minValue')");
            }

            [Fact]
            public void Should_Not_Throw_When_Min_And_Max_On_Limits()
            {
                Invoking(() => { GetManyWithinRange(int.MinValue, int.MaxValue); })
                  .Should()
                  .NotThrow();
            }

            [Fact]
            public void Should_Not_Throw_When_Max_Greater_Than_Min()
            {
                var maxValue = Create<int>();
                var minValue = maxValue - 1;

                Invoking(() => { GetManyWithinRange(minValue, maxValue); })
                  .Should()
                  .NotThrow();
            }

            [Fact]
            public void Should_Throw_When_Count_Less_Than_One()
            {
                var minValue = Create<int>();
                var count = Math.Min(0, -Create<int>());

                Invoking(() => { GetManyWithinRange(minValue, minValue + 1, count); })
                  .Should()
                  .Throw<ArgumentOutOfRangeException>()
                  .WithMessage("count must be greater than zero (Parameter 'count')");
            }

            [Fact]
            public void Should_Not_Throw_When_Count_Greater_Than_Zero()
            {
                var minValue = Create<int>();

                Invoking(() => { GetManyWithinRange(minValue, minValue + 1, 1); })
                  .Should()
                  .NotThrow();
            }

            [Fact]
            public void Should_Return_Expected_Count()
            {
                var minValue = Create<int>();
                var maxValue = minValue + Create<int>();
                var count = 1 + Create<int>() % 100;

                var values = GetManyWithinRange(minValue, maxValue, count);

                values.Should().HaveCount(count);
            }

            [Fact]
            public void Should_Return_Expected_Default_Count()
            {
                var minValue = Create<int>();
                var maxValue = minValue + Create<int>();

                var values = GetManyWithinRange(minValue, maxValue);

                values.Should().HaveCount(5);
            }

            [Fact]
            public void Should_Return_Within_Expected_Range()
            {
                var minValue = Create<int>();
                var maxValue = minValue + Create<int>();

                var values = GetManyWithinRange(minValue, maxValue).ToList();

                foreach (var value in values)
                {
                    value.Should().BeGreaterOrEqualTo(minValue).And.BeLessOrEqualTo(maxValue);
                }
            }

            [Fact]
            public void Should_Exclude_Duplicates()
            {
                var values = GetManyWithinRange(1, 10, 10, false).ToList();

                var duplicates = values.GroupBy(item => item).Any(grp => grp.Count() > 1);

                duplicates.Should().BeFalse();
            }
        }

        public class GetManyWithinRange_Double : FixtureBaseFixture
        {
            [Fact]
            public void Should_Throw_When_Min_Greater_Than_Max()
            {
                var maxValue = Create<double>();
                var minValue = maxValue + 0.1d;

                Invoking(() => { GetManyWithinRange(minValue, maxValue); })
                  .Should()
                  .Throw<ArgumentOutOfRangeException>()
                  .WithMessage("The minValue must be less than maxValue (Parameter 'minValue')");
            }

            [Fact]
            public void Should_Not_Throw_When_Min_And_Max_On_Limits()
            {
                Invoking(() => { GetManyWithinRange(double.MinValue, double.MaxValue); })
                  .Should()
                  .NotThrow();
            }

            [Fact]
            public void Should_Not_Throw_When_Max_Greater_Than_Min()
            {
                var maxValue = Create<double>();
                var minValue = maxValue - 0.1d;

                Invoking(() => { GetManyWithinRange(minValue, maxValue); })
                  .Should()
                  .NotThrow();
            }

            [Fact]
            public void Should_Throw_When_Count_Less_Than_One()
            {
                var minValue = Create<double>();
                var count = Math.Min(0, -Create<int>());

                Invoking(() => { GetManyWithinRange(minValue, minValue + 0.1d, count); })
                  .Should()
                  .Throw<ArgumentOutOfRangeException>()
                  .WithMessage("count must be greater than zero (Parameter 'count')");
            }

            [Fact]
            public void Should_Not_Throw_When_Count_Greater_Than_Zero()
            {
                var minValue = Create<double>();

                Invoking(() => { GetManyWithinRange(minValue, minValue + 0.1d, 1); })
                  .Should()
                  .NotThrow();
            }

            [Fact]
            public void Should_Return_Expected_Count()
            {
                var minValue = Create<double>();
                var maxValue = minValue + Create<double>();
                var count = 1 + Create<int>() % 100;

                var values = GetManyWithinRange(minValue, maxValue, count);

                values.Should().HaveCount(count);
            }

            [Fact]
            public void Should_Return_Expected_Default_Count()
            {
                var minValue = Create<double>();
                var maxValue = minValue + Create<double>();

                var values = GetManyWithinRange(minValue, maxValue);

                values.Should().HaveCount(5);
            }

            [Fact]
            public void Should_Return_Within_Expected_Range()
            {
                var minValue = Create<double>();
                var maxValue = minValue + Create<double>();

                var values = GetManyWithinRange(minValue, maxValue).ToList();

                foreach (var value in values)
                {
                    value.Should().BeGreaterOrEqualTo(minValue).And.BeLessOrEqualTo(maxValue);
                }
            }

            [Fact]
            public void Should_Exclude_Duplicates()
            {
                var values = GetManyWithinRange(1.0d, 2.0d, 1000, false).ToList();

                var duplicates = values.GroupBy(item => item).Any(grp => grp.Count() > 1);

                duplicates.Should().BeFalse();
            }
        }

        public class AssertDefaultConstructor_ : FixtureBaseFixture
        {
            [Fact]
            public void Should_Have_Default_Constructor()
            {
                AssertDefaultConstructor<Exception>();
            }

            [Fact]
            public void Should_Have_Known_Message()
            {
                AssertDefaultConstructor<DefaultConstructorExceptionDummy>("abc");
            }
        }

        public class AssertNoDefaultConstructor_ : FixtureBaseFixture
        {
            [Fact]
            public void Should_Not_Have_Default_Constructor()
            {
                AssertNoDefaultConstructor<NoDefaultConstructorExceptionDummy>();
            }

            [Fact]
            public void Should_Throw_When_Has_Default_Constructor()
            {
                Invoking(() =>
                {
                    AssertNoDefaultConstructor<DefaultConstructorExceptionDummy>();
                })
                .Should()
                .Throw<Xunit.Sdk.XunitException>()
                .WithMessage("Expected constructor to be <null>, but found Void .ctor().");
            }
        }

        public class AssertConstructorWithMessage_ : FixtureBaseFixture
        {
            [Fact]
            public void Should_Have_Constructor_With_Message()
            {
                AssertConstructorWithMessage<ConstructorWithMessageExceptionDummy>();
            }

            [Fact]
            public void Should_Throw_When_Not_Have_Constructor_With_Message()
            {
                Invoking(() =>
                {
                    AssertConstructorWithMessage<DefaultConstructorExceptionDummy>();
                })
                .Should()
                .Throw<Xunit.Sdk.XunitException>()
                .WithMessage("Expected constructor not to be <null>.");
            }
        }

        public class AssertNoConstructorWithMessage_ : FixtureBaseFixture
        {
            private sealed class ExceptionNoConstructorWithMessageDummy : Exception
            {
                public ExceptionNoConstructorWithMessageDummy()
                {
                }
            }

            [Fact]
            public void Should_Not_Have_Constructor_With_Message()
            {
                AssertNoConstructorWithMessage<ExceptionNoConstructorWithMessageDummy>();
            }

            [Fact]
            public void Should_Throw_When_Have_Constructor_With_Message()
            {
                Invoking(() =>
                {
                    AssertNoConstructorWithMessage<ConstructorWithMessageExceptionDummy>();
                })
                .Should()
                .Throw<Xunit.Sdk.XunitException>()
                .WithMessage("Expected constructor to be <null>, but found Void .ctor(System.String).");
            }
        }

        public class AssertConstructorWithMessageAndInnerException_ : FixtureBaseFixture
        {
            private sealed class ConstructorWithMessageAndInnerExceptionDummy : Exception
            {
                public ConstructorWithMessageAndInnerExceptionDummy(string message, Exception innerException)
                    : base(message, innerException)
                {
                }
            }

            [Fact]
            public void Should_Have_Constructor_With_Message_And_Inner_Exception()
            {
                AssertConstructorWithMessageAndInnerException<ConstructorWithMessageAndInnerExceptionDummy>();
            }

            [Fact]
            public void Should_Throw_When_Not_Have_Constructor_With_Message_And_Inner_Exception()
            {
                Invoking(() =>
                {
                    AssertConstructorWithMessageAndInnerException<DefaultConstructorExceptionDummy>();
                })
                .Should()
                .Throw<Xunit.Sdk.XunitException>()
                .WithMessage("Expected constructor not to be <null>.");
            }
        }

        public class AssertHandledAggregateException_ : FixtureBaseFixture
        {
            private readonly IList<Exception> _exceptions;

            public AssertHandledAggregateException_()
            {
                _exceptions = new Exception[]
                {
                    new InvalidOperationException(),
                    new ArgumentNullException(),
                    new ArgumentOutOfRangeException()
                };
            }

            [Fact]
            public void Should_Throw_When_Action_Null()
            {
                Action action = () =>
                {
                    AssertHandledAggregateException(null, exception => false);
                };

                action
                  .Should()
                  .Throw<ArgumentNullException>()
                  .Where(exception => exception.ParamName == "action");
            }

            [Fact]
            public void Should_Throw_When_Exception_Handler_Null()
            {
                Action action = () =>
                {
                    AssertHandledAggregateException(() => { }, null);
                };

                action
                  .Should()
                  .Throw<ArgumentNullException>()
                  .Where(exception => exception.ParamName == "exceptionHandler");
            }

            [Fact]
            public void Should_Throw_When_No_Exception()
            {
                Action action = () =>
                {
                    AssertHandledAggregateException(
              () => { },
              exception => false);
                };

                action
                  .Should()
                  .Throw<AggregateAssertionException>()
                  .Where(exception => exception.Message == "Expected an AggregateException but nothing was thrown");
            }

            [Fact]
            public void Should_Assert_All_Exceptions_Handled()
            {
                Action action = () =>
                {
                    AssertHandledAggregateException(
              () => throw new AggregateException(_exceptions),
              exception => exception is InvalidOperationException ||
                           exception is ArgumentNullException ||
                           exception is ArgumentOutOfRangeException);
                };

                action
                  .Should()
                  .NotThrow();
            }

            [Fact]
            public void Should_Assert_Unhandled_Exception()
            {
                Action action = () =>
                {
                    AssertHandledAggregateException(
              () =>
              {
                  var exceptions = new[] { _exceptions[0], _exceptions[2] };
                  throw new AggregateException(exceptions);
              },
              exception => exception is InvalidOperationException);
                };

                action
                  .Should()
                  .Throw<AggregateAssertionException>()
                  .Which
                  .UnhandledException.InnerExceptions.Single()
                  .Should()
                  .BeOfType<ArgumentOutOfRangeException>();
            }

            [Fact]
            public void Should_Assert_Unhandled_Exceptions()
            {
                Action action = () =>
                {
                    AssertHandledAggregateException(
              () => throw new AggregateException(_exceptions),
              exception => exception is ArgumentNullException);
                };

                action
                  .Should()
                  .Throw<AggregateAssertionException>()
                  .Where(exception => exception.Message == $"There were unhandled exceptions: {typeof(InvalidOperationException)}, {typeof(ArgumentOutOfRangeException)}");
            }

            [Fact]
            public void Should_Throw_Expected_AggregationException()
            {
                Action action = () =>
                {
                    AssertHandledAggregateException(
              () => throw new ArgumentOutOfRangeException(),
              exception => false);
                };

                action
                  .Should()
                  .Throw<AggregateAssertionException>()
                  .Where(exception => exception.Message == $"Expected an AggregateException but a {typeof(ArgumentOutOfRangeException)} was thrown");
            }
        }
    }
}