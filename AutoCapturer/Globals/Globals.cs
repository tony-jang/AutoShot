using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace AutoCapturer.Globals
{
    static partial class Globals
    {

        // 현재 설정
        public static Setting.Setting CurrentSetting;




        /// <summary>
        /// 사용 할 수 없는 String 모음
        /// </summary>
        public static string NotAccessStr = @"\/:*?""<>|";



        public enum ThickPos
        {
            Bottom, Top, Left, Right
        }
        public static Thickness GetMargin(Thickness margin, ThickPos thickpos, double value, double Maximum = 1000, bool DisAllowMinus = true, bool AllowOverMaximum = false)
        {
            double l = margin.Left, t = margin.Top, r = margin.Right, b = margin.Bottom;
            bool lb = false, tb = false, rb = false, bb = false;

            switch (thickpos)
            {
                case ThickPos.Left: l = value; lb = true; break;
                case ThickPos.Top: t = value; tb = true; break;
                case ThickPos.Right: r = value; rb = true; break;
                case ThickPos.Bottom: b = value; bb = true; break;
            }
            if (DisAllowMinus)
            {
                if (l < 0 && lb) l = 0;
                if (t < 0 && tb) t = 0;
                if (r < 0 && rb) r = 0;
                if (b < 0 && bb) b = 0;
            }
            if (!AllowOverMaximum)
            {
                if (l > Maximum && lb) l = Maximum;
                if (t > Maximum && tb) t = Maximum;
                if (r > Maximum && rb) r = Maximum;
                if (b > Maximum && bb) b = Maximum;
            }
            return new Thickness(l, t, r, b);
        }





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


        public static BitmapSource CopyScreen()
        {
            var FullBound = Rectangle.Empty;
            foreach (var scr in Screen.AllScreens)
            {
                FullBound = Rectangle.Union(FullBound, scr.Bounds);
            }

            using (var screenBmp = new Bitmap(FullBound.Width, FullBound.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                using (var bmpGraphics = Graphics.FromImage(screenBmp))
                {
                    bmpGraphics.CopyFromScreen(FullBound.Left, 0, 0, 0, FullBound.Size);
                    return Imaging.CreateBitmapSourceFromHBitmap(
                        screenBmp.GetHbitmap(),
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());
                }
            }
        }

        #region [ Array Methods ]
        public static object[] CopyArray(List<object> list)
        {
            List<object> lists = new List<object>();
            foreach (object itm in list)
            {
                lists.Add(itm);
            }

            return lists.ToArray();
        }
        public static object[] CopyArray(ItemCollection list)
        {
            List<object> lists = new List<object>();
            foreach (object itm in list)
            {
                lists.Add(itm);
            }

            return lists.ToArray();
        }
        public static object[] CopyArray(object[] list)
        {
            List<object> lists = new List<object>();
            foreach (object itm in list)
            {
                lists.Add(itm);
            }

            return lists.ToArray();
        }

        #endregion


        #region [ Icon Methods ]
        public static ImageSource GetIcon(string filepath)
        {
            var converter = new FileToImageIconConverter(filepath);

            return converter.Icon;
        }

        public class FileToImageIconConverter
        {
            private string filePath;
            private System.Windows.Media.ImageSource icon;

            public string FilePath { get { return filePath; } }
            public ImageSource Icon
            {
                get
                {
                    if (icon == null && System.IO.File.Exists(FilePath))
                    {
                        using (System.Drawing.Icon sysicon = System.Drawing.Icon.ExtractAssociatedIcon(FilePath))
                        {
                            icon = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                                      sysicon.Handle,
                                      System.Windows.Int32Rect.Empty,
                                      System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                        }
                    }

                    return icon;
                }
            }
            public FileToImageIconConverter(string filePath)
            {
                this.filePath = filePath;
            }
        }
        #endregion

        public static Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            // BitmapImage bitmapImage = new BitmapImage(new Uri("../Images/test.png", UriKind.Relative));

            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return new Bitmap(bitmap);
            }
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
