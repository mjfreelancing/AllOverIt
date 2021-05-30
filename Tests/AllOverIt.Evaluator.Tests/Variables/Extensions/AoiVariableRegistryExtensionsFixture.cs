using AllOverIt.Evaluator.Variables;
using AllOverIt.Evaluator.Variables.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.FakeItEasy;
using FakeItEasy;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace AllOverIt.Evaluator.Tests.Variables.Extensions
{
    public class AoiVariableRegistryExtensionsFixture : AoiFixtureBase
    {
        private readonly Fake<IAoiVariableRegistry> _registryFake;
        private readonly IAoiVariable _variable;

        public AoiVariableRegistryExtensionsFixture()
        {
            _registryFake = new Fake<IAoiVariableRegistry>();
            _variable = this.CreateStub<IAoiVariable>();
        }

        public class Add_Single : AoiVariableRegistryExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Registry_Null()
            {
                Invoking(() => AoiVariableRegistryExtensions.Add(null, _variable))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithMessage(GetExpectedArgumentNullExceptionMessage("registry"));
            }

            [Fact]
            public void Should_Throw_When_Variable_Null()
            {
                Invoking(() => _registryFake.FakedObject.Add((IAoiVariable)null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithMessage(GetExpectedArgumentNullExceptionMessage("variable"));
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

        public class Add_Multiple : AoiVariableRegistryExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Registry_Null()
            {
                Invoking(() => AoiVariableRegistryExtensions.Add(null, new[] { _variable }))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithMessage(GetExpectedArgumentNullExceptionMessage("registry"));
            }

            [Fact]
            public void Should_Throw_When_Variables_Null()
            {
                Invoking(() => _registryFake.FakedObject.Add((IAoiVariable[])null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithMessage(GetExpectedArgumentNullExceptionMessage("variables"));
            }

            [Fact]
            public void Should_Throw_When_Variables_Empty()
            {
                Invoking(() => _registryFake.FakedObject.Add(Enumerable.Empty<IAoiVariable>().ToArray()))
                    .Should()
                    .Throw<ArgumentException>()
                    .WithMessage(GetExpectedArgumentCannotBeEmptyExceptionMessage("variables"));
            }

            [Fact]
            public void Should_Call_Registry_AddVariable()
            {
                var variables = Enumerable.Range(1, 5).Select(_ => this.CreateStub<IAoiVariable>()).ToArray();

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
