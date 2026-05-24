using AllOverIt.Evaluator.Variables;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Shouldly.Extensions;
using Shouldly;

namespace AllOverIt.Evaluator.Tests.Variables
{
    public class LazyVariableFixture : FixtureBase
    {

        private readonly string _name;
        private double _value;
        private readonly bool _threadSafe;
        private LazyVariable _variable;

        public LazyVariableFixture()
        {
            _name = Create<string>();
            _value = Create<double>();
            _threadSafe = Create<bool>();
            _variable = new LazyVariable(_name, () => _value, _threadSafe);
        }

        public class Constructor : LazyVariableFixture
        {
            [Fact]
            public void Should_Throw_When_Name_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue => _variable = new LazyVariable(stringValue, () => _value, Create<bool>()),
                    "name");
            }

            [Fact]
            public void Should_Throw_When_Func_Null()
            {
                Should.Throw<ArgumentNullException>(() => _variable = new LazyVariable(Create<string>(), (Func<double>)null))
                    .WithNamedMessageWhenNull("valueResolver");
            }

            [Fact]
            public void Should_Set_Members()
            {
                var expected = new
                {
                    Name = _name,
                    Value = _value,
                    VariableRegistry = default(IVariableRegistry),
                    //ThreadSafe = _threadSafe,
                    ReferencedVariables = Enumerable.Empty<string>()
                };

                expected.ShouldBeEquivalentTo(_variable);
            }

            [Fact]
            public void Should_Invoke_Value()
            {
                var invoked = false;

                _variable = new LazyVariable(_name, () =>
                {
                    invoked = true;
                    return _value + 1;
                });

                var actual = _variable.Value;

                invoked.ShouldBeTrue();
                actual.ShouldBe(_value + 1);
            }

            [Fact]
            public void Should_Invoke_Once()
            {
                var count = 0;

                _variable = new LazyVariable(_name, () =>
                {
                    count++;
                    return _value;
                });

                var actual1 = _variable.Value;
                var actual2 = _variable.Value;

                actual1.ShouldBe(actual2);
                count.ShouldBe(1);
            }
        }

        public class Reset : LazyVariableFixture
        {
            [Fact]
            public void Should_Reset_Value()
            {
                double GetValue() => _value;
                var expected = CreateExcluding(_value);

                _variable = new LazyVariable(_name, GetValue);

                _variable.Value.ShouldBe(_value);

                _variable.Reset();

                _value = expected;

                _variable.Value.ShouldBe(expected);
            }
        }
    }
}
