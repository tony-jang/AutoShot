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

        // 현재 설정
        public static Setting.Setting CurrentSetting;




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

        /// <summary>
        /// MainWindow의 Dispatcher를 나타냅니다. (MainWindow에서 초기화 할때 설정)
        /// </summary>
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

        public static double Max(double[] data)
        {
            double max = 0.0;

            foreach(double dble in data)
                if (max < dble) max = dble;

            return max;
        }

        public static double[] Copy(this double[] array)
        {
            List<double> list = new List<double>();
            foreach (double d in array)
            {
                list.Add(d);
            }

            return list.ToArray();
        }

        #region [ Math Method ]
        public static double NearDouble(double[] data, double BaseValue, FindMode mode = FindMode.None)
        {
            bool flag = false;

            List<double> list = data.Copy().ToList();

            // 초기화 작업
            foreach (double d in data.Copy())
            {
                switch (mode)
                {
                    case FindMode.None: flag = true; break;
                    case FindMode.NoOver:
                        if (d > BaseValue) list.Remove(d);
                        break;
                    case FindMode.NoUnder:
                        if (d < BaseValue) list.Remove(d);
                        break;
                }

                if (flag) break;
            }
            data = list.ToArray();


            if (data.Length == 1) return data[0];

            double Near = double.MinValue;
            foreach(double d in data)
            {
                if (Math.Abs(BaseValue - d) < Math.Abs(BaseValue - Near))
                {
                    if (d >= BaseValue && mode == FindMode.NoOver && !(Max(data) == d)) continue;
                    else if (d <= BaseValue && mode == FindMode.NoUnder) continue;
                    if (d == BaseValue) continue;

                    Near = d;
                }
            }


            return Near;
        }

        public enum FindMode
        {
            None, NoOver, NoUnder
        }

        #endregion
    }
}
