using AllOverIt.Evaluator.Variables;
using AllOverIt.Fixture;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace AllOverIt.Evaluator.Tests.Variables
{
    public class AoiConstantVariableFixture : AoiFixtureBase
    {
        private readonly string _name;
        private readonly double _value;
        private readonly IEnumerable<string> _referencedVariableNames;
        private AoiConstantVariable _variable;

        public AoiConstantVariableFixture()
        {
            _name = Create<string>();
            _value = Create<double>();
            _referencedVariableNames = CreateMany<string>();
            _variable = new AoiConstantVariable(_name, _value);
        }

        public class Constructor : AoiConstantVariableFixture
        {
            [Fact]
            public void Should_Throw_When_Name_Null()
            {
                Invoking(() => _variable = new AoiConstantVariable(null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithMessage(GetExpectedArgumentNullExceptionMessage("name"));
            }

            [Fact]
            public void Should_Throw_When_Name_Empty()
            {
                Invoking(() => _variable = new AoiConstantVariable(string.Empty))
                    .Should()
                    .Throw<ArgumentException>()
                    .WithMessage(GetExpectedArgumentCannotBeEmptyExceptionMessage("name"));
            }

            [Fact]
            public void Should_Throw_When_Name_Whitespace()
            {
                Invoking(() => _variable = new AoiConstantVariable("  "))
                    .Should()
                    .Throw<ArgumentException>()
                    .WithMessage(GetExpectedArgumentCannotBeEmptyExceptionMessage("name"));
            }

            [Fact]
            public void Should_Not_Throw_When_ReferencedVariableNames_Null()
            {
                Invoking(() => _variable = new AoiConstantVariable(Create<string>(), Create<double>(), null))
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
                    VariableRegistry = default(IAoiVariableRegistry),
                    ReferencedVariables = default(IEnumerable<string>)
                }, option => option.Excluding(prop => prop.ReferencedVariables));
            }

            [Fact]
            public void Should_Set_Default_Value()
            {
                var name = Create<string>();

                _variable = new AoiConstantVariable(name);

                _variable.Should().BeEquivalentTo(new
                {
                    Name = name,
                    Value = default(double),
                    VariableRegistry = default(IAoiVariableRegistry),
                    ReferencedVariables = default(IEnumerable<string>)
                }, option => option.Excluding(prop => prop.ReferencedVariables));
            }

            [Fact]
            public void Should_Return_Empty_ReferencedVariables()
            {
                _variable = new AoiConstantVariable(Create<string>(), Create<double>(), null)
                {
                    VariableRegistry = new AoiVariableRegistry()
                };

                _variable.ReferencedVariables.Should().BeEmpty();

            }

            [Fact]
            public void Should_Throw_If_No_VariableRegistry_When_Get_ReferencedVariables()
            {
                _variable = new AoiConstantVariable(Create<string>(), Create<double>(), null);

                Invoking(() => { _ = _variable.ReferencedVariables; })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithMessage(GetExpectedArgumentNullExceptionMessage("variableRegistry"));
            }

            [Fact]
            public void Should_Resolve_Variables()
            {
                var variables = new List<IAoiVariable>();
                var variableRegistry = new AoiVariableRegistry();

                foreach (var name in _referencedVariableNames)
                {
                    var variable = new AoiConstantVariable(name);
                    variables.Add(variable);

                    variableRegistry.AddVariable(variable);
                }

                _variable = new AoiConstantVariable(Create<string>(), Create<double>(), _referencedVariableNames)
                {
                    VariableRegistry = variableRegistry
                };

                var referencedVariables = _variable.ReferencedVariables;

                referencedVariables.Should().BeEquivalentTo(variables);
            }
        }
    }
}
