using System;
using System.Runtime.InteropServices;

namespace AutoCapturer.Interop
{
    static class UnsafeNativeMethods
    {
        [DllImport(ExternDll.User32)]
        public static extern bool GetMonitorInfo(IntPtr hMonitor, ref NativeMethods.MONITORINFO lpmi);

        [DllImport(ExternDll.User32)]
        public static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);
    }
}
