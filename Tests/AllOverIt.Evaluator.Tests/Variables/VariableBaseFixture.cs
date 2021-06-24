using AllOverIt.Evaluator.Tests.Variables.Dummies;
using AllOverIt.Evaluator.Variables;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AllOverIt.Evaluator.Tests.Variables
{
    public class VariableBaseFixture : AoiFixtureBase
    {
        private readonly string _name;
        private readonly double _value;
        private readonly IEnumerable<string> _referencedVariableNames;
        private VariableBaseDummy _variable;

        public VariableBaseFixture()
        {
            _name = Create<string>();
            _value = Create<double>();
            _referencedVariableNames = CreateMany<string>();
            _variable = new VariableBaseDummy(_name, _value, _referencedVariableNames);
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
            public void Should_Not_Throw_When_ReferencedVariableNames_Null()
            {
                Invoking(() => _variable = new VariableBaseDummy(Create<string>(), Create<double>(), null))
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Set_Members()
            {
                _variable.Should().BeEquivalentTo(new
                {
                    Name = _name,
                    Value = _value,
                    ReferencedVariables = _referencedVariableNames,
                    VariableRegistry = default(IVariableRegistry)
                }, option => option.Excluding(prop => prop.ReferencedVariables));
            }

            [Fact]
            public void Should_Set_Default_Value()
            {
                var name = Create<string>();

                _variable = new VariableBaseDummy(name);

                _variable.Should().BeEquivalentTo(new
                {
                    Name = name,
                    Value = default(double),
                    ReferencedVariables = Enumerable.Empty<string>(),
                    VariableRegistry = default(IVariableRegistry)
                }, option => option.Excluding(prop => prop.ReferencedVariables));
            }
        }

        public class GetReferencedVariables : VariableBaseFixture
        {
            [Fact]
            public void Should_Return_Empty_ReferencedVariables()
            {
                _variable = new VariableBaseDummy(Create<string>(), Create<double>(), null)
                {
                    VariableRegistry = new VariableRegistry()
                };

                _variable.ReferencedVariables.Should().BeEmpty();
            }

            [Fact]
            public void Should_Throw_If_No_VariableRegistry_When_Get_ReferencedVariables()
            {
                _variable = new VariableBaseDummy(Create<string>(), Create<double>(), null);

                Invoking(() => { _ = _variable.ReferencedVariables; })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("variableRegistry");
            }

            [Fact]
            public void Should_Resolve_Variables()
            {
                var variables = new List<IVariable>();
                var variableRegistry = new VariableRegistry();

                foreach (var name in _referencedVariableNames)
                {
                    var variable = new ConstantVariable(name);
                    variables.Add(variable);

                    variableRegistry.AddVariable(variable);
                }

                _variable = new VariableBaseDummy(Create<string>(), Create<double>(), _referencedVariableNames)
                {
                    VariableRegistry = variableRegistry
                };

                var referencedVariables = _variable.ReferencedVariables;

                referencedVariables.Should().BeEquivalentTo(variables);
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
