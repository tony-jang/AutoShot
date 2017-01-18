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
using static AutoCapturer.Interop.UnsafeNativeMethods;

namespace AutoCapturer.Worker
{
    class ImgFromPrtScrWorker : BaseWorker
    {
        Thread thr;


        public ImgFromPrtScrWorker()
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
                                        Bitmap bitmap = (Bitmap)clipboardData.GetData(f.DataFormats.Bitmap);

                                        try
                                        {
                                            ImageWorkEventArgs ev = new ImageWorkEventArgs();
                                            ev.Data = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                                            OnFind(ev);
                                        }
                                        catch (NullReferenceException)
                                        {

                                            Globals.Globals.MainDispatcher.Invoke(new Action(() =>
                                            {
                                                PopUps.PopUpWdw wdw = new PopUps.PopUpWdw("예외(오류) 발생", "NullReferenceException 예외가 발생했습니다.");
                                                wdw.ShowTime(1000);
                                            }));
                                        }
                                        break;
                                    }
                                }
                            }
                            Thread.Sleep(1);
                        } while (true);

                    }
                    Thread.Sleep(1);
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

        public override void Work()
        {
            thr.SetApartmentState(ApartmentState.STA);
            thr.Start();
        }

        public override void Rest()
        {
            Stop();
        }

        public override void Stop()
        {
            thr.Abort();
        }
    }

    public class ImageWorkEventArgs : WorkEventArgs
    {
        public ImageSource Data { get; set; }
    }
}
