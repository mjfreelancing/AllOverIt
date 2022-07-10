using AllOverIt.Evaluator.Exceptions;
using AllOverIt.Evaluator.Tests.Variables.Dummies;
using AllOverIt.Evaluator.Variables;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FakeItEasy;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace AllOverIt.Evaluator.Tests.Variables
{
    public class VariableRegistryFixture : FixtureBase
    {
        private VariableRegistry _registry;

        public VariableRegistryFixture()
        {
            _registry = new VariableRegistry();
        }

        public class AddVariable : VariableRegistryFixture
        {
            [Fact]
            public void Should_Throw_When_Variable_Null()
            {
                Invoking(() => _registry.AddVariable(null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("variable");
            }

            [Fact]
            public void Should_Throw_When_Variable_Registered()
            {
                const string name = "xyz";

                var variable = new Fake<IVariable>();
                variable.CallsTo(fake => fake.Name).Returns(name);

                _registry = new VariableRegistry();
                _registry.AddVariable(variable.FakedObject);

                Invoking(() => _registry.AddVariable(variable.FakedObject))
                    .Should()
                    .Throw<VariableException>()
                    .WithMessage("The variable 'xyz' is already registered.");
            }

            [Fact]
            public void Should_Add_Variable()
            {
                var name = Create<string>();

                var variable = new Fake<IVariable>();
                variable.CallsTo(fake => fake.Name).Returns(name);

                _registry = new VariableRegistry();
                _registry.AddVariable(variable.FakedObject);

                var expected = new Dictionary<string, IVariable> { [name] = variable.FakedObject };

                expected.Should().BeEquivalentTo(_registry.Variables);
            }

            [Fact]
            public void Should_Assign_VariableRegistry_To_Variable()
            {
                var variable = new VariableBaseDummy(Create<string>());

                _registry.AddVariable(variable);

                variable.VariableRegistry.Should().BeSameAs(_registry);
            }
        }

        public class GetValue : VariableRegistryFixture
        {
            [Fact]
            public void Should_Throw_When_Name_Null()
            {
                Invoking(() => _registry.GetValue(null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("name");
            }

            [Fact]
            public void Should_Throw_When_Name_Empty()
            {
                Invoking(() => _registry.GetValue(string.Empty))
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("name");
            }

            [Fact]
            public void Should_Throw_When_Name_Whitespace()
            {
                Invoking(() => _registry.GetValue("  "))
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("name");
            }

            [Fact]
            public void Should_Throw_When_Not_Registered()
            {
                var name = Create<string>();

                Invoking(() => _registry.GetValue(name))
                    .Should()
                    .Throw<VariableException>()
                    .WithMessage($"The variable '{name}' is not registered");
            }

            [Fact]
            public void Should_Return_Value()
            {
                var name = Create<string>();
                var value = Create<double>();

                var expected = new Fake<IVariable>();
                expected.CallsTo(fake => fake.Name).Returns(name);
                expected.CallsTo(fake => fake.Value).Returns(value);

                _registry.AddVariable(expected.FakedObject);

                var actual = _registry.GetValue(name);

                actual.Should().Be(value);
            }
        }

        public class SetValue : VariableRegistryFixture
        {
            [Fact]
            public void Should_Throw_When_Name_Null()
            {
                Invoking(() => _registry.SetValue(null, Create<double>()))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("name");
            }

            [Fact]
            public void Should_Throw_When_Name_Empty()
            {
                Invoking(() => _registry.SetValue(string.Empty, Create<double>()))
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("name");
            }

            [Fact]
            public void Should_Throw_When_Name_Whitespace()
            {
                Invoking(() => _registry.SetValue("  ", Create<double>()))
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("name");
            }

            [Fact]
            public void Should_Throw_When_Not_Registered()
            {
                var name = Create<string>();

                Invoking(() => _registry.SetValue(name, Create<double>()))
                    .Should()
                    .Throw<VariableException>()
                    .WithMessage($"The variable '{name}' is not registered");
            }

            [Fact]
            public void Should_Throw_When_Not_Mutable()
            {
                var name = Create<string>();
                var variable = new ConstantVariable(name);

                _registry = new VariableRegistry();
                _registry.AddVariable(variable);

                Invoking(() => _registry.SetValue(name, Create<double>()))
                    .Should()
                    .Throw<VariableImmutableException>()
                    .WithMessage($"The variable '{name}' is not mutable");
            }

            [Fact]
            public void Should_Set_Value()
            {
                var name = Create<string>();
                var value = Create<double>();
                var variable = new MutableVariable(name);

                _registry.AddVariable(variable);

                _registry.SetValue(name, value);

                variable.Value.Should().Be(value);
            }
        }
    }
}
