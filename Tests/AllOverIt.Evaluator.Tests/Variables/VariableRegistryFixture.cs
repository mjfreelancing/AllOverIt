using AllOverIt.Evaluator.Exceptions;
using AllOverIt.Evaluator.Tests.Variables.Dummies;
using AllOverIt.Evaluator.Variables;
using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using FakeItEasy;
using Shouldly;
using System.Collections;

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
                Should.Throw<ArgumentNullException>(() => _registry.AddVariable(null))
                    .WithNamedMessageWhenNull("variable");
            }

            [Fact]
            public void Should_Throw_When_Variable_Registered()
            {
                const string name = "xyz";

                var variable = new Fake<IVariable>();
                variable.CallsTo(fake => fake.Name).Returns(name);

                _registry.AddVariable(variable.FakedObject);

                Should.Throw<VariableException>(() => _registry.AddVariable(variable.FakedObject))
                    .WithMessage("The variable 'xyz' is already registered.");
            }

            [Fact]
            public void Should_Add_Variable()
            {
                var name = Create<string>();

                var variable = new Fake<IVariable>();
                variable.CallsTo(fake => fake.Name).Returns(name);

                _registry.AddVariable(variable.FakedObject);

                _registry.TryGetVariable(name, out var actual).ShouldBeTrue();

                actual.ShouldBeSameAs(variable.FakedObject);
            }

            [Fact]
            public void Should_Assign_VariableRegistry_To_Variable()
            {
                var variable = new VariableBaseDummy(Create<string>());

                _registry.AddVariable(variable);

                variable.VariableRegistry.ShouldBeSameAs(_registry);
            }
        }

        public class AddVariables : VariableRegistryFixture
        {
            [Fact]
            public void Should_Throw_When_Variables_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                {
                    _registry.AddVariables(null);
                })
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
                    _registry.TryGetVariable(name, out var actual).ShouldBeTrue();

                    actual.ShouldBeSameAs(variables[index]);
                }
            }
        }

        public class GetValue : VariableRegistryFixture
        {
            [Fact]
            public void Should_Throw_When_Name_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue => _registry.GetValue(stringValue),
                    "name");
            }

            [Fact]
            public void Should_Throw_When_Not_Registered()
            {
                var name = Create<string>();

                Should.Throw<VariableException>(() => { _registry.GetValue(name); })
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

                actual.ShouldBe(value);
            }
        }

        public class SetValue : VariableRegistryFixture
        {
            [Fact]
            public void Should_Throw_When_Name_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue => _registry.SetValue(stringValue, Create<double>()),
                    "name");
            }

            [Fact]
            public void Should_Throw_When_Not_Registered()
            {
                var name = Create<string>();

                Should.Throw<VariableException>(() => _registry.SetValue(name, Create<double>()))
                    .WithMessage($"The variable '{name}' is not registered");
            }

            [Fact]
            public void Should_Throw_When_Not_Mutable()
            {
                var name = Create<string>();
                var variable = new ConstantVariable(name);

                _registry.AddVariable(variable);

                Should.Throw<VariableImmutableException>(() => _registry.SetValue(name, Create<double>()))
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

                variable.Value.ShouldBe(value);
            }
        }

        public class Clear : VariableRegistryFixture
        {
            [Fact]
            public void Should_Clear_Registry()
            {
                var variable = this.CreateStub<IVariable>();

                _registry.AddVariable(variable);

                _registry.Count().ShouldBe(1);

                _registry.Clear();

                _registry.Count().ShouldBe(0);
            }
        }

        public class TryGetVariable : VariableRegistryFixture
        {
            [Fact]
            public void Should_Throw_When_Name_Null()
            {
                Should.Throw<ArgumentNullException>(() => { _registry.TryGetVariable(null, out _); })
                    .WithNamedMessageWhenNull("name");
            }

            [Fact]
            public void Should_Try_Get_Variable()
            {
                var name = Create<string>();

                var variable = new Fake<IVariable>();
                variable.CallsTo(fake => fake.Name).Returns(name);

                _registry.AddVariable(variable.FakedObject);

                _registry.TryGetVariable(name, out var actual).ShouldBeTrue();

                actual.ShouldBeSameAs(variable.FakedObject);
            }

            [Fact]
            public void Should_Not_Try_Get_Variable()
            {
                _registry.TryGetVariable(Create<string>(), out var actual).ShouldBeFalse();
            }
        }

        public class GetVariable : VariableRegistryFixture
        {
            [Fact]
            public void Should_Throw_When_Name_Null()
            {
                Should.Throw<ArgumentNullException>(() => { _ = _registry.GetVariable(null); })
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

                actual.ShouldBeSameAs(variable.FakedObject);
            }

            [Fact]
            public void Should_Throw_When_Not_Get_Variable()
            {
                var name = Create<string>();

                Should.Throw<VariableException>(() =>
                {
                    _ = _registry.GetVariable(name);
                })
                .WithMessage($"The variable '{name}' is not registered");
            }
        }

        public class ContainsVariable : VariableRegistryFixture
        {
            [Fact]
            public void Should_Throw_When_Name_Null()
            {
                Should.Throw<ArgumentNullException>(() => { _registry.ContainsVariable(null); })
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

                actual.ShouldBeTrue();
            }

            [Fact]
            public void Should_Not_ContainVariable()
            {
                var actual = _registry.ContainsVariable(Create<string>());

                actual.ShouldBeFalse();
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

                    actual.ShouldBeSameAs(variables[index]);
                }
            }
        }

        public class GetEnumerator_Explicit : VariableRegistryFixture
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

                var index = 0;

                foreach (var kvp in ((IEnumerable)_registry))
                {
                    var item = (KeyValuePair<string, IVariable>)kvp;

                    var actual = item.Value;

                    actual.ShouldBeSameAs(variables[index++]);
                }
            }
        }
    }
}
