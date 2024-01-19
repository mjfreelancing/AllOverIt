using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Serialization.Binary.Writers;
using System;
using System.Linq;

namespace AllOverIt.Serialization.Binary.Readers
{
    /// <summary>A dynamic binary value reader for a given object type. This reader is typically used by an <see cref="EnrichedBinaryReader"/>
    /// when an object type needs to be read from a stream that has no previously registered custom reader. It is assumed that the object type
    /// would have been previously written using a <see cref="DynamicBinaryValueWriter"/> within an <see cref="EnrichedBinaryWriter"/>.<br/>
    /// This reader will enumerate all properties that are read/write, including complex objects. Complex objects will be read using a customer reader
    /// that has been registered with the associated <see cref="IEnrichedBinaryReader"/>, if available, otherwise another <see cref="DynamicBinaryValueReader"/>
    /// will be used.<br/> It is assumed that the reverse process will be used for reading; any type written using a <see cref="DynamicBinaryValueWriter"/>
    /// will be read back using a <see cref="DynamicBinaryValueReader"/> and any custom writers will similarly have complimentary readers. <br/> It
    /// is recommended to use custom readers and writers for performance and stream size reasons, but the dynamic readers and writers are quite
    /// useful for prototyping scenarios.</summary>
    public class DynamicBinaryValueReader : EnrichedBinaryValueReader
    {
        /// <summary>Constructor.</summary>
        /// <param name="type">The object type this reader will instantiate and populate from a stream.</param>
        public DynamicBinaryValueReader(Type type)
            : base(type)
        {
        }

        /// <inheritdoc />
        public override object ReadValue(IEnrichedBinaryReader reader)
        {
            _ = reader.WhenNotNull(nameof(reader));

            // Create an instance of the required type
            var instance = Activator.CreateInstance(Type);

            var properties = Type
               .GetPropertyInfo()
               .Where(propInfo => propInfo.CanRead && propInfo.CanWrite && !propInfo.IsIndexer());

            foreach (var propertyInfo in properties)
            {
                // Read the property value - ReadObject() caters for class types by invoking a custom value
                // reader or creating a new (or a cached) DynamicBinaryValueReader for the class type.
                var value = reader.ReadObject();

                propertyInfo.SetValue(instance, value);
            }

            return instance;
        }
    }
}
