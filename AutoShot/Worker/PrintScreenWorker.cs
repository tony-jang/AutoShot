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
using System.IO;

namespace AutoShot.Worker
{
    class PrintScreenWorker : BaseWorker
    {
        static IKeyboardMouseEvents hook;

        

        public PrintScreenWorker()
        {
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
                        BitmapSource image = null;

                        while (image == null)
                        {
                            try
                            {
                                Bitmap bmp = new Bitmap(img);
                                using (var ms = new MemoryStream())
                                {
                                    bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                    ms.Position = 0;

                                    var bi = new BitmapImage();
                                    bi.BeginInit();
                                    bi.CacheOption = BitmapCacheOption.OnLoad;
                                    bi.StreamSource = ms;
                                    bi.EndInit();

                                    image = bi;
                                    bmp.Dispose();
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                        
                        ev.Data = image;
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
