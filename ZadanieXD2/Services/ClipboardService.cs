using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace ZadanieXD2.Services
{
    class ClipboardService
    {
        // CHAT GIBIDI (rozumiem w dużej mierze zamysł, ale fajnie by to tak dokładnie ogarnąć)

        private const int WM_CLIPBOARDUPDATE = 0x031D;
        private IntPtr windowHandle;
        private HwndSource hwndSource;

        [DllImport("user32.dll")]
        private static extern bool AddClipboardFormatListener(IntPtr hwnd);

        [DllImport("user32.dll")]
        private static extern bool RemoveClipboardFormatListener(IntPtr hwnd);

        public event Action<string> ClipboardTextChanged;

        public void StartMonitoring(Window window)
        {
            windowHandle = new WindowInteropHelper(window).Handle;
            hwndSource = HwndSource.FromHwnd(windowHandle);
            hwndSource.AddHook(WndProc);
            AddClipboardFormatListener(windowHandle);
        }

        public void StopMonitoring()
        {
            RemoveClipboardFormatListener(windowHandle);
            hwndSource.RemoveHook(WndProc);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_CLIPBOARDUPDATE)
            {
                if (Clipboard.ContainsText())
                {
                    ClipboardTextChanged?.Invoke(Clipboard.GetText());
                }
                handled = true;
            }
            return IntPtr.Zero;
        }
    }
}
