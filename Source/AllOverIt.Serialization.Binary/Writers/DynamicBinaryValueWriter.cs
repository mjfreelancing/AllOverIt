using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Serialization.Binary.Readers;
using System;
using System.Linq;

namespace AllOverIt.Serialization.Binary.Writers
{
    /// <summary>A dynamic binary value writer for a given object type. This writer is typically used by an <see cref="EnrichedBinaryWriter"/>
    /// when an object type needs to be written to a stream that has no previously registered custom writer. It is assumed that the object type
    /// will be later read back using a <see cref="DynamicBinaryValueReader"/> within an <see cref="EnrichedBinaryReader"/>.<br/> This writer
    /// will enumerate all properties that are read/write, including complex objects. Complex objects will be written using a customer writer
    /// that has been registered with the associated <see cref="IEnrichedBinaryWriter"/>, if available, otherwise another <see cref="DynamicBinaryValueWriter"/>
    /// will be used.<br/> It is assumed that the reverse process will be used for reading; any type written using a <see cref="DynamicBinaryValueWriter"/>
    /// will be read back using a <see cref="DynamicBinaryValueReader"/> and any custom writers will similarly have complimentary readers. <br/> It
    /// is recommended to use custom readers and writers for performance and stream size reasons, but the dynamic readers and writers are quite
    /// useful for prototyping scenarios.</summary>
    public class DynamicBinaryValueWriter : EnrichedBinaryValueWriter
    {
        /// <summary>Constructor.</summary>
        /// <param name="type">The object type this writer write to a stream.</param>
        public DynamicBinaryValueWriter(Type type)
            : base(type)
        {
        }

        /// <inheritdoc />
        public override void WriteValue(IEnrichedBinaryWriter writer, object instance)
        {
            _ = writer.WhenNotNull(nameof(writer));
            _ = instance.WhenNotNull(nameof(instance));

            var properties = Type
                .GetPropertyInfo()
                .Where(propInfo => propInfo.CanRead && propInfo.CanWrite && !propInfo.IsIndexer());

            foreach (var propertyInfo in properties)
            {
                var propertyType = propertyInfo.PropertyType;
                var value = propertyInfo.GetValue(instance);

                // Write the property value - WriteObject() caters for class types by invoking a custom value
                // writer or creating a new (or a cached) DynamicBinaryValueWriter for the class type.
                writer.WriteObject(value, propertyInfo.PropertyType);
            }
        }
    }
}
