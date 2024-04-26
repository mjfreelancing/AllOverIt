using AllOverIt.Evaluator.Variables;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;

namespace AllOverIt.Evaluator.Tests.Variables
{
    public class ConstantVariableFixture : FixtureBase
    {
        private readonly string _name;
        private readonly double _value;
        private ConstantVariable _variable;

        public ConstantVariableFixture()
        {
            _name = Create<string>();
            _value = Create<double>();
            _variable = new ConstantVariable(_name, _value);
        }

        public class Constructor : ConstantVariableFixture
        {
            [Fact]
            public void Should_Throw_When_Name_Null()
            {
                Invoking(() => _variable = new ConstantVariable(null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("name");
            }

            [Fact]
            public void Should_Throw_When_Name_Empty()
            {
                Invoking(() => _variable = new ConstantVariable(string.Empty))
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("name");
            }

            [Fact]
            public void Should_Throw_When_Name_Whitespace()
            {
                Invoking(() => _variable = new ConstantVariable("  "))
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
                    ReferencedVariables = default(IEnumerable<string>)
                };

                expected.Should().BeEquivalentTo(_variable, option => option.Excluding(subject => subject.ReferencedVariables));
            }

            [Fact]
            public void Should_Set_Default_Value()
            {
                var name = Create<string>();

                _variable = new ConstantVariable(name);

                var expected = new
                {
                    Name = name,
                    Value = default(double),
                    VariableRegistry = default(IVariableRegistry),
                    ReferencedVariables = default(IEnumerable<string>)
                };

                expected.Should().BeEquivalentTo(_variable, option => option.Excluding(subject => subject.ReferencedVariables));
            }
        }
    }
}
