using AllOverIt.Evaluator.Variables;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;

namespace AllOverIt.Evaluator.Tests.Variables
{
    public class DelegateVariableFixture : FixtureBase
    {
        private readonly string _name;
        private readonly double _value;
        private DelegateVariable _variable;

        public DelegateVariableFixture()
        {
            _name = Create<string>();
            _value = Create<double>();
            _variable = new DelegateVariable(_name, () => _value);
        }

        public class Constructor : DelegateVariableFixture
        {
            [Fact]
            public void Should_Throw_When_Name_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue => _variable = new DelegateVariable(stringValue, () => _value),
                    "name");
            }

            [Fact]
            public void Should_Throw_When_Func_Null()
            {
                Invoking(() => _variable = new DelegateVariable(Create<string>(), (Func<double>) null))
                    .Should()
                    .Throw<ArgumentNullException>()
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
                    ReferencedVariables = default(IEnumerable<string>)
                };

                expected.Should().BeEquivalentTo(_variable, option => option.Excluding(subject => subject.ReferencedVariables));
            }

            [Fact]
            public void Should_Invoke_Value()
            {
                var invoked = false;

                _variable = new DelegateVariable(_name, () =>
                {
                    invoked = true;
                    return _value + 1;
                });

                var actual = _variable.Value;

                invoked.Should().BeTrue();
                actual.Should().Be(_value + 1);
            }
        }
    }
}
