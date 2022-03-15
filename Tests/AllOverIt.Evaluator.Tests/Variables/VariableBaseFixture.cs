using AllOverIt.Evaluator.Tests.Variables.Dummies;
using AllOverIt.Evaluator.Variables;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace AllOverIt.Evaluator.Tests.Variables
{
    public class VariableBaseFixture : FixtureBase
    {
        private readonly string _name;
        private readonly double _value;
        private VariableBaseDummy _variable;

        public VariableBaseFixture()
        {
            _name = Create<string>();
            _value = Create<double>();
            _variable = new VariableBaseDummy(_name, _value);
        }

        public class Constructor : VariableBaseFixture
        {
            [Fact]
            public void Should_Throw_When_Name_Null()
            {
                Invoking(() => _variable = new VariableBaseDummy(null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("name");
            }

            [Fact]
            public void Should_Throw_When_Name_Empty()
            {
                Invoking(() => _variable = new VariableBaseDummy(string.Empty))
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("name");
            }

            [Fact]
            public void Should_Throw_When_Name_Whitespace()
            {
                Invoking(() => _variable = new VariableBaseDummy("  "))
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
                    ReferencedVariables = Enumerable.Empty<string>(),
                    VariableRegistry = default(IVariableRegistry)
                };

                expected.Should().BeEquivalentTo(_variable);
            }

            [Fact]
            public void Should_Set_Default_Value()
            {
                var name = Create<string>();

                _variable = new VariableBaseDummy(name);

                var expected = new
                {
                    Name = name,
                    Value = default(double),
                    ReferencedVariables = Enumerable.Empty<string>(),
                    VariableRegistry = default(IVariableRegistry)
                };

                expected.Should().BeEquivalentTo(_variable);
            }
        }

        public class GetReferencedVariables : VariableBaseFixture
        {
            [Fact]
            public void Should_Return_Empty_ReferencedVariables()
            {
                _variable = new VariableBaseDummy(Create<string>(), Create<double>())
                {
                    VariableRegistry = new VariableRegistry()
                };

                _variable.ReferencedVariables.Should().BeEmpty();
            }
        }

        public class SetValue : VariableBaseFixture
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
