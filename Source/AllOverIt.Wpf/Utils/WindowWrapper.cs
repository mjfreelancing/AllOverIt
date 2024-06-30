using AllOverIt.Assertion;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace AllOverIt.Wpf.Utils
{
    /// <summary>Provides P/Invoke based wrapping operations for a <see cref="Window"/>.</summary>
    public sealed class WindowWrapper : IDisposable
    {
#pragma warning disable SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll")]
        private static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        [DllImport("user32.dll")]
        private static extern IntPtr DestroyMenu(IntPtr hWnd);

#pragma warning restore SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time

        private const int GWL_STYLE = -16;

        private const int WS_MAXIMIZEBOX = 0x10000;
        private const int WS_MINIMIZEBOX = 0x20000;
        private const int WS_SYSMENU = 0x80000;

        private const uint MF_BYCOMMAND = 0x00000000;
        private const uint MF_GRAYED = 0x00000001;
        private const uint MF_ENABLED = 0x00000000;
        private const uint SC_CLOSE = 0xF060;

        private readonly IntPtr _windowHandle;
        private IntPtr _menuHandle;

        /// <summary>Constructor. Initializes the wrapper for a specified <see cref="Window"/>.</summary>
        /// <param name="window">The <see cref="Window"/> being wrapped.</param>
        public WindowWrapper(Window window)
        {
            _ = window.WhenNotNull();

            _windowHandle = new WindowInteropHelper(window).Handle;
        }

        /// <summary>Enables the Minimize button for the wrapped <see cref="Window"/>.</summary>
        /// <returns>The same <see cref="WindowWrapper"/> instance, allowing for a fluent syntax.</returns>
        public WindowWrapper EnableMinimizeButton()
        {
            _ = SetWindowLong(_windowHandle, GWL_STYLE, GetWindowLong(_windowHandle, GWL_STYLE) | WS_MINIMIZEBOX);

            return this;
        }

        /// <summary>Disables the Minimize button for the wrapped <see cref="Window"/>.</summary>
        /// <returns>The same <see cref="WindowWrapper"/> instance, allowing for a fluent syntax.</returns>
        public WindowWrapper DisableMinimizeButton()
        {
            _ = SetWindowLong(_windowHandle, GWL_STYLE, GetWindowLong(_windowHandle, GWL_STYLE) & ~WS_MINIMIZEBOX);

            return this;
        }

        /// <summary>Enables the Maximize button for the wrapped <see cref="Window"/>.</summary>
        /// <returns>The same <see cref="WindowWrapper"/> instance, allowing for a fluent syntax.</returns>
        public WindowWrapper EnableMaximizeButton()
        {
            _ = SetWindowLong(_windowHandle, GWL_STYLE, GetWindowLong(_windowHandle, GWL_STYLE) | WS_MAXIMIZEBOX);

            return this;
        }

        /// <summary>Disables the Maximize button for the wrapped <see cref="Window"/>.</summary>
        /// <returns>The same <see cref="WindowWrapper"/> instance, allowing for a fluent syntax.</returns>
        public WindowWrapper DisableMaximizeButton()
        {
            _ = SetWindowLong(_windowHandle, GWL_STYLE, GetWindowLong(_windowHandle, GWL_STYLE) & ~WS_MAXIMIZEBOX);

            return this;
        }

        /// <summary>Shows the Minimize and Maximize buttons for the wrapped <see cref="Window"/>.</summary>
        /// <returns>The same <see cref="WindowWrapper"/> instance, allowing for a fluent syntax.</returns>
        public WindowWrapper ShowMinimizeAndMaximizeButtons()
        {
            _ = SetWindowLong(_windowHandle, GWL_STYLE, GetWindowLong(_windowHandle, GWL_STYLE) | WS_MINIMIZEBOX | WS_MAXIMIZEBOX);

            return this;
        }

        /// <summary>Hides the Minimize and Maximize buttons for the wrapped <see cref="Window"/>.</summary>
        /// <returns>The same <see cref="WindowWrapper"/> instance, allowing for a fluent syntax.</returns>
        public WindowWrapper HideMinimizeAndMaximizeButtons()
        {
            _ = SetWindowLong(_windowHandle, GWL_STYLE, GetWindowLong(_windowHandle, GWL_STYLE) & ~WS_MINIMIZEBOX & ~WS_MAXIMIZEBOX);

            return this;
        }

        /// <summary>Enables the Close button for the wrapped <see cref="Window"/>.</summary>
        /// <returns>The same <see cref="WindowWrapper"/> instance, allowing for a fluent syntax.</returns>
        public WindowWrapper EnableCloseButton()
        {
            if (_menuHandle != IntPtr.Zero)
            {
                EnableMenuItem(_menuHandle, SC_CLOSE, MF_BYCOMMAND | MF_ENABLED);
            }

            return this;
        }

        /// <summary>Disables the Close button for the wrapped <see cref="Window"/>.</summary>
        /// <returns>The same <see cref="WindowWrapper"/> instance, allowing for a fluent syntax.</returns>
        public WindowWrapper DisableCloseButton()
        {
            // Capture the existing menu handle so it can be used when later enabling the button again
            _menuHandle = GetSystemMenu(_windowHandle, false);

            if (_menuHandle != IntPtr.Zero)
            {
                EnableMenuItem(_menuHandle, SC_CLOSE, MF_BYCOMMAND | MF_GRAYED);
            }

            return this;
        }

        /// <summary>Hides all buttons for the wrapped <see cref="Window"/>.</summary>
        /// <returns>The same <see cref="WindowWrapper"/> instance, allowing for a fluent syntax.</returns>
        public WindowWrapper HideAllButtons()
        {
            _ = SetWindowLong(_windowHandle, GWL_STYLE, GetWindowLong(_windowHandle, GWL_STYLE) & ~WS_SYSMENU);

            return this;
        }

        /// <summary>Shows all buttons for the wrapped <see cref="Window"/>.</summary>
        /// <returns>The same <see cref="WindowWrapper"/> instance, allowing for a fluent syntax.</returns>
        public WindowWrapper ShowAllButtons()
        {
            _ = SetWindowLong(_windowHandle, GWL_STYLE, GetWindowLong(_windowHandle, GWL_STYLE) | WS_SYSMENU);

            return this;
        }

        /// <summary>Disposes internal resources.</summary>
        public void Dispose()
        {
            if (_menuHandle != IntPtr.Zero)
            {
                _ = DestroyMenu(_menuHandle);

                _menuHandle = IntPtr.Zero;
            }
        }
    }
}