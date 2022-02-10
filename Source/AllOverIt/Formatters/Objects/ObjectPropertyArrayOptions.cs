namespace AllOverIt.Formatters.Objects
{
    /// <summary>Provides options for the handling of enumerable values.</summary>
    public sealed class ObjectPropertyEnumerableOptions
    {
        /// <summary>Indicates if all values within the array should be collated to a single string.</summary>
        public bool CollateValues { get; set; }

        /// <summary>When <see cref="CollateValues"/> is True, this specifies the string separator to use.</summary>
        public string Separator { get; set; } = ", ";
    }
}