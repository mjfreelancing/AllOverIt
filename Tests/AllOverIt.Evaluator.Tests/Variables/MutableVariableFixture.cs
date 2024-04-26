using AllOverIt.Evaluator.Variables;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;

namespace AllOverIt.Evaluator.Tests.Variables
{
    public class MutableVariableFixture : FixtureBase
    {
        private readonly string _name;
        private readonly double _value;
        private MutableVariable _variable;

        public MutableVariableFixture()
        {
            _name = Create<string>();
            _value = Create<double>();
            _variable = new MutableVariable(_name, _value);
        }

        public class Constructor : MutableVariableFixture
        {
            [Fact]
            public void Should_Throw_When_Name_Null()
            {
                Invoking(() => _variable = new MutableVariable(null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("name");
            }

            [Fact]
            public void Should_Throw_When_Name_Empty()
            {
                Invoking(() => _variable = new MutableVariable(string.Empty))
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("name");
            }

            [Fact]
            public void Should_Throw_When_Name_Whitespace()
            {
                Invoking(() => _variable = new MutableVariable("  "))
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("name");
            }

            [Fact]
            public void Should_Set_Members()
            {
                var expected = new
                {
                    Name = _name,
                    Value = _value,
                    VariableRegistry = default(IVariableRegistry),
                    ReferencedVariables = Enumerable.Empty<string>()
                };

                expected.Should().BeEquivalentTo(_variable);
            }

            [Fact]
            public void Should_Set_Default_Value()
            {
                var name = Create<string>();

                _variable = new MutableVariable(name);

                var expected = new
                {
                    Name = name,
                    Value = default(double),
                    VariableRegistry = default(IVariableRegistry),
                    ReferencedVariables = Enumerable.Empty<string>()
                };

                expected.Should().BeEquivalentTo(_variable);
            }
        }

        public class SetValue : MutableVariableFixture
        {
            [Fact]
            public void Should_Set_Value()
            {
                var value = CreateExcluding(_value);

                _variable.SetValue(value);

                _variable.Value.Should().Be(value);
            }
        }
    }
}
