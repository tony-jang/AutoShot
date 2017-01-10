using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace AutoCapturer.Globals
{
    static class Globals
    {

        /// <summary>
        /// 사용 할 수 없는 String 모음
        /// </summary>
        public static string NotAccessStr = @"\/:*?""<>|";



        public static double RatioX { get; set; }
        public static double RatioY { get; set; }

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

        public static Dispatcher MainDispatcher { get; set; }


        public static byte[] ImageSourceToBytes(BitmapEncoder encoder, ImageSource imageSource)
        {
            byte[] bytes = null;
            var bitmapSource = imageSource as BitmapSource;

            if (bitmapSource != null)
            {
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

                using (var stream = new MemoryStream())
                {
                    encoder.Save(stream);
                    bytes = stream.ToArray();
                }
            }

            return bytes;
        }



        public static string GetDownloadPath()
        {
            string pathUser = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string pathDownload = System.IO.Path.Combine(pathUser, "Downloads");

            return pathDownload;
        }

    }
}
