using AllOverIt.Evaluator.Variables;
using AllOverIt.Evaluator.Variables.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using FakeItEasy;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace AllOverIt.Evaluator.Tests.Variables.Extensions
{
    public class VariableRegistryExtensionsFixture : AoiFixtureBase
    {
        private readonly Fake<IVariableRegistry> _registryFake;
        private readonly IVariable _variable;

        public VariableRegistryExtensionsFixture()
        {
            _registryFake = new Fake<IVariableRegistry>();
            _variable = this.CreateStub<IVariable>();
        }

        public class Add_Single : VariableRegistryExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Registry_Null()
            {
                Invoking(() => VariableRegistryExtensions.Add(null, _variable))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("registry");
            }

            [Fact]
            public void Should_Throw_When_Variable_Null()
            {
                Invoking(() => _registryFake.FakedObject.Add((IVariable)null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("variable");
            }

            [Fact]
            public void Should_Call_Registry_AddVariable()
            {
                _registryFake.FakedObject.Add(_variable);

                _registryFake
                  .CallsTo(fake => fake.AddVariable(_variable))
                  .MustHaveHappened(1, Times.Exactly);
            }
        }

        public class Add_Multiple : VariableRegistryExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Registry_Null()
            {
                Invoking(() => VariableRegistryExtensions.Add(null, new[] { _variable }))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("registry");
            }

            [Fact]
            public void Should_Throw_When_Variables_Null()
            {
                Invoking(() => _registryFake.FakedObject.Add((IVariable[])null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("variables");
            }

            [Fact]
            public void Should_Throw_When_Variables_Empty()
            {
                Invoking(() => _registryFake.FakedObject.Add(Enumerable.Empty<IVariable>().ToArray()))
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("variables");
            }

            [Fact]
            public void Should_Call_Registry_AddVariable()
            {
                var variables = Enumerable.Range(1, 5).Select(_ => this.CreateStub<IVariable>()).ToArray();

                _registryFake.FakedObject.Add(variables);

                foreach (var variable in variables)
                {
                    _registryFake
                        .CallsTo(fake => fake.AddVariable(variable))
                        .MustHaveHappened(1, Times.Exactly);
                }
            }
        }
    }
}
