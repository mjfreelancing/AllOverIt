using AllOverIt.Evaluator.Exceptions;
using AllOverIt.Evaluator.Tests.Variables.Dummies;
using AllOverIt.Evaluator.Variables;
using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using FakeItEasy;
using FluentAssertions;
using System;
using System.Linq;
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

                _registry.AddVariable(variable.FakedObject);

                _registry.TryGetVariable(name, out var actual).Should().BeTrue();

                actual.Should().BeSameAs(variable.FakedObject);
            }

            [Fact]
            public void Should_Assign_VariableRegistry_To_Variable()
            {
                var variable = new VariableBaseDummy(Create<string>());

                _registry.AddVariable(variable);

                variable.VariableRegistry.Should().BeSameAs(_registry);
            }
        }

        public class AddVariables : VariableRegistryFixture
        {
            [Fact]
            public void Should_Throw_When_Variables_Null()
            {
                Invoking(() =>
                {
                    _registry.AddVariables(null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("variables");
            }

            [Fact]
            public void Should_Add_Variable()
            {
                var names = CreateMany<string>();

                var variables = names
                    .Select(name =>
                    {
                        var variable = new Fake<IVariable>();
                        variable.CallsTo(fake => fake.Name).Returns(name);

                        return variable.FakedObject;
                    })
                    .ToArray();

                _registry.AddVariables(variables);

                foreach (var (name, index) in names.WithIndex())
                {
                    _registry.TryGetVariable(name, out var actual).Should().BeTrue();

                    actual.Should().BeSameAs(variables[index]);
                }
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

        public class Clear : VariableRegistryFixture
        {
            [Fact]
            public void Should_Clear_Registry()
            {
                var variable = this.CreateStub<IVariable>();

                _registry.AddVariable(variable);

                _registry.Should().HaveCount(1);

                _registry.Clear();

                _registry.Should().HaveCount(0);
            }
        }

        public class TryGetVariable : VariableRegistryFixture
        {
            [Fact]
            public void Should_Throw_When_Name_Null()
            {
                Invoking(() => _registry.TryGetVariable(null, out var variable))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("name");
            }

            [Fact]
            public void Should_Try_Get_Variable()
            {
                var name = Create<string>();

                var variable = new Fake<IVariable>();
                variable.CallsTo(fake => fake.Name).Returns(name);

                _registry.AddVariable(variable.FakedObject);

                _registry.TryGetVariable(name, out var actual).Should().BeTrue();

                actual.Should().BeSameAs(variable.FakedObject);
            }

            [Fact]
            public void Should_Not_Try_Get_Variable()
            {
                _registry.TryGetVariable(Create<string>(), out var actual).Should().BeFalse();
            }
        }

        public class GetVariable : VariableRegistryFixture
        {
            [Fact]
            public void Should_Throw_When_Name_Null()
            {
                Invoking(() => _registry.GetVariable(null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("name");
            }

            [Fact]
            public void Should_Try_Get_Variable()
            {
                var name = Create<string>();

                var variable = new Fake<IVariable>();
                variable.CallsTo(fake => fake.Name).Returns(name);

                _registry.AddVariable(variable.FakedObject);

                var actual = _registry.GetVariable(name);

                actual.Should().BeSameAs(variable.FakedObject);
            }

            [Fact]
            public void Should_Throw_When_Not_Get_Variable()
            {
                var name = Create<string>();

                Invoking(() =>
                {
                    _ = _registry.GetVariable(name);
                })
                .Should()
                .Throw<VariableException>()
                .WithMessage($"The variable '{name}' is not registered");
            }
        }

        public class ContainsVariable : VariableRegistryFixture
        {
            [Fact]
            public void Should_Throw_When_Name_Null()
            {
                Invoking(() => _registry.ContainsVariable(null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("name");
            }

            [Fact]
            public void Should_ContainVariable()
            {
                var name = Create<string>();

                var variable = new Fake<IVariable>();
                variable.CallsTo(fake => fake.Name).Returns(name);

                _registry.AddVariable(variable.FakedObject);

                var actual = _registry.ContainsVariable(name);

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Not_ContainVariable()
            {
                var actual = _registry.ContainsVariable(Create<string>());

                actual.Should().BeFalse();
            }
        }

        public class GetEnumerator : VariableRegistryFixture
        {
            [Fact]
            public void Should_Enumerate_Variablea()
            {
                var names = CreateMany<string>();

                var variables = names
                    .Select(name =>
                    {
                        var variable = new Fake<IVariable>();
                        variable.CallsTo(fake => fake.Name).Returns(name);

                        return variable.FakedObject;
                    })
                    .ToArray();

                _registry.AddVariables(variables);

                foreach (var (kvp, index) in _registry.WithIndex())
                {
                    var actual = kvp.Value;

                    actual.Should().BeSameAs(variables[index]);
                }
            }
        }
    }
}
