using System.Windows;
using System;
using System.Windows.Interop;
using System.Windows.Shell;
using AutoCapturer.Interop;
using MINMAXINFO = AutoCapturer.Interop.NativeMethods.MINMAXINFO;
using MONITORINFO = AutoCapturer.Interop.NativeMethods.MONITORINFO;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace AutoCapturer.Windows
{
    public class ChromeWindow : Window
    {
        public IntPtr Handle { get; private set; }

        public ChromeWindow()
        {
            this.Style = FindResource("ChromeWindowStyle") as Style;

            CommandBindings.Add(new CommandBinding(WindowSystemCommand.CloseCommand, OnClose, OnCloseExecute));
        }

        private void OnCloseExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OnClose(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            var hwndSource = PresentationSource.FromVisual(this) as HwndSource;

            this.Handle = hwndSource.Handle;

            hwndSource.AddHook(WndProc);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case NativeMethods.WM_GETMINMAXINFO:
                    WmGetMinMaxInfo(hwnd, ref lParam);
                    break;

                case NativeMethods.WM_SYSCOMMAND:
                    // TODO: Catch maximize, restore
                    break;
            }

            return IntPtr.Zero;
        }

        private void WmGetMinMaxInfo(IntPtr hwnd, ref IntPtr lParam)
        {
            var mmi = Marshal.PtrToStructure<MINMAXINFO>(lParam);
            IntPtr monitor = UnsafeNativeMethods.MonitorFromWindow(hwnd, NativeMethods.MONITOR_DEFAULTTONEAREST);

            if (monitor != IntPtr.Zero)
            {
                var mInfo = new MONITORINFO();
                mInfo.cbSize = Marshal.SizeOf(typeof(MONITORINFO));

                UnsafeNativeMethods.GetMonitorInfo(monitor, ref mInfo);

                mmi.ptMaxPosition.X = Math.Abs(mInfo.rcWork.left - mInfo.rcMonitor.left);
                mmi.ptMaxPosition.Y = Math.Abs(mInfo.rcWork.top - mInfo.rcMonitor.top);
                mmi.ptMaxSize.X = Math.Abs(mInfo.rcWork.right - mInfo.rcWork.left);
                mmi.ptMaxSize.Y = Math.Abs(mInfo.rcWork.bottom - mInfo.rcWork.top);
            }

            Marshal.StructureToPtr(mmi, lParam, true);
        }
    }
}
