using AllOverIt.Evaluator.Exceptions;
using AllOverIt.Evaluator.Tests.Variables.Dummies;
using AllOverIt.Evaluator.Variables;
using AllOverIt.Fixture;
using AllOverIt.Fixture.FakeItEasy;
using FakeItEasy;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace AllOverIt.Evaluator.Tests.Variables
{
    public class AoiVariableRegistryFixture : AoiFixtureBase
    {
        private AoiVariableRegistry _registry;

        public AoiVariableRegistryFixture()
        {
            _registry = new AoiVariableRegistry();
        }

        public class Constructor : AoiVariableRegistryFixture
        {
            [Fact]
            public void Should_Throw_When_Variables_Null()
            {
                Invoking(() => _registry = new AoiVariableRegistry(null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithMessage(GetExpectedArgumentNullExceptionMessage("variableRegistry"));
            }

            [Fact]
            public void Should_Assign_Variables()
            {
                var name = Create<string>();
                var variables = new Dictionary<string, IAoiVariable>();

                var variable = new Fake<IAoiVariable>();
                variable.CallsTo(fake => fake.Name).Returns(name);

                _registry = new AoiVariableRegistry(variables);
                _registry.AddVariable(variable.FakedObject);

                variables.Should().BeEquivalentTo(new Dictionary<string, IAoiVariable> { [name] = variable.FakedObject });
            }
        }

        public class AddVariable : AoiVariableRegistryFixture
        {
            [Fact]
            public void Should_Throw_When_Variable_Null()
            {
                Invoking(() => _registry.AddVariable(null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithMessage(GetExpectedArgumentNullExceptionMessage("variable"));
            }

            [Fact]
            public void Should_Check_If_Variable_Exists()
            {
                var name = Create<string>();

                var variable = new Fake<IAoiVariable>();
                variable.CallsTo(fake => fake.Name).Returns(name);

                var variablesFake = new Fake<IDictionary<string, IAoiVariable>>();

                variablesFake
                  .CallsTo(fake => fake.ContainsKey(name))
                  .Returns(false);

                _registry = new AoiVariableRegistry(variablesFake.FakedObject);

                _registry.AddVariable(variable.FakedObject);

                variablesFake
                  .CallsTo(fake => fake.ContainsKey(name))
                  .MustHaveHappened(1, Times.Exactly);
            }

            [Fact]
            public void Should_Throw_When_Variable_Registered()
            {
                const string name = "xyz";

                var variable = new Fake<IAoiVariable>();
                variable.CallsTo(fake => fake.Name).Returns(name);

                var variablesFake = new Fake<IDictionary<string, IAoiVariable>>();

                variablesFake
                  .CallsTo(fake => fake.ContainsKey(name))
                  .Returns(true);

                _registry = new AoiVariableRegistry(variablesFake.FakedObject);

                Invoking(() => _registry.AddVariable(variable.FakedObject))
                    .Should()
                    .Throw<AoiVariableException>()
                    .WithMessage("The variable 'xyz' is already registered");
            }

            [Fact]
            public void Should_Add_Variable()
            {
                var variables = new Dictionary<string, IAoiVariable>();
                var name = Create<string>();

                var variable = new Fake<IAoiVariable>();
                variable.CallsTo(fake => fake.Name).Returns(name);

                _registry = new AoiVariableRegistry(variables);
                _registry.AddVariable(variable.FakedObject);

                variables.Should().BeEquivalentTo(new Dictionary<string, IAoiVariable> { [name] = variable.FakedObject });
            }

            [Fact]
            public void Should_Assign_VariableRegistry_To_Variable()
            {
                var variable = new VariableBaseDummy(Create<string>());

                _registry.AddVariable(variable);

                variable.VariableRegistry.Should().BeSameAs(_registry);
            }
        }

        public class GetValue : AoiVariableRegistryFixture
        {
            [Fact]
            public void Should_Throw_When_Name_Null()
            {
                Invoking(() => _registry.GetValue(null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithMessage(GetExpectedArgumentNullExceptionMessage("name"));
            }

            [Fact]
            public void Should_Throw_When_Name_Empty()
            {
                Invoking(() => _registry.GetValue(string.Empty))
                    .Should()
                    .Throw<ArgumentException>()
                    .WithMessage(GetExpectedArgumentCannotBeEmptyExceptionMessage("name"));
            }

            [Fact]
            public void Should_Throw_When_Name_Whitespace()
            {
                Invoking(() => _registry.GetValue("  "))
                    .Should()
                    .Throw<ArgumentException>()
                    .WithMessage(GetExpectedArgumentCannotBeEmptyExceptionMessage("name"));
            }

            [Fact]
            public void Should_Throw_When_Not_Registered()
            {
                var name = Create<string>();

                Invoking(() => _registry.GetValue(name))
                    .Should()
                    .Throw<AoiVariableException>()
                    .WithMessage($"The variable '{name}' is not registered");
            }

            [Fact]
            public void Should_Look_Up_Variable_In_Registry()
            {
                var name = Create<string>();

                var variable = this.CreateStub<IAoiVariable>();

                var variablesFake = new Fake<IDictionary<string, IAoiVariable>>();

                variablesFake.CallsTo(fake => fake.TryGetValue(name, out variable))
                  .Returns(true);

                _registry = new AoiVariableRegistry(variablesFake.FakedObject);

                _registry.GetValue(name);

                variablesFake
                  .CallsTo(fake => fake.TryGetValue(name, out variable))
                  .MustHaveHappened(1, Times.Exactly);
            }

            [Fact]
            public void Should_Return_Value()
            {
                var name = Create<string>();
                var value = Create<double>();

                var expected = new Fake<IAoiVariable>();
                expected.CallsTo(fake => fake.Name).Returns(name);
                expected.CallsTo(fake => fake.Value).Returns(value);

                _registry.AddVariable(expected.FakedObject);

                var actual = _registry.GetValue(name);

                actual.Should().Be(value);
            }
        }

        public class SetValue : AoiVariableRegistryFixture
        {
            [Fact]
            public void Should_Throw_When_Name_Null()
            {
                Invoking(() => _registry.SetValue(null, Create<double>()))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithMessage(GetExpectedArgumentNullExceptionMessage("name"));
            }

            [Fact]
            public void Should_Throw_When_Name_Empty()
            {
                Invoking(() => _registry.SetValue(string.Empty, Create<double>()))
                    .Should()
                    .Throw<ArgumentException>()
                    .WithMessage(GetExpectedArgumentCannotBeEmptyExceptionMessage("name"));
            }

            [Fact]
            public void Should_Throw_When_Name_Whitespace()
            {
                Invoking(() => _registry.SetValue("  ", Create<double>()))
                    .Should()
                    .Throw<ArgumentException>()
                    .WithMessage(GetExpectedArgumentCannotBeEmptyExceptionMessage("name"));
            }

            [Fact]
            public void Should_Throw_When_Not_Registered()
            {
                var name = Create<string>();

                Invoking(() => _registry.SetValue(name, Create<double>()))
                    .Should()
                    .Throw<AoiVariableException>()
                    .WithMessage($"The variable '{name}' is not registered");
            }

            [Fact]
            public void Should_Look_Up_Variable_In_Registry()
            {
                var name = Create<string>();
                IAoiVariable variable = this.CreateStub<IAoiMutableVariable>();

                var variablesFake = new Fake<IDictionary<string, IAoiVariable>>();

                variablesFake
                  .CallsTo(fake => fake.TryGetValue(name, out variable))
                  .Returns(true);

                _registry = new AoiVariableRegistry(variablesFake.FakedObject);

                _registry.SetValue(name, Create<double>());

                variablesFake
                  .CallsTo(fake => fake.TryGetValue(name, out variable))
                  .MustHaveHappened(1, Times.Exactly);
            }

            [Fact]
            public void Should_Throw_When_Not_Mutable()
            {
                var name = Create<string>();
                IAoiVariable variable;

                var variablesFake = new Fake<IDictionary<string, IAoiVariable>>();

                variablesFake
                  .CallsTo(fake => fake.TryGetValue(name, out variable))
                  .Returns(true);

                _registry = new AoiVariableRegistry(variablesFake.FakedObject);

                Invoking(() => _registry.SetValue(name, Create<double>()))
                    .Should()
                    .Throw<AoiVariableNotMutableException>()
                    .WithMessage($"The variable '{name}' is not mutable");
            }

            [Fact]
            public void Should_Set_Value()
            {
                var name = Create<string>();
                var value = Create<double>();
                var variable = new AoiMutableVariable(name);

                _registry.AddVariable(variable);

                _registry.SetValue(name, value);

                variable.Value.Should().Be(value);
            }
        }
    }
}
