using System.Collections.Generic;

namespace AllOverIt.Formatters.Objects
{
    /// <summary>Provides options for the handling of enumerable values.</summary>
    public sealed class ObjectPropertyEnumerableOptions
    {
        /// <summary>Indicates if all values within the array should be collated to a single string.</summary>
        /// <remarks>Only string and primitive collections can be collated. Arrays of complex types are ignored.</remarks>
        public bool CollateValues { get; set; }

        /// <summary>When <see cref="CollateValues"/> is True, this specifies the string separator to use.</summary>
        public string Separator { get; set; } = ", ";

        /// <summary>An optional set of property paths that will be auto-collated.</summary>
        /// <remarks>Only string and primitive collections can be collated. Paths referencing non-collections or a collection
        /// of complex types are ignored.</remarks>
        public IReadOnlyCollection<string> AutoCollatedPaths { get; set; }
    }
}