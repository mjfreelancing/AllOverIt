using AllOverIt.Fixture.Exceptions;
using AllOverIt.Fixture.Tests.Dummies;
using AutoFixture;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AllOverIt.Fixture.Tests
{
    public class FixtureBaseFixture : FixtureBase
    {
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
                Action action = () => Invoking((Func<bool>)null);

                action
                  .Should()
                  .Throw<ArgumentNullException>();
            }

            [Fact]
            public void Should_Return_Same_Action()
            {
                Func<string> toInvoke = () => Create<string>();

                var invoked = Invoking(toInvoke);

                invoked.Should().BeSameAs(toInvoke);
            }
        }

        public class Awaiting_Task : FixtureBaseFixture
        {
            [Fact]
            public void Should_Throw_When_Action_Null()
            {
                Action action = () => Awaiting((Func<Task>)null);

                action
                  .Should()
                  .Throw<ArgumentNullException>();
            }

            [Fact]
            public void Should_Return_Same_Action()
            {
                Func<Task> toInvoke = () => Task.FromResult(true);

                var invoked = Awaiting(toInvoke);

                invoked.Should().BeSameAs(toInvoke);
            }
        }

        public class Awaiting_Task_Result : FixtureBaseFixture
        {
            [Fact]
            public void Should_Throw_When_Action_Null()
            {
                Action action = () => Awaiting((Func<Task<bool>>)null);

                action
                  .Should()
                  .Throw<ArgumentNullException>();
            }

            [Fact]
            public void Should_Return_Same_Action()
            {
                Func<Task<bool>> toInvoke = () => Task.FromResult(true);

                var invoked = Awaiting(toInvoke);

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

                Func<DummyClass> creator = () =>
                {
                    creatorCalled = true;
                    return new DummyClass();
                };

                Register(creator);

                Create<DummyClass>();

                creatorCalled.Should().BeTrue();
            }

            [Fact]
            public void Should_Create_DummyClassThreeArgs()
            {
                Func<DummyClass> creator = () => new DummyClass();

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

                Func<int, DummyClassOneArg> creator = arg1 =>
                {
                    creatorCalled = true;
                    return new DummyClassOneArg(arg1);
                };

                Register(creator);

                Create<DummyClassOneArg>();

                creatorCalled.Should().BeTrue();
            }

            [Fact]
            public void Should_Create_DummyClassThreeArgs()
            {
                Func<int, DummyClassOneArg> creator = arg1 => new DummyClassOneArg(arg1);

                Register(creator);

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

                Func<int, double, DummyClassTwoArgs> creator = (arg1, arg2) =>
                {
                    creatorCalled = true;
                    return new DummyClassTwoArgs(arg1, arg2);
                };

                Register(creator);

                Create<DummyClassTwoArgs>();

                creatorCalled.Should().BeTrue();
            }

            [Fact]
            public void Should_Create_DummyClassThreeArgs()
            {
                Func<int, double, DummyClassTwoArgs> creator = (arg1, arg2) => new DummyClassTwoArgs(arg1, arg2);

                Register(creator);

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

                Func<int, double, DummyEnum, DummyClassThreeArgs> creator = (arg1, arg2, arg3) =>
                {
                    creatorCalled = true;
                    return new DummyClassThreeArgs(arg1, arg2, arg3);
                };

                Register(creator);

                Create<DummyClassThreeArgs>();

                creatorCalled.Should().BeTrue();
            }

            [Fact]
            public void Should_Create_DummyClassThreeArgs()
            {
                Func<int, double, DummyEnum, DummyClassThreeArgs> creator =
                  (arg1, arg2, arg3) => new DummyClassThreeArgs(arg1, arg2, arg3);

                Register(creator);

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

                Func<int, double, DummyEnum, float, DummyClassFourArgs> creator = (arg1, arg2, arg3, arg4) =>
                {
                    creatorCalled = true;
                    return new DummyClassFourArgs(arg1, arg2, arg3, arg4);
                };

                Register(creator);

                Create<DummyClassFourArgs>();

                creatorCalled.Should().BeTrue();
            }

            [Fact]
            public void Should_Create_DummyClassFourArgs()
            {
                Func<int, double, DummyEnum, float, DummyClassFourArgs> creator =
                  (arg1, arg2, arg3, arg4) => new DummyClassFourArgs(arg1, arg2, arg3, arg4);

                Register(creator);

                var actual = Create<DummyClassFourArgs>();

                actual.Should().BeOfType<DummyClassFourArgs>();
            }
        }

        public class Create_ : FixtureBaseFixture
        {
            [Fact]
            public void Should_Create_String()
            {
                var value = Create<string>();

                value.Should().NotBeNullOrEmpty();
            }

            [Fact(Timeout = 1000)]
            public void Should_Create_Random_Enum()
            {
                var value1 = Create<DummyEnum>();
                var value2 = Create<DummyEnum>();

                while (value2 - value1 == 1)
                {
                    value2 = Create<DummyEnum>();
                }

                value1.Should().BeOfType<DummyEnum>();
                (value2 - value1).Should().NotBe(1);
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

                value.Should().NotBeEquivalentTo(excludedValue);
            }

            [Fact]
            public void Should_Exclude_Values()
            {
                var excludedValues = CreateMany<DummyClass>().ToArray();

                var value = CreateExcluding(excludedValues);

                foreach (var excludedValue in excludedValues)
                {
                    value.Should().NotBeEquivalentTo(excludedValue);
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
                IReadOnlyList<int> values = CreateMany<int>();

                values.Should().HaveCount(5);
            }

            [Fact]
            public void Should_Create_Many_String()
            {
                IReadOnlyList<string> values = CreateMany<string>();

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
                IReadOnlyList<DummyClass> values = CreateMany<DummyClass>();

                values.Should().HaveCount(5);
            }

            [Fact]
            public void Should_Create_Many_Populated_Class()
            {
                var expectedInts = CreateMany<int>();
                var expectedStrings = CreateMany<string>();

                var intIndex = 0;
                var strIndex = 0;

                Fixture.Register(() => expectedInts[intIndex++]);
                Fixture.Register(() => expectedStrings[strIndex++]);

                var values = CreateMany<DummyClass>();

                var index = 0;
                foreach (var value in values)
                {
                    value.IntValue.Should().Be(expectedInts[index]);
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