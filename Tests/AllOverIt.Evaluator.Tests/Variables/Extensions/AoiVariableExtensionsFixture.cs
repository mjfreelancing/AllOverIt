using AllOverIt.Evaluator.Tests.Variables.Dummies;
using AllOverIt.Evaluator.Tests.Variables.Helpers;
using AllOverIt.Evaluator.Variables;
using AllOverIt.Evaluator.Variables.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using FluentAssertions;
using System;
using Xunit;

namespace AllOverIt.Evaluator.Tests.Variables.Extensions
{
    public class AoiVariableExtensionsFixture : AoiFixtureBase
    {
        public class SetVariableRegistry : AoiVariableExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Variable_Null()
            {
                Invoking(() => AoiVariableExtensions.SetVariableRegistry(null, this.CreateStub<IAoiVariableRegistry>()))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("variable");
            }

            [Fact]
            public void Should_Throw_When_Variable_Registryy_Null()
            {
                Invoking(() => this.CreateStub<IAoiVariable>().SetVariableRegistry(null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("variableRegistry");
            }

            [Fact]
            public void Should_Set_Variable_Registry()
            {
                var variable = new VariableBaseDummy(Create<string>());
                var variableRegistry = this.CreateStub<IAoiVariableRegistry>();

                variable.SetVariableRegistry(variableRegistry);

                variable.VariableRegistry.Should().BeSameAs(variableRegistry);
            }
        }

        public class GetAllReferencedVariables : AoiVariableExtensionsFixture
        {
            [Fact]
            public void Should_Get_All_Referenced_Variables()
            {
                var registry = VariableLookupHelper.GetKnownVariableRegistry();
                var testCases = VariableLookupHelper.GetExpectedReferencedAll(registry);

                VariableLookupHelper.AssertExpectedLookup(
                  testCases,
                  variable => variable.GetAllReferencedVariables());
            }
        }
    }
}