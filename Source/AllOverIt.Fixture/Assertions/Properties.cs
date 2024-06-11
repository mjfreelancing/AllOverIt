namespace AllOverIt.Fixture.Assertions
{
    /// <summary>Gets access to methods that provide the ability to assert property declarations.</summary>
    public static class Properties
    {
        /// <summary>Gets an object that provides property assertions for a specific class type.</summary>
        /// <typeparam name="TType">The class type to assert properties on.</typeparam>
        /// <param name="declaredOnly">When <see langword="True"/>, base class properties will be ignored.</param>
        /// <returns>An object that provides property assertions for a specific class type.</returns>
        public static ClassProperties<TType> For<TType>(bool declaredOnly = false) where TType : class
        {
            return new ClassProperties<TType>(declaredOnly);
        }
    }
}
