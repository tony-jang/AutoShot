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
using Gma.System.MouseKeyHook;
using AutoShot.Utils;

namespace AutoShot.Worker
{
    class PrintScreenWorker : BaseWorker
    {
        static IKeyboardMouseEvents hook;

        

        public PrintScreenWorker()
        {
            //hr = new Thread(new ThreadStart(() =>
            //{
            //    do
            //    {
            //        if (GetAsyncKeyState((int)f.Keys.PrintScreen) == -32767)
            //        {
            //            do
            //            {
            //                if (Clipboard.ContainsImage())
            //                {
            //                    Thread.Sleep(200);
            //                    BitmapSource bmp = null;

            //                    do { } while (!GetClipboardImage(ref bmp));

            //                    f.IDataObject clipboardData = f.Clipboard.GetDataObject();
            //                    if (clipboardData != null)
            //                    {
            //                        if (clipboardData.GetDataPresent(f.DataFormats.Bitmap))
            //                        {
            //                            Bitmap bitmap = (Bitmap)clipboardData.GetData(f.DataFormats.Bitmap);

            //                            try
            //                            {
            //                                ImageWorkEventArgs ev = new ImageWorkEventArgs();
            //                                ev.Data = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            //                                OnFind(ev);
            //                            }
            //                            catch (NullReferenceException)
            //                            {

            //                                Globals.Globals.MainDispatcher.Invoke(new Action(() =>
            //                                {
            //                                    PopUps.PopUpWdw wdw = new PopUps.PopUpWdw("예외(오류) 발생", "NullReferenceException 예외가 발생했습니다.");
            //                                    wdw.ShowTime(1000);
            //                                }));
            //                            }
            //                            break;
            //                        }
            //                    }
            //                }
            //                Thread.Sleep(1);
            //            } while (true);

            //        }
            //        Thread.Sleep(1);
            //    } while (true);
            //}));
            hook = Hook.GlobalEvents();
            hook.KeyDown += Hook_KeyDown;
            ClipboardMonitor.OnClipboardChange += ClipboardMonitor_OnClipboardChange;
        }

        public bool ReadImage { get; private set; }

        private void ClipboardMonitor_OnClipboardChange(ClipboardFormat format, object data)
        {
            if (format == ClipboardFormat.Bitmap)
            {
                if (data is Image img)
                {
                    if (ReadImage)
                    {
                        ReadImage = false;

                        ImageWorkEventArgs ev = new ImageWorkEventArgs();
                        ev.Data = Clipboard.GetImage();
                        OnFind(ev);
                    }
                }
            }
            ReadImage = false;
        }

        private void Hook_KeyDown(object sender, f.KeyEventArgs e)
        {
            if (e.KeyCode == f.Keys.PrintScreen)
            {
                ReadImage = true;
            }
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
