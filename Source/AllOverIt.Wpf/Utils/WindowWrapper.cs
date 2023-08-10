using AllOverIt.Assertion;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace AllOverIt.Wpf.Utils
{
    public sealed class WindowWrapper : IDisposable
    {
        [DllImport("user32.dll")]
        private static extern int GetWindowLong(nint hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(nint hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        private static extern nint GetSystemMenu(nint hWnd, bool bRevert);

        [DllImport("user32.dll")]
        private static extern bool EnableMenuItem(nint hMenu, uint uIDEnableItem, uint uEnable);

        [DllImport("user32.dll")]
        private static extern nint DestroyMenu(nint hWnd);

        private const int GWL_STYLE = -16;

        private const int WS_MAXIMIZEBOX = 0x10000;
        private const int WS_MINIMIZEBOX = 0x20000;
        private const int WS_SYSMENU = 0x80000;

        private const uint MF_BYCOMMAND = 0x00000000;
        private const uint MF_GRAYED = 0x00000001;
        private const uint MF_ENABLED = 0x00000000;
        private const uint SC_CLOSE = 0xF060;

        private readonly nint _windowHandle;
        private nint _menuHandle;

        public WindowWrapper(Window window)
        {
            _ = window.WhenNotNull();

            _windowHandle = new WindowInteropHelper(window).Handle;
        }

        public WindowWrapper EnableMinimizeButton()
        {
            _ = SetWindowLong(_windowHandle, GWL_STYLE, GetWindowLong(_windowHandle, GWL_STYLE) | WS_MINIMIZEBOX);

            return this;
        }

        public WindowWrapper DisableMinimizeButton()
        {
            _ = SetWindowLong(_windowHandle, GWL_STYLE, GetWindowLong(_windowHandle, GWL_STYLE) & ~WS_MINIMIZEBOX);

            return this;
        }

        public WindowWrapper EnableMaximizeButton()
        {
            _ = SetWindowLong(_windowHandle, GWL_STYLE, GetWindowLong(_windowHandle, GWL_STYLE) | WS_MAXIMIZEBOX);

            return this;
        }

        public WindowWrapper DisableMaximizeButton()
        {
            _ = SetWindowLong(_windowHandle, GWL_STYLE, GetWindowLong(_windowHandle, GWL_STYLE) & ~WS_MAXIMIZEBOX);

            return this;
        }

        public WindowWrapper ShowMinimizeAndMaximizeButtons()
        {
            _ = SetWindowLong(_windowHandle, GWL_STYLE, GetWindowLong(_windowHandle, GWL_STYLE) | WS_MAXIMIZEBOX | WS_MINIMIZEBOX);

            return this;
        }

        public WindowWrapper HideMinimizeAndMaximizeButtons()
        {
            _ = SetWindowLong(_windowHandle, GWL_STYLE, GetWindowLong(_windowHandle, GWL_STYLE) & ~WS_MAXIMIZEBOX & ~WS_MINIMIZEBOX);

            return this;
        }

        public WindowWrapper EnableCloseButton()
        {
            if (_menuHandle != nint.Zero)
            {
                EnableMenuItem(_menuHandle, SC_CLOSE, MF_BYCOMMAND | MF_ENABLED);
            }

            return this;
        }

        public WindowWrapper DisableCloseButton()
        {
            // Capture the existing menu handle so it can be used when later enabling the button again
            _menuHandle = GetSystemMenu(_windowHandle, false);

            if (_menuHandle != nint.Zero)
            {
                EnableMenuItem(_menuHandle, SC_CLOSE, MF_BYCOMMAND | MF_GRAYED);
            }

            return this;
        }

        public WindowWrapper HideAllButtons()
        {
            _ = SetWindowLong(_windowHandle, GWL_STYLE, GetWindowLong(_windowHandle, GWL_STYLE) & ~WS_SYSMENU);

            return this;
        }

        public WindowWrapper ShowAllButtons()
        {
            _ = SetWindowLong(_windowHandle, GWL_STYLE, GetWindowLong(_windowHandle, GWL_STYLE) | WS_SYSMENU);

            return this;
        }

        public void Dispose()
        {
            if (_menuHandle != nint.Zero)
            {
                DestroyMenu(_menuHandle);
                _menuHandle = nint.Zero;
            }
        }
    }
}