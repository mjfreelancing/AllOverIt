namespace AllOverIt.Extensions
{
    /// <summary>Provides extension methods for nullable types.</summary>
    public static class NullableExtensions
    {
        /// <summary>Deconstructs a nullable type to retrieve its HasValue and Value properties.</summary>
        /// <typeparam name="TType">The underlying type of the nullable.</typeparam>
        /// <param name="nullable">The nullable instance.</param>
        /// <param name="hasValue">The nullable type's HasValue value.</param>
        /// <param name="value">The nullable type's value when it has one, otherwise the type's default value.</param>
        public static void Deconstruct<TType>(this TType? nullable, out bool hasValue, out TType value)
            where TType : struct
        {
            hasValue = nullable.HasValue;
            value = nullable.GetValueOrDefault();
        }
    }
}