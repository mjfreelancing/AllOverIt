﻿using AllOverIt.Evaluator.Variables;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace AllOverIt.Evaluator.Tests.Variables
{
    public class LazyVariableFixture : FixtureBase
    {

        private readonly string _name;
        private double _value;
        private readonly IEnumerable<string> _referencedVariableNames;
        private readonly bool _threadSafe;
        private LazyVariable _variable;

        public LazyVariableFixture()
        {
            _name = Create<string>();
            _value = Create<double>();
            _referencedVariableNames = CreateMany<string>();
            _threadSafe = Create<bool>();
            _variable = new LazyVariable(_name, () => _value, _referencedVariableNames, _threadSafe);
        }

        public class Constructor : LazyVariableFixture
        {
            [Fact]
            public void Should_Throw_When_Name_Null()
            {
                Invoking(() => _variable = new LazyVariable(null, () => _value, null, Create<bool>()))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("name");
            }

            [Fact]
            public void Should_Throw_When_Name_Empty()
            {
                Invoking(() => _variable = new LazyVariable(string.Empty, () => _value, null, Create<bool>()))
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("name");
            }

            [Fact]
            public void Should_Throw_When_Name_Whitespace()
            {
                Invoking(() => _variable = new LazyVariable(" ", () => _value, null, Create<bool>()))
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("name");
            }

            [Fact]
            public void Should_Throw_When_Func_Null()
            {
                Invoking(() => _variable = new LazyVariable(Create<string>(), null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("valueResolver");
            }

            [Fact]
            public void Should_Not_Throw_When_ReferencedVariableNames_Null()
            {
                Invoking(() => _variable = new LazyVariable(Create<string>(), () => _value, null))
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
                    ThreadSafe = _threadSafe,
                    ReferencedVariables = default(IEnumerable<string>)
                }, option => option.Excluding(prop => prop.ReferencedVariables));
            }

            [Fact]
            public void Should_Return_Empty_ReferencedVariables()
            {
                _variable = new LazyVariable(Create<string>(), () => _value, null, _threadSafe)
                {
                    VariableRegistry = new VariableRegistry()
                };

                _variable.ReferencedVariables.Should().BeEmpty();
            }

            [Fact]
            public void Should_Throw_If_No_VariableRegistry_When_Get_ReferencedVariables()
            {
                _variable = new LazyVariable(Create<string>(), () => _value, null, _threadSafe);

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

                _variable = new LazyVariable(Create<string>(), () => _value, _referencedVariableNames, _threadSafe)
                {
                    VariableRegistry = variableRegistry
                };

                var referencedVariables = _variable.ReferencedVariables;

                referencedVariables.Should().BeEquivalentTo(variables);
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

                invoked.Should().BeTrue();
                actual.Should().Be(_value + 1);
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

                actual1.Should().Equals(actual2);
                count.Should().Be(1);
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

                _variable.Value.Should().Be(_value);

                _variable.Reset();

                _value = expected;

                _variable.Value.Should().Be(expected);
            }
        }
    }
}