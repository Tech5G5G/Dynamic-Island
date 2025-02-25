namespace Dynamic_Island.Helpers
{
    /// <summary>Helper to edit window attributes.</summary>
    public static class WindowHelper
    {
        const int DWMWA_WINDOW_CORNER_PREFERENCE = 33;

        const int WS_EX_LAYERED = 0x00080000;
        const int GWL_EXSTYLE = -20;

        /// <summary>Removes the border elements of a window; that is, its shadow and border.</summary>
        /// <param name="window">The window to remove the border elements from.</param>
        /// <returns>The <paramref name="window"/>'s handle in the form of an <see cref="nint"/>.</returns>
        public static IntPtr RemoveBorderElements(Window window)
        {
            (window.AppWindow.Presenter as OverlappedPresenter).SetBorderAndTitleBar(false, false);
            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);

            int att = 0;
            _ = PInvoke.SetWindowAttribute(hWnd, DWMWA_WINDOW_CORNER_PREFERENCE, ref att, System.Runtime.InteropServices.Marshal.SizeOf(typeof(int))); //Removes window shadow
            PInvoke.SetWindowLong(hWnd, GWL_EXSTYLE, (IntPtr)(PInvoke.GetWindowLong(hWnd, GWL_EXSTYLE) | WS_EX_LAYERED)); //Removes window border

            return hWnd;
        }

        /// <summary>Applies pill window properties to <paramref name="window"/>, like setting <see cref="AppWindow.IsShownInSwitchers"/> to <see langword="false"/>.</summary>
        /// <param name="window">The window to apply pill properties to.</param>
        public static void ApplyPillProperties(Window window)
        {
            PInvoke.SetForegroundWindow(WinRT.Interop.WindowNative.GetWindowHandle(window));
            window.AppWindow.IsShownInSwitchers = false;
            var presenter = window.AppWindow.Presenter as OverlappedPresenter;
            presenter.IsResizable = false;
            presenter.IsAlwaysOnTop = true;
        }

        /// <summary>Removes and hides the title bar from <paramref name="window"/>.</summary>
        /// <param name="window">The window whose title bar is to be hidden.</param>
        public static void HideTitleBar(Window window)
        {
            window.ExtendsContentIntoTitleBar = true;
            window.AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Collapsed;
        }

        /// <summary>Gets whether the cursor is in <paramref name="window"/>.</summary>
        /// <returns><see langword="true"/> if the cursor is in <paramref name="window"/>, otherwise, <see langword="false"/>.</returns>
        public static bool CursorInWindow(Window window)
        {
            PInvoke.GetCursorPos(out PointInt32 cursor);
            var size = window.AppWindow.Size;
            var pos = window.AppWindow.Position;
            return new Rect(pos.X, pos.Y, size.Width, size.Height).Contains(new(cursor.X, cursor.Y));
        }
    }
}
