using System.Windows;
using AllOverIt.Wpf.Utils;

namespace AllOverIt.Wpf.Extensions
{
    /// <summary>Provides several useful extension methods for use with <see cref="Window"/>.</summary>
    public static class WindowExtensions
    {
        public static WindowWrapper WrapWindow(this Window window)
        {
            return new WindowWrapper(window);
        }
    }
}