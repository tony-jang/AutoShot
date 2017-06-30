using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using f = System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using static AutoShot.Interop.UnsafeNativeMethods;
using AutoShot.Windows;

namespace AutoShot.Worker
{
    class PrintScreenWorker : BaseWorker
    {
        Thread thr;


        public PrintScreenWorker()
        {
            thr = new Thread(new ThreadStart(() =>
            {
                do
                {
                    if (GetAsyncKeyState((int)f.Keys.PrintScreen) == -32767)
                    {
                        do
                        {
                            if (Clipboard.ContainsImage())
                            {
                                Thread.Sleep(200);
                                BitmapSource bmp = null;

                                do { } while (!GetClipboardImage(ref bmp));

                                f.IDataObject clipboardData = f.Clipboard.GetDataObject();
                                if (clipboardData != null)
                                {
                                    if (clipboardData.GetDataPresent(f.DataFormats.Bitmap))
                                    {
                                        var bitmap = (Bitmap)clipboardData.GetData(f.DataFormats.Bitmap);

                                        try
                                        {
                                            var ev = new ImageWorkEventArgs();
                                            ev.Data = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                                            OnFind(ev);
                                        }
                                        catch (NullReferenceException)
                                        {

                                            Globals.Globals.MainDispatcher.Invoke(new Action(() =>
                                            {
                                                var wdw = new PopUpWindow("예외(오류) 발생", "NullReferenceException 예외가 발생했습니다.");
                                                wdw.ShowTime(1000);
                                            }));
                                        }
                                        break;
                                    }
                                }
                            }
                            Thread.Sleep(10);
                        } while (true);

                    }
                    Thread.Sleep(10);
                } while (true);

            }));
        }

        protected override void OnFind(WorkEventArgs e)
        {
            base.OnFind(e);
        }

        public bool GetClipboardImage(ref BitmapSource img)
        {
            try { img = Clipboard.GetImage(); }
            catch { return false; }

            return true;
        }
        
    }

    public class ImageWorkEventArgs : WorkEventArgs
    {
        public ImageSource Data { get; set; }
    }
}
