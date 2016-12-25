using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCapturer.Globals
{
    static class Globals
    {
        public static int GetLocationDpi(int pt, bool IsX = true)
        {
            int loc;
            using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
            {
                float dpiX = graphics.DpiX;
                float dpiY = graphics.DpiY;
                if (IsX) { loc = (int)((pt) * (100 / dpiX)); }
                else { loc = (int)((pt) * (100 / dpiY)); }
            }

            return loc;
        }
        public static int GetDpi()
        {
            using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
            {
                float dpiX = graphics.DpiX;
                return (int)dpiX;
            }
        }
    }
}
