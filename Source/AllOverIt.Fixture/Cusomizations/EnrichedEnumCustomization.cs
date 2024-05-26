using AllOverIt.Patterns.Enumeration;
using AutoFixture;

namespace AllOverIt.Fixture.Cusomizations
{
    /// <summary>Provides a fixture customization for creating <typeparamref name="TEnrichedEnum"/> instances.</summary>
    /// <typeparam name="TEnrichedEnum">The <see cref="EnrichedEnum{TEnum}"/> type.</typeparam>
    public sealed class EnrichedEnumCustomization<TEnrichedEnum> : ICustomization where TEnrichedEnum : EnrichedEnum<TEnrichedEnum>
    {
        private readonly Random _random;

        /// <summary>Default constructor.</summary>
        public EnrichedEnumCustomization()
        {
#if NET8_0_OR_GREATER
            _random = Random.Shared;
#else
            _random = new Random((int) DateTime.Now.Ticks);
#endif
        }

        /// <summary>Constructor.</summary>
        /// <param name="fixture">The fixture to be customized.</param>
        public void Customize(IFixture fixture)
        {
            fixture.Customize<TEnrichedEnum>(composer => composer.FromFactory(() =>
            {
                // Using Names rather than Values just in case someone is using the same value across different names (unlikely, but...)
                var allNames = EnrichedEnum<TEnrichedEnum>.GetAllNames().ToArray();
                var index = _random.Next(allNames.Length);

                return EnrichedEnum<TEnrichedEnum>.From(allNames[index]);
            }));
        }
    }
}