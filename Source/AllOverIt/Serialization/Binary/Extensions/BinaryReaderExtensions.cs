using System;
using System.IO;

namespace AllOverIt.Serialization.Binary.Extensions
{
    public static class BinaryReaderExtensions
    {
        // TODO: To be moved as these are system extensions
        public static string ReadSafeString(this BinaryReader reader)
        {
            var hasValue = reader.ReadBoolean();

            return hasValue
                ? reader.ReadString()
                : default;
        }

        public static Guid ReadGuid(this BinaryReader reader)
        {
            var bytes = reader.ReadBytes(16);
            return new Guid(bytes);
        }

        public static object ReadEnum(this BinaryReader reader)
        {
            var valueTypeName = reader.ReadString();
            var valueType = Type.GetType(valueTypeName);                    // TODO: Check for null

            var value = reader.ReadString();
            return Enum.Parse(valueType, value);
        }

        public static TEnum ReadEnum<TEnum>(this BinaryReader reader)
        {
            var valueTypeName = reader.ReadString();
            var valueType = Type.GetType(valueTypeName);                    // TODO: Check for null

            var value = reader.ReadString();
            return (TEnum) Enum.Parse(valueType, value);
        }
    }

}
