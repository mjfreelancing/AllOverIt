namespace AllOverIt.Extensions
{
    /// <summary>Provides a variety of extension methods for types that require byte ordering operations.</summary>
    public static partial class EndianExtensions
    {
        internal static bool _isLittleEndian = BitConverter.IsLittleEndian;     // provide the ability to switch the flag for tests
        private static bool IsLittleEndian => _isLittleEndian;

        #region AsBigEndian

        /// <summary>Returns the same value if the current architecture is big endian, otherwise the bytes will be re-ordered.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A converted value if the current architecture is not big endian, otherwise the original value.</returns>
        public static ushort AsBigEndian(this ushort value)
        {
            return IsLittleEndian
                ? value.SwapBytes()
                : value;
        }

        /// <summary>Returns the same value if the current architecture is big endian, otherwise the bytes will be re-ordered.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A converted value if the current architecture is not big endian, otherwise the original value.</returns>
        public static short AsBigEndian(this short value)
        {
            return IsLittleEndian
                ? value.SwapBytes()
                : value;
        }

        /// <summary>Returns the same value if the current architecture is big endian, otherwise the bytes will be re-ordered.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A converted value if the current architecture is not big endian, otherwise the original value.</returns>
        public static uint AsBigEndian(this uint value)
        {
            return IsLittleEndian
                ? value.SwapBytes()
                : value;
        }

        /// <summary>Returns the same value if the current architecture is big endian, otherwise the bytes will be re-ordered.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A converted value if the current architecture is not big endian, otherwise the original value.</returns>
        public static int AsBigEndian(this int value)
        {
            return IsLittleEndian
                ? value.SwapBytes()
                : value;
        }

        /// <summary>Returns the same value if the current architecture is big endian, otherwise the bytes will be re-ordered.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A converted value if the current architecture is not big endian, otherwise the original value.</returns>
        public static ulong AsBigEndian(this ulong value)
        {
            return IsLittleEndian
                ? value.SwapBytes()
                : value;
        }

        /// <summary>Returns the same value if the current architecture is big endian, otherwise the bytes will be re-ordered.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A converted value if the current architecture is not big endian, otherwise the original value.</returns>
        public static long AsBigEndian(this long value)
        {
            return IsLittleEndian
                ? value.SwapBytes()
                : value;
        }

        /// <summary>Returns the same value if the current architecture is big endian, otherwise the bytes will be re-ordered.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A converted value if the current architecture is not big endian, otherwise the original value.</returns>
        public static float AsBigEndian(this float value)
        {
            return IsLittleEndian
                ? value.SwapBytes()
                : value;
        }

        /// <summary>Returns the same value if the current architecture is big endian, otherwise the bytes will be re-ordered.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A converted value if the current architecture is not big endian, otherwise the original value.</returns>
        public static double AsBigEndian(this double value)
        {
            return IsLittleEndian
                ? value.SwapBytes()
                : value;
        }

        /// <summary>Returns the same value if the current architecture is big endian, otherwise the bytes will be re-ordered.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A converted value if the current architecture is not big endian, otherwise the original value.</returns>
        public static decimal AsBigEndian(this decimal value)
        {
            return IsLittleEndian
                ? value.SwapBytes()
                : value;
        }

        #endregion

        #region AsLittleEndian

        /// <summary>Returns the same value if the current architecture is little endian, otherwise the bytes will be re-ordered.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A converted value if the current architecture is not little endian, otherwise the original value.</returns>
        public static ushort AsLittleEndian(this ushort value)
        {
            return IsLittleEndian
                ? value
                : value.SwapBytes();
        }

        /// <summary>Returns the same value if the current architecture is little endian, otherwise the bytes will be re-ordered.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A converted value if the current architecture is not little endian, otherwise the original value.</returns>
        public static short AsLittleEndian(this short value)
        {
            return IsLittleEndian
                ? value
                : value.SwapBytes();
        }

        /// <summary>Returns the same value if the current architecture is little endian, otherwise the bytes will be re-ordered.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A converted value if the current architecture is not little endian, otherwise the original value.</returns>
        public static uint AsLittleEndian(this uint value)
        {
            return IsLittleEndian
                ? value
                : value.SwapBytes();
        }

        /// <summary>Returns the same value if the current architecture is little endian, otherwise the bytes will be re-ordered.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A converted value if the current architecture is not little endian, otherwise the original value.</returns>
        public static int AsLittleEndian(this int value)
        {
            return IsLittleEndian
                ? value
                : value.SwapBytes();
        }

        /// <summary>Returns the same value if the current architecture is little endian, otherwise the bytes will be re-ordered.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A converted value if the current architecture is not little endian, otherwise the original value.</returns>
        public static ulong AsLittleEndian(this ulong value)
        {
            return IsLittleEndian
                ? value
                : value.SwapBytes();
        }

        /// <summary>Returns the same value if the current architecture is little endian, otherwise the bytes will be re-ordered.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A converted value if the current architecture is not little endian, otherwise the original value.</returns>
        public static long AsLittleEndian(this long value)
        {
            return IsLittleEndian
                ? value
                : value.SwapBytes();
        }

        /// <summary>Returns the same value if the current architecture is little endian, otherwise the bytes will be re-ordered.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A converted value if the current architecture is not little endian, otherwise the original value.</returns>
        public static float AsLittleEndian(this float value)
        {
            return IsLittleEndian
                ? value
                : value.SwapBytes();
        }

        /// <summary>Returns the same value if the current architecture is little endian, otherwise the bytes will be re-ordered.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A converted value if the current architecture is not little endian, otherwise the original value.</returns>
        public static double AsLittleEndian(this double value)
        {
            return IsLittleEndian
                ? value
                : value.SwapBytes();
        }

        /// <summary>Returns the same value if the current architecture is little endian, otherwise the bytes will be re-ordered.</summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A converted value if the current architecture is not little endian, otherwise the original value.</returns>
        public static decimal AsLittleEndian(this decimal value)
        {
            return IsLittleEndian
                ? value
                : value.SwapBytes();
        }

        #endregion
    }
}
