using AllOverIt.Fixture;
using AutoFixture.AutoNSubstitute;
using System.Diagnostics.CodeAnalysis;

namespace TestUtils
{
    [ExcludeFromCodeCoverage]
    /// <summary>A base class for all fixtures, using NSubstitute, providing access to a variety of useful
    /// methods that help generate automated input values.</summary>
    public abstract class NSubstituteFixtureBase : FixtureBase
    {
        /// <summary>Default constructor.</summary>
        public NSubstituteFixtureBase()
        {
            var customization = new AutoNSubstituteCustomization { GenerateDelegates = true };
            this.Customize(customization);
        }
    }
}
