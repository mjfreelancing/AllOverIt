using AllOverIt.Assertion;

namespace AllOverIt.Formatters.Objects
{
    /// <summary>Contains serialization information for the parent of an object property.</summary>
    public sealed class ObjectPropertyParent
    {
        /// <summary>The name of the property. Will be null when a collection item.</summary>
        public string? Name { get; }

        /// <summary>The value of the property.</summary>
        public object? Value { get; }

        /// <summary>When an element within a collection, this is the index of the item.</summary>
        public int? Index { get; }

        /// <summary>Constructor.</summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value of the property.</param>
        /// <param name="index">When an element within a collection, this is the index of the item.</param>
        public ObjectPropertyParent(string? name, object? value, int? index)
        {
            Name = name?.WhenNotEmpty();        // Can be null but should not be empty
            Value = value;
            Index = index;
        }
    }
}
