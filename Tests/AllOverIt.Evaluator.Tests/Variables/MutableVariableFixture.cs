using AllOverIt.Evaluator.Variables;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace AllOverIt.Evaluator.Tests.Variables
{
    public class MutableVariableFixture : AoiFixtureBase
    {
        private readonly string _name;
        private readonly double _value;
        private readonly IEnumerable<string> _referencedVariableNames;
        private MutableVariable _variable;

        public MutableVariableFixture()
        {
            _name = Create<string>();
            _value = Create<double>();
            _referencedVariableNames = CreateMany<string>();
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
            public void Should_Not_Throw_When_ReferencedVariableNames_Null()
            {
                Invoking(() => _variable = new MutableVariable(Create<string>(), Create<double>(), null))
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
                    VariableRegistry = default(IVariableRegistry),
                    ReferencedVariables = default(IEnumerable<string>)
                }, option => option.Excluding(prop => prop.ReferencedVariables));
            }

            [Fact]
            public void Should_Set_Default_Value()
            {
                var name = Create<string>();

                _variable = new MutableVariable(name);

                _variable.Should().BeEquivalentTo(new
                {
                    Name = name,
                    Value = default(double),
                    VariableRegistry = default(IVariableRegistry),
                    ReferencedVariables = default(IEnumerable<string>)
                }, option => option.Excluding(prop => prop.ReferencedVariables));
            }

            [Fact]
            public void Should_Return_Empty_ReferencedVariables()
            {
                _variable = new MutableVariable(Create<string>(), Create<double>(), null)
                {
                    VariableRegistry = new VariableRegistry()
                };

                _variable.ReferencedVariables.Should().BeEmpty();
            }

            [Fact]
            public void Should_Throw_If_No_VariableRegistry_When_Get_ReferencedVariables()
            {
                _variable = new MutableVariable(Create<string>(), Create<double>(), null);

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

                _variable = new MutableVariable(Create<string>(), Create<double>(), _referencedVariableNames)
                {
                    VariableRegistry = variableRegistry
                };

                var referencedVariables = _variable.ReferencedVariables;

                referencedVariables.Should().BeEquivalentTo(variables);
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
