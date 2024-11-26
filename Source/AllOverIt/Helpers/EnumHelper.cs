using System.Numerics;
using System.Runtime.InteropServices;

namespace AllOverIt.Helpers
{
    /// <summary>Provides static, general purpose, methods related to using Enums.</summary>
    public static class EnumHelper
    {
        /// <summary>Returns all possible enum values.</summary>
        /// <typeparam name="TEnumType">The Enum type.</typeparam>
        /// <returns>All possible enum values.</returns>
        public static TEnumType[] GetEnumValues<TEnumType>()
          where TEnumType : Enum
        {
            return (TEnumType[]) Enum.GetValues(typeof(TEnumType));
        }

        /// <summary>Given a bit mask as a numeric value, get an array of corresponding enum values for a given
        /// enumeration type. It is assumed each enum value will be a multiple of two.</summary>
        /// <typeparam name="TEnumType">The enumeration type.</typeparam>
        /// <typeparam name="TMaskType">The numeric type containing the bit mask.</typeparam>
        /// <param name="mask">The value of the bit mask.</param>
        /// <returns>An array of enum values for a given bit mask value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If the numeric value contains a set bit that does not correspond to an enum value.</exception>
        public static TEnumType[] GetValuesFromBitMask<TEnumType, TMaskType>(TMaskType mask)
            where TEnumType : Enum
            where TMaskType : IBitwiseOperators<TMaskType, int, int>
        {
            var enumType = typeof(TEnumType);

            var enumValues = new List<TEnumType>();

            // Iterate the size of the mask value since:
            // * If it is too big then we'll get an ArgumentOutOfRangeException exception.
            // * We don't care if it is smaller than the enum's underlying type.
            var bits = Marshal.SizeOf<TMaskType>() * 8;

            for (var i = 0; i < bits; i++)
            {
                var bitValue = 1 << i;
                var hasFlag = (mask & bitValue) != 0;

                if (hasFlag)
                {
                    if (!Enum.IsDefined(enumType, bitValue))
                    {
                        throw new ArgumentOutOfRangeException(
                            nameof(mask),
                            mask,
                            $"The value has a flag that does not correspond with the '{enumType.Name}' type.");
                    }

                    enumValues.Add((TEnumType) Enum.ToObject(enumType, bitValue));
                }
            }

            return [.. enumValues];
        }

        /// <summary>Given an enum type with a [Flags] attribute, get an array of the enum values that have been OR'd together.</summary>
        /// <typeparam name="TEnumType">The enumeration type.</typeparam>
        /// <param name="flagsValue">The combined enumeration value.</param>
        /// <returns>An array of enum values matching the values that have been OR'd together.</returns>
        public static TEnumType[] GetValuesFromEnumWithFlags<TEnumType>(TEnumType flagsValue)
            where TEnumType : Enum
        {
            var enumValues = new List<TEnumType>();

            var enumCandidates = GetEnumValues<TEnumType>();

            foreach (var candidate in enumCandidates)
            {
                if (flagsValue.HasFlag(candidate))
                {
                    enumValues.Add(candidate);
                }
            }

            return [.. enumValues];
        }
    }
}