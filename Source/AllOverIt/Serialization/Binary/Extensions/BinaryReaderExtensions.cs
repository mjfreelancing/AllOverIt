using AllOverIt.Serialization.Binary.Exceptions;
using System;
using System.IO;

namespace AllOverIt.Serialization.Binary.Extensions
{
    public static class BinaryReaderExtensions
    {
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
            var enumType = GetEnumType(reader);
            var value = reader.ReadString();

            return Enum.Parse(enumType, value);
        }

        public static TEnum ReadEnum<TEnum>(this BinaryReader reader)
        {
            var enumType = GetEnumType(reader);
            var value = reader.ReadString();

            return (TEnum) Enum.Parse(enumType, value);
        }

        private static Type GetEnumType(BinaryReader reader)
        {
            var enumTypeName = reader.ReadString();
            var enumType = Type.GetType(enumTypeName);

            if (enumType is null)
            {
                throw new BinaryReaderException($"Unknown enum type '{enumTypeName}'.");
            }

            return enumType;
        }
    }
}
