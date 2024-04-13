using System;
using System.Collections.Generic;

namespace AllOverIt.Formatters.Objects
{
    /// <summary>A base class that provides support for filtering properties and values when serializing via
    /// <see cref="ObjectPropertySerializer"/>.</summary>
    public abstract class ObjectPropertyFilter
    {
        /// <summary>The type of the current value.</summary>
        public Type? Type { get; internal set; }

        /// <summary>The value of the current value.</summary>
        public object? Value { get; internal set; }

        /// <summary>The full path, with index notation for enumerable types, within the object graph where the current value sits.</summary>
        public string? Path { get; internal set; }

        /// <summary>The full path, without index notation for enumerable types, within the object graph where the current value sits.</summary>
        public string? PropertyPath { get; internal set; }

        /// <summary>The name of the property associated with the current value. This will be null for values within a collection.</summary>
        public string? Name { get; internal set; }

        /// <summary>If the current value is within a collection then this represents its relative index.</summary>
        public int? Index { get; internal set; }

        /// <summary>Provides a collection of parent objects associated with the current value.</summary>
        public IReadOnlyCollection<ObjectPropertyParent>? Parents { get; internal set; }

        /// <summary>Provides options that allow array values to be collated to a single value.</summary>
        /// <remarks>These options override the serializer's global array handling options.</remarks>
        public ObjectPropertyEnumerableOptions EnumerableOptions { get; } = new();
        
        /// <summary>Use this method to filter out properties. Return true to include the property and false to have it excluded.</summary>
        public virtual bool OnIncludeProperty()
        {
            return true;
        }

        /// <summary>Use this method to filter out property values. Return true to include the value and false to have it excluded.</summary>
        public virtual bool OnIncludeValue()
        {
            return true;
        }
    }
}