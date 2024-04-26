using AllOverIt.Wpf.Utils;
using System.Windows;

namespace AllOverIt.Wpf.Extensions
{
    /// <summary>Provides several useful extension methods for use with <see cref="Window"/>.</summary>
    public static class WindowExtensions
    {
        /// <summary>Gets a <see cref="WindowWrapper"/> for a specified <see cref="Window"/>.</summary>
        /// <param name="window">The window to be wrapped.</param>
        /// <returns>A <see cref="WindowWrapper"/> for a specified <see cref="Window"/>.</returns>
        public static WindowWrapper WrapWindow(this Window window)
        {
            return new WindowWrapper(window);
        }
    }
}