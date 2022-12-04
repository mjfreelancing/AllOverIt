using AllOverIt.Reflection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AllOverIt.Formatters.Objects
{
    /// <summary>Provides options that determine how serialization of properties and their values are handled by <see cref="ObjectPropertySerializer"/>.</summary>
    public sealed class ObjectPropertySerializerOptions
    {
        private readonly List<Type> _ignoredTypes = new()
        {
            typeof(Task),
            typeof(Task<>)
        };

        /// <summary>Includes types that will be explicitly excluded during serialization.</summary>
        /// <remarks>Excludes {Task} and {Task&lt;>} by default.</remarks>
        public IEnumerable<Type> IgnoredTypes => _ignoredTypes;

        /// <summary>If true then null value properties will be included, otherwise they will be omitted.</summary>
        /// <remarks>This takes priority over custom filters via the <see cref="Filter"/> option.</remarks>
        public bool IncludeNulls { get; set; }

        /// <summary>If true then empty collection properties will be included, otherwise they will be omitted.</summary>
        /// <remarks>This takes priority over custom filters via the <see cref="Filter"/> option.</remarks>
        public bool IncludeEmptyCollections { get; set; }

        /// <summary>Binding options that determine how properties are resolved.</summary>
        public BindingOptions BindingOptions { get; set; } = BindingOptions.Default;

        /// <summary>Specifies an alternative output for null values.</summary>
        public string NullValueOutput { get; set; } = "<null>";

        /// <summary>Specifies an alternative output for empty string and collection values.</summary>
        public string EmptyValueOutput { get; set; } = "<empty>";

        /// <summary>Provides options that allow array values to be collated to a single value.</summary>
        /// <remarks>If a <see cref="Filter"/> has been assigned then its array options will override these settings.</remarks>
        public ObjectPropertyEnumerableOptions EnumerableOptions { get; } = new();

        /// <summary>Provides options that define root level value key names for objects that cannot be serialized due
        /// to the lack of properties.</summary>
        public ObjectPropertyRootValueOptions RootValueOptions { get; } = new();

        /// <summary>An optional filter that can be implemented to exclude properties by name or value. Values can be
        /// modified, or formatted if the filter implements <see cref="IFormattableObjectPropertyFilter"/>.</summary>
        public ObjectPropertyFilter Filter { get; set; }

        /// <summary>Clears the current list of ignored types.</summary>
        public void ClearIgnoredTypes()
        {
            _ignoredTypes.Clear();
        }

        /// <summary>Appends one or more types to be ignored.</summary>
        /// <param name="types">The array of types to be ignored.</param>
        public void AddIgnoredTypes(params Type[] types)
        {
            _ignoredTypes.AddRange(types);
        }
    }
}