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
    public class VariableExtensionsFixture : FixtureBase
    {
        public class SetVariableRegistry : VariableExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Variable_Null()
            {
                Invoking(() => VariableExtensions.SetVariableRegistry(null, this.CreateStub<IVariableRegistry>()))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("variable");
            }

            [Fact]
            public void Should_Throw_When_Variable_Registryy_Null()
            {
                Invoking(() => this.CreateStub<IVariable>().SetVariableRegistry(null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("variableRegistry");
            }

            [Fact]
            public void Should_Set_Variable_Registry()
            {
                var variable = new VariableBaseDummy(Create<string>());
                var variableRegistry = this.CreateStub<IVariableRegistry>();

                variable.SetVariableRegistry(variableRegistry);

                variable.VariableRegistry.Should().BeSameAs(variableRegistry);
            }
        }

        public class GetAllReferencedVariables : VariableExtensionsFixture
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