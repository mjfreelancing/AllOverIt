namespace AllOverIt.Fixture.Assertions
{
    /// <summary>Gets access to methods that provide the ability to assert property declarations.</summary>
    public static class Properties
    {
        /// <summary>Gets an object that provides property assertions for a specific class type.</summary>
        /// <typeparam name="TType">The class type to assert properties on.</typeparam>
        /// <returns>An object that provides property assertions for a specific class type.</returns>
        public static ClassProperties<TType> For<TType>() where TType : class
        {
            return new ClassProperties<TType>();
        }
    }
}
