using AllOverIt.Evaluator.Tests.Variables.Helpers;
using AllOverIt.Evaluator.Variables;
using AllOverIt.Fixture;
using AllOverIt.Fixture.FakeItEasy;
using FluentAssertions;
using System;
using Xunit;

namespace AllOverIt.Evaluator.Tests.Variables
{
    public class AoiVariableLookupFixture : AoiFixtureBase
    {
        private AoiVariableLookup _variableLookup;
        private readonly IAoiVariableRegistry _registry;

        public AoiVariableLookupFixture()
        {
            _registry = VariableLookupHelper.GetKnownVariableRegistry();
            _variableLookup = new AoiVariableLookup(_registry);
        }

        public class Constructor : AoiVariableLookupFixture
        {
            [Fact]
            public void Should_Throw_When_Variable_Registry_Null()
            {
                Invoking(() => _variableLookup = new AoiVariableLookup(null))
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithMessage(GetExpectedArgumentNullExceptionMessage("variableRegistry"));
            }

            [Fact]
            public void Should_Set_Variable_Registry()
            {
                var variableRegistry = this.CreateStub<IAoiVariableRegistry>();

                _variableLookup = new AoiVariableLookup(variableRegistry);

                _variableLookup.VariableRegistry.Should().BeSameAs(variableRegistry);
            }
        }

        public class GetReferencedVariables : AoiVariableLookupFixture
        {
            [Fact]
            public void Should_Get_Explicit_Referenced_Variables()
            {
                var testCases = VariableLookupHelper.GetExpectedReferencedExplicit(_registry);

                VariableLookupHelper.AssertExpectedLookup(
                  testCases,
                  variable => _variableLookup.GetReferencedVariables(variable, AoiVariableLookupMode.Explicit));
            }

            [Fact]
            public void Should_Get_All_Referenced_Variables()
            {
                var testCases = VariableLookupHelper.GetExpectedReferencedAll(_registry);

                VariableLookupHelper.AssertExpectedLookup(
                  testCases,
                  variable => _variableLookup.GetReferencedVariables(variable, AoiVariableLookupMode.All));
            }
        }

        public class GetReferencingVariables : AoiVariableLookupFixture
        {
            [Fact]
            public void Should_Get_Explicit_Referencing_Variables()
            {
                var testCases = VariableLookupHelper.GetExpectedReferencingExplicit(_registry);

                VariableLookupHelper.AssertExpectedLookup(
                  testCases,
                  variable => _variableLookup.GetReferencingVariables(variable, AoiVariableLookupMode.Explicit));
            }

            [Fact]
            public void Should_Get_All_Referencing_Variables()
            {
                var testCases = VariableLookupHelper.GetExpectedReferencingAll(_registry);

                VariableLookupHelper.AssertExpectedLookup(
                  testCases,
                  variable => _variableLookup.GetReferencingVariables(variable, AoiVariableLookupMode.All));
            }
        }
    }
}