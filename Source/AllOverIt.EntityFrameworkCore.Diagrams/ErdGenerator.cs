namespace AllOverIt.EntityFrameworkCore.Diagrams
{
    /// <summary>An entity relationship diagram generator factory.</summary>
    public static class ErdGenerator
    {
        /// <summary>Creates an instance of an entity relationship diagram generator.</summary>
        /// <typeparam name="TGenerator">The entity relationship diagram generator type.</typeparam>
        /// <param name="configure">An action used to configure the generator,</param>
        /// <returns>The entity relationship diagram generator.</returns>
        public static TGenerator Create<TGenerator>(Action<ErdOptions> configure = default) where TGenerator : ErdGeneratorBase
        {
            var options = new ErdOptions();

            configure?.Invoke(options);

            var formatter = (TGenerator) Activator.CreateInstance(typeof(TGenerator), [options]);

            return formatter;
        }
    }
}