//using AllOverIt.Evaluator.Tests.Variables.Helpers;
//using AllOverIt.Evaluator.Variables;
//using AllOverIt.Fixture;
//using AllOverIt.Fixture.Extensions;
//using FluentAssertions;
//using System;
//using Xunit;

//namespace AllOverIt.Evaluator.Tests.Variables
//{
//    public class VariableLookupFixture : FixtureBase
//    {
//        private VariableLookup _variableLookup;
//        private readonly IVariableRegistry _registry;

//        public VariableLookupFixture()
//        {
//            _registry = VariableLookupHelper.GetKnownVariableRegistry();
//            _variableLookup = new VariableLookup(_registry);
//        }

//        public class Constructor : VariableLookupFixture
//        {
//            [Fact]
//            public void Should_Throw_When_Variable_Registry_Null()
//            {
//                Invoking(() => _variableLookup = new VariableLookup(null))
//                    .Should()
//                    .Throw<ArgumentNullException>()
//                    .WithNamedMessageWhenNull("variableRegistry");
//            }
//        }

//        public class GetReferencedVariables : VariableLookupFixture
//        {
//            [Fact]
//            public void Should_Get_Explicit_Referenced_Variables()
//            {
//                var testCases = VariableLookupHelper.GetExpectedReferencedExplicit(_registry);

//                VariableLookupHelper.AssertExpectedLookup(
//                  testCases,
//                  variable => _variableLookup.GetReferencedVariables(variable, VariableLookupMode.Explicit));
//            }

//            [Fact]
//            public void Should_Get_All_Referenced_Variables()
//            {
//                var testCases = VariableLookupHelper.GetExpectedReferencedAll(_registry);

//                VariableLookupHelper.AssertExpectedLookup(
//                  testCases,
//                  variable => _variableLookup.GetReferencedVariables(variable, VariableLookupMode.All));
//            }
//        }

//        public class GetReferencingVariables : VariableLookupFixture
//        {
//            [Fact]
//            public void Should_Get_Explicit_Referencing_Variables()
//            {
//                var testCases = VariableLookupHelper.GetExpectedReferencingExplicit(_registry);

//                VariableLookupHelper.AssertExpectedLookup(
//                  testCases,
//                  variable => _variableLookup.GetReferencingVariables(variable, VariableLookupMode.Explicit));
//            }

//            [Fact]
//            public void Should_Get_All_Referencing_Variables()
//            {
//                var testCases = VariableLookupHelper.GetExpectedReferencingAll(_registry);

//                VariableLookupHelper.AssertExpectedLookup(
//                  testCases,
//                  variable => _variableLookup.GetReferencingVariables(variable, VariableLookupMode.All));
//            }
//        }
//    }
//}